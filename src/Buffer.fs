namespace PieceTable

open Types

(* Inspired by Chris Okasaki's Red Black Tree design and F# implementation at 
 * https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Binary_Search_Trees *)

module Buffer =
    (* Type abbreviations. *)
    type private InsertedLength = int
    type private Key = int (* The node's index (0, 1, 2, 3, etc.). *)
    type private Value = string

    (* Buffer type definition as a red black tree. *)
    [<Struct>]
    type Colour = R | B
    type BufferType = E | T of Colour * BufferType * Key * Value * BufferType

    (* Discriminated union for handling different append cases. *)
    [<Struct>]
    type private AppendResult =
        | FullAppend of faTree:BufferType
        | PartialAppend of paTree:BufferType * paKey:Key * insLength:InsertedLength
        | BufferWasFull of bufFullKey:Key

    (* The maximum number of characters permitted in a buffer, before we create a new one. *)
    [<Literal>]
    let private MaxBufferLength = 65535

    /// An empty tree.
    let empty = E

    let private balance = function                                  (* Red nodes in relation to black root *)
        | B, T(R, T(R, a, x, s1, b), y, s2, c), z, s3, d            (* Left, left *)
        | B, T(R, a, x, s1, T(R, b, y, s2, c)), z, s3, d            (* Left, right *)
        | B, a, x, s1, T(R, T(R, b, y, s2, c), z, s3, d)            (* Right, left *)
        | B, a, x, s1, T(R, b, y, s2, T(R, c, z, s3, d))            (* Right, right *)
            -> T(R, T(B, a, x, s1, b), y, s2, T(B, c, z, s3, d))
        | c, l, x, s1, r -> T(c, l, x, s1, r)

    let private insert key value tree: BufferType =
        let rec ins = function
            | E -> T(R, E, key, value, E)
            | T(c, a, curKey, curVal, b) ->
                (* We only ever want to insert at the end of the buffer tree. *)
                balance(c, a, curKey, curVal, ins b)

        (* Forcing root node to be black *)                
        match ins tree with
            | E -> failwith "Should never return empty from an insert"
            | T(_, l, k, v, r) -> T(B, l, k, v, r)

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
    let append (str: string) buffer =
        let rec loop = function
            | E -> (* Only ever matched if whole tree is empty. *)
                if str.Length >= MaxBufferLength
                then 
                    let nextIndex = MaxBufferLength
                    let tree = T(R, E, 0, str[..MaxBufferLength - 1], E)
                    PartialAppend(tree, 0, nextIndex)
                else 
                    let tree = T(R, E, 0, str, E)
                    FullAppend(tree)
            | T(c, a, curKey, curVal, b) -> 
                match b with
                | E ->
                    if curVal.Length + str.Length <= MaxBufferLength
                    then 
                        let tree = balance(c, a, curKey, curVal + str, b)
                        FullAppend(tree)
                    elif curVal.Length = MaxBufferLength
                    then BufferWasFull(curKey)
                    elif curVal.Length < MaxBufferLength && curVal.Length + str.Length > MaxBufferLength
                    then 
                        let remainingBufferLength = MaxBufferLength - curVal.Length
                        let fitString = str[0..remainingBufferLength - 1]
                        let tree = T(c, a, curKey, curVal + fitString, b)
                        PartialAppend(tree, curKey, fitString.Length)
                    else failwith "unexpected Buffer.tryAppend case"
                | T(_, _, _, _, _) ->
                    match loop b with
                    | FullAppend newRight ->
                        let reconstructTree = T(c, a, curKey, curVal, newRight)
                        FullAppend(reconstructTree)
                    | PartialAppend(newRight, maxKey, insLength) ->
                        let reconstructTree = T(c, a, curKey, curVal, newRight)
                        PartialAppend(reconstructTree, maxKey, insLength)
                    | BufferWasFull key -> BufferWasFull key

        match loop buffer with
        | FullAppend tree -> tree
        | PartialAppend(partialTree, maxKey, insLength) ->
            (* Insert the rest of the string into the tree. *)
            insertLongString str[insLength..] maxKey partialTree 
        | BufferWasFull maxKey ->
            (* Insert the string into the tree. *)
            insertLongString str maxKey buffer

    /// Create a buffer with a string.
    let createWithString str = append str E

    /// Find the string associated with a particular key.
    let rec private nodeSubstring key startPos endPos = function
        | E -> ""
        | T(_, l, curKey, value, r) ->
            if key = curKey 
            then value[startPos..endPos] (* Returns valid substring even if end is out of bounds. *)
            elif key < curKey 
            then nodeSubstring key startPos endPos l
            else nodeSubstring key startPos endPos r

    /// Gets text in a buffer at a specific span.
    let substring (span: SpanType) tree = 
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
        then nodeSubstring startKey startBufferIndex endBufferIndex tree
        elif startKey = endKey - 1
        then
            let startStr = nodeSubstring startKey startBufferIndex MaxBufferLength tree
            let endStr = nodeSubstring endKey 0 endBufferIndex tree
            startStr + endStr
        else 
            let startStr = nodeSubstring startKey startBufferIndex MaxBufferLength tree
            let endStr = nodeSubstring endKey 0 endBufferIndex tree
            
            let midStrRange = [|startKey + 1..endKey - 1|]
            let midStr = Array.fold (fun acc key -> acc + (nodeSubstring key 0 MaxBufferLength tree)) "" midStrRange
            startStr + midStr + endStr

    /// For testing/debugging purposes. 
    /// Performs an in-order travesal of the buffer's tree and adds each node's length to a list.
    /// Returns a list of ints representing the length of each buffer chronologically.
    let lengthAsList buffer =
        let rec traverse tree (accList: int list) =
            match tree with
            | E -> accList
            | T(_,l,_,v,r) ->
                (traverse l accList) @ [v.Length] |> traverse r
        traverse buffer []

    /// For testing/debugging purposes, returns all text in a buffer through an in-order traversal.
    /// Highly recommended not to use this in an application as it takes O(n) time and is therefore slow.
    /// Instead, use the Buffer.textInSpan method to get a section of text from the buffer.
    let text buffer =
        let rec traverse tree accText =
            match tree with
            | E -> accText
            | T(_,l,_,v,r) ->
                (traverse l accText) + v |> traverse r
        traverse buffer ""

    /// OOP API for substring method for testing, as SpanType is internal to this assembly.
    type BufferType with
        member this.Substring(index, length) = 
            let span = Span.createWithLength index length
            substring span this