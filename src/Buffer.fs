namespace PieceTable

open Types
open UnicodeString
open System.Globalization

(* 
 * This buffer uses a balanced binary tree to store 
 * UnicodeStringType lengths of 1024 in different nodes.
 * This helps us to bypass the 2 GB size limitation of objects in .NET, 
 * maximise memory sharing (a string A containing a substring of string B does not
 * share memory, but splitting the string in this way means parts will be shared)
 * and it also enables us to handle input more performantly.
 *
 * Namely, in the standard case where a user inputs text that the .NET string type 
 * is well suited for, we will use a string which gives us better performance.
 * In case the user inputs text containing a grapheme cluster, we use a StringInfo type
 * which means only nodes containing a StringInfo type are impacted by slow performance.
 * The logic switching between a normal string and StringInfo is encapsulated in the
 * UnicodeString module, but the performance advantages are part of this buffer's design.
 *
 * The balanced binary tree used is inspired by Prabhakar Ragde's AA Tree design,
 * available at the below link.
 * https://arxiv.org/pdf/1412.4882.pdf
 *
 * An F# implementation of all the features described in that paper is available at
 * the following link.
 * http://github.com/hummy123/functional-aa-Tree/
 *)

 open AaTree

module Buffer =
    let private createNode key str = { Key = key; Value = str }

    let rec insert (item: BufferNode) = function
        | E -> T(1, E, item, E)
        | T(h, l, v, r) ->
            split <| (skew <| T(h, l, v, insert item r))

    (* Discriminated union for handling different append cases. *)
    [<Struct>]
    type private AppendResult =
        | FullAdd of faTree:AaTree<BufferNode>
        | PartialAdd of paTree:BufferTree * paKey:int * insLength:int
        | BufferWasFull of bufFullKey:int

    (* The maximum numbeEcharacters permitted in a buffer, before we create a new one. *)
    [<Literal>]
    let MaxBufferLength = 1024

    /// An empty buffer.
    let empty = {Tree = E; Length = 0}

    /// Inserts a long string (greater than the max buffer length) into a tree,
    /// by splitting the string and adding it to multiple buffer nodes.
    /// Asumes that the largest buffer in the tree is already filled to max buffer length.
    /// There is also no harm using this if the string is less than the max buffer length.
    let inline private insertLongString (str: UnicodeStringType) maxKeyInTree tree =
        let rec loop curKey loopNum start newTree =
            if start >= str.Length 
            then newTree
            else
                let subStr = str[start..(loopNum * MaxBufferLength) - 1] |> UnicodeString.create
                let nextKey = curKey + 1
                let node = { Key = nextKey; Value = subStr }
                let nextLoopNum = loopNum + 1
                let nextTree = insert node newTree
                let nextStart = start + MaxBufferLength
                loop nextKey nextLoopNum nextStart nextTree
        loop maxKeyInTree 1 0 tree

    /// Append a string to the buffer.
    let inline private add (str: UnicodeStringType) tree =
        let rec loop = function
            | E -> (* Only ever matched if whole tree is empty. *)
                if str.Length >= MaxBufferLength
                then 
                    let nextIndex = MaxBufferLength
                    let str = str[..MaxBufferLength - 1] |> UnicodeString.create
                    let node = createNode 0 str
                    let newTree = leaf node
                    PartialAdd(newTree, 0, nextIndex)
                else 
                    let node = createNode 0 str
                    let newTree = leaf node
                    FullAdd(newTree)
            | T(c, a, (curNode: BufferNode), b) -> 
                match b with
                | E ->
                    if curNode.Value.Length + str.Length <= MaxBufferLength
                    then
                        let node = { curNode with Value = curNode.Value + str }
                        let newTree = T(c, a, node, b)
                        FullAdd(newTree)
                    elif curNode.Value.Length = MaxBufferLength
                    then BufferWasFull(curNode.Key)
                    elif curNode.Value.Length < MaxBufferLength && curNode.Value.Length + str.Length > MaxBufferLength
                    then 
                        let remainingBufferLength = MaxBufferLength - curNode.Value.Length
                        let fitString = str[0..remainingBufferLength - 1] |> UnicodeString.create
                        let node = { curNode with Value = curNode.Value + fitString }
                        let newTree = T(c, a, node, b)
                        PartialAdd(newTree, node.Key, fitString.Length)
                    else failwith "unexpected Buffer.tryAppend case"
                | T(_, _, _, _) ->
                    match loop b with
                    | FullAdd newRight ->
                        let reconstructTree = BT(c, a, curKey, curVal, newRight)
                        FullAdd(reconstructTree)
                    | PartialAdd(newRight, maxKey, insLength) ->
                        let reconstructTree = BT(c, a, curKey, curVal, newRight)
                        PartialAdd(reconstructTree, maxKey, insLength)
                    | BufferWasFull key -> BufferWasFull key

        match loop tree with
        | FullAdd tree -> tree
        | PartialAdd(partialTree, maxKey, insLength) ->
            (* Insert the rest of the string into the tree. *)
            let str = str[insLength..] |> UnicodeString.create
            insertLongString str maxKey partialTree 
        | BufferWasFull maxKey ->
            (* Insert the string into the tree. *)
            insertLongString str maxKey tree

    /// Intended to be used for testing string length, etc. as it is easier to pass in a string..
    let appendString (str: string) buffer =
        let str = UnicodeString.create str
        let tree = add str buffer.Tree
        { Tree = tree; Length = buffer.Length + str.Length }

    let appendUnicode (str: UnicodeStringType) buffer =
        { Tree = add str buffer.Tree; Length = buffer.Length + str.Length }
                
    /// Create a buffer with a string, intended for testing.
    let inline createWithString str = appendString str empty

    let inline createWithUnicode str = appendUnicode str empty

    /// Gets text in a buffer at a specific span.
    let substring start length buffer = 
        let rec traverse startKey (endKey: int) startIndex endIndex node (acc: string) =
            match node with
            | E -> acc
            | T(_, l, node, r) ->
                let left = 
                    if startKey < node.Key
                    then traverse startKey endKey startIndex endIndex l acc
                    else acc

                let middle = (* Handle different cases like start, end, etc. *)
                    if node.Key = startKey && node.Key = endKey (* In partial range. *)
                    then left + node.Value[startIndex..endIndex]
                    elif node.Key > startKey && node.Key < endKey (* In full range. *)
                    then left + node.Value.String
                    elif node.Key = startKey (* The range starts at this node but ends later. *)
                    then left + node.Value[startIndex..]
                    elif node.Key = endKey (* The range ends at this node but starts before. *)
                    then left + node.Value[..endIndex]
                    elif startKey > node.Key (* We are before our range. *)
                    then left
                    elif endKey < node.Key (* We are after our range. *)
                    then left
                    else failwith "unexpected buffer substring/traverse case"

                if endKey > node.Key
                then traverse startKey endKey startIndex endIndex r middle
                else middle

        (* Calculate the buffer key we need to traverse to find the span's data. *)
        let startKey = start / MaxBufferLength
        let startBufferIndex = 
            if startKey = 0 
            then start
            else start % MaxBufferLength

        let finish = start + length
        let endKey = finish / MaxBufferLength
        let endBufferIndex = 
            if endKey = 0 
            then finish - 1
            else (finish % MaxBufferLength) - 1
        
        traverse startKey endKey startBufferIndex endBufferIndex buffer.Tree ""

    /// For testing/debugging purposes. 
    /// Performs an in-order travesal of the buffer's tree and adds each node's length to a list.
    /// Returns a list of ints representing the length of each buffer chronologically.
    let lengthAsList buffer =
        fold (fun acc (node: BufferNode) -> acc @ [node.Value.String.Length]) [] buffer.Tree

    /// For testing/debugging purposes, returns all text in a buffer through an in-order traversal.
    /// Highly recommended not to use this in an application as it takes O(n) time and is therefore slow.
    /// Instead, use the Buffer.textInSpan method to get a section of text from the buffer.
    let text buffer = 
        fold (fun acc node -> acc + node.Value.String) "" buffer.Tree

    /// OOP API for substring method for testing, as SpanType is internal to this assembly.
    type BufferType with
        member this.Substring(index, length) = 
            substring index length this
