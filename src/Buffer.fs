namespace PieceTable

open Types

(* Inspired by Chris Okasaki's Red Black Tree design and F# implementation at 
 * https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Binary_Search_Trees *)

module Buffer =
    (* Discriminated union for handling different append cases. *)
    [<Struct>]
    type private AppendResult =
        | FullAdd of faTree:BufferTree
        | PartialAdd of paTree:BufferTree * paKey:Key * insLength:InsertedLength
        | BufferWasFull of bufFullKey:Key

    (* The maximum number of characters permitted in a buffer, before we create a new one. *)
    [<Literal>]
    let private MaxBufferLength = 65535

    /// An empty buffer.
    let empty = {Tree = Empty; Length = 0}

    let private balance = function                                        (* Red nodes in relation to black root *)
        | B, Tree(R, Tree(R, a, x, s1, b), y, s2, c), z, s3, d            (* Left, left *)
        | B, Tree(R, a, x, s1, Tree(R, b, y, s2, c)), z, s3, d            (* Left, right *)
        | B, a, x, s1, Tree(R, Tree(R, b, y, s2, c), z, s3, d)            (* Right, left *)
        | B, a, x, s1, Tree(R, b, y, s2, Tree(R, c, z, s3, d))            (* Right, right *)
            -> Tree(R, Tree(B, a, x, s1, b), y, s2, Tree(B, c, z, s3, d))
        | c, l, x, s1, r -> Tree(c, l, x, s1, r)

    let private insert key value tree: BufferTree =
        let rec ins = function
            | Empty -> Tree(R, Empty, key, value, Empty)
            | Tree(c, a, curKey, curVal, b) ->
                (* We only ever want to insert at the end of the buffer tree. *)
                balance(c, a, curKey, curVal, ins b)

        (* Forcing root node to be black *)                
        match ins tree with
            | Empty -> failwith "Should never return empty from an insert"
            | Tree(_, l, k, v, r) -> Tree(B, l, k, v, r)

    /// Inserts a long string (greater than the max buffer length) into a tree,
    /// by splitting the string and adding it to multiple buffer nodes.
    /// Asumes that the largest buffer in the tree is already filled to max buffer length.
    /// There is also no harm using this if the string is less than the max buffer length.
    let private insertLongString (str: string) maxKeyInTree tree =
        let rec loop curKey loopNum start newTree =
            if start >= str.Length 
            then newTree
            else
                let subStr = str[start..(loopNum * MaxBufferLength) - 1]
                let nextKey = curKey + 1
                let nextLoopNum = loopNum + 1
                let nextTree = insert nextKey subStr newTree
                let nextStart = start + MaxBufferLength
                loop nextKey nextLoopNum nextStart nextTree
        loop maxKeyInTree 1 0 tree

    /// Append a string to the buffer.
    let private add (str: string) tree =
        let rec loop = function
            | Empty -> (* Only ever matched if whole tree is empty. *)
                if str.Length >= MaxBufferLength
                then 
                    let nextIndex = MaxBufferLength
                    let newTree = Tree(R, Empty, 0, str[..MaxBufferLength - 1], Empty)
                    PartialAdd(newTree, 0, nextIndex)
                else 
                    let newTree = Tree(R, Empty, 0, str, Empty)
                    FullAdd(newTree)
            | Tree(c, a, curKey, curVal, b) -> 
                match b with
                | Empty ->
                    if curVal.Length + str.Length <= MaxBufferLength
                    then 
                        let newTree = balance(c, a, curKey, curVal + str, b)
                        FullAdd(newTree)
                    elif curVal.Length = MaxBufferLength
                    then BufferWasFull(curKey)
                    elif curVal.Length < MaxBufferLength && curVal.Length + str.Length > MaxBufferLength
                    then 
                        let remainingBufferLength = MaxBufferLength - curVal.Length
                        let fitString = str[0..remainingBufferLength - 1]
                        let newTree = Tree(c, a, curKey, curVal + fitString, b)
                        PartialAdd(newTree, curKey, fitString.Length)
                    else failwith "unexpected Buffer.tryAppend case"
                | Tree(_, _, _, _, _) ->
                    match loop b with
                    | FullAdd newRight ->
                        let reconstructTree = Tree(c, a, curKey, curVal, newRight)
                        FullAdd(reconstructTree)
                    | PartialAdd(newRight, maxKey, insLength) ->
                        let reconstructTree = Tree(c, a, curKey, curVal, newRight)
                        PartialAdd(reconstructTree, maxKey, insLength)
                    | BufferWasFull key -> BufferWasFull key

        match loop tree with
        | FullAdd tree -> tree
        | PartialAdd(partialTree, maxKey, insLength) ->
            (* Insert the rest of the string into the tree. *)
            insertLongString str[insLength..] maxKey partialTree 
        | BufferWasFull maxKey ->
            (* Insert the string into the tree. *)
            insertLongString str maxKey tree

    let append str buffer =
        let tree = add str buffer.Tree
        { Tree = tree; Length = buffer.Length + str.Length }
                
    /// Create a buffer with a string.
    let createWithString str = append str empty

    /// Find the string associated with a particular key.
    let rec private nodeSubstring key startPos endPos = function
        | Empty -> ""
        | Tree(_, l, curKey, value, r) ->
            if key = curKey 
            then value[startPos..endPos] (* Returns valid substring even if end is out of bounds. *)
            elif key < curKey 
            then nodeSubstring key startPos endPos l
            else nodeSubstring key startPos endPos r

    /// Gets text in a buffer at a specific span.
    let substring (span: SpanType) buffer = 
        (* Calculate the buffer key we need to traverse to find the span's data. *)
        let startKey = span.Start / MaxBufferLength
        let startBufferIndex = 
            if startKey = 0 
            then span.Start
            else span.Start % MaxBufferLength

        let endPos = Span.stop span
        let endKey = endPos / MaxBufferLength
        let endBufferIndex = 
            if endKey = 0 
            then endPos - 1
            else (endPos % MaxBufferLength) - 1
        
        if startKey = endKey 
        then nodeSubstring startKey startBufferIndex endBufferIndex buffer.Tree
        elif startKey = endKey - 1
        then
            let startStr = nodeSubstring startKey startBufferIndex MaxBufferLength buffer.Tree
            let endStr = nodeSubstring endKey 0 endBufferIndex buffer.Tree
            startStr + endStr
        else 
            let startStr = nodeSubstring startKey startBufferIndex MaxBufferLength buffer.Tree
            let endStr = nodeSubstring endKey 0 endBufferIndex buffer.Tree
            
            let midStrRange = [|startKey + 1..endKey - 1|]
            let midStr = 
                Array.fold (fun acc key -> acc + (nodeSubstring key 0 MaxBufferLength buffer.Tree)) "" midStrRange
            startStr + midStr + endStr

    /// Visits every node in the buffer and returns a string.
    let stringLength buffer =
        let rec traverse tree (acc: int) =
            match tree with
            | Empty -> acc
            | Tree(_, l, _, v, r) ->
                (traverse l acc) + v.Length |> traverse r
        traverse buffer.Tree 0

    /// For testing/debugging purposes. 
    /// Performs an in-order travesal of the buffer's tree and adds each node's length to a list.
    /// Returns a list of ints representing the length of each buffer chronologically.
    let lengthAsList buffer =
        let rec traverse tree (accList: int list) =
            match tree with
            | Empty -> accList
            | Tree(_,l,_,v,r) ->
                (traverse l accList) @ [v.Length] |> traverse r
        traverse buffer.Tree []

    /// For testing/debugging purposes, returns all text in a buffer through an in-order traversal.
    /// Highly recommended not to use this in an application as it takes O(n) time and is therefore slow.
    /// Instead, use the Buffer.textInSpan method to get a section of text from the buffer.
    let text buffer =
        let rec traverse tree accText =
            match tree with
            | Empty -> accText
            | Tree(_,l,_,v,r) ->
                (traverse l accText) + v |> traverse r
        traverse buffer.Tree ""

    /// OOP API for substring method for testing, as SpanType is internal to this assembly.
    type BufferType with
        member this.Substring(index, length) = 
            let span = Span.createWithLength index length
            substring span this