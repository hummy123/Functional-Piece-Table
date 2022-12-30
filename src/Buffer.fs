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

module Buffer =
    let private skew = function
<<<<<<< HEAD
        | BT(lvx, BT(lvy, a, ky, vy, b), kx, vx, c) when lvx = lvy
            -> BT(lvx, a, ky, vy, BT(lvx, b, kx, vx, c))
        | t -> t

    let private split = function
        | BT(lvx, a, kx, vx, BT(lvy, b, ky, vy, BT(lvz, c, kz, vz, d))) 
            when lvx = lvy && lvy = lvz
              -> BT(lvx + 1, BT(lvx, a, kx, vx, b), ky, vy, BT(lvx, c, kz, vz, d))
        | t -> t

    let rec insert key value = function
        | BE -> BT(1, BE, key, value, BE)
        | BT(h, l, k, v, r) as node ->
            split <| (skew <| BT(h, l, k, v, insert key value r))
=======
        | T(lvx, T(lvy, a, ky, vy, b), kx, vx, c) when lvx = lvy
            -> T(lvx, a, ky, vy, T(lvx, b, kx, vx, c))
        | t -> t

    let private split = function
        | T(lvx, a, kx, vx, T(lvy, b, ky, vy, T(lvz, c, kz, vz, d))) 
            when lvx = lvy && lvy = lvz
              -> T(lvx + 1, T(lvx, a, kx, vx, b), ky, vy, T(lvx, c, kz, vz, d))
        | t -> t

    let rec insert key value = function
        | E -> T(1, E, key, value, E)
        | T(h, l, k, v, r) as node ->
            split <| (skew <| T(h, l, k, v, insert key value r))
>>>>>>> main

    (* Discriminated union for handling different append cases. *)
    [<Struct>]
    type private AppendResult =
        | FullAdd of faTree:BufferTree
        | PartialAdd of paTree:BufferTree * paKey:Key * insLength:InsertedLength
        | BufferWasFull of bufFullKey:Key

    (* The maximum numbeEcharacters permitted in a buffer, before we create a new one. *)
    [<Literal>]
    let MaxBufferLength = 1024

    /// An empty buffer.
<<<<<<< HEAD
    let empty = {Tree = BE; Length = 0}
=======
    let empty = {Tree = E; Length = 0}
>>>>>>> main

    /// Inserts a long string (greater than the max buffer length) into a tree,
    /// by splitting the string and adding it to multiple buffer nodes.
    /// Asumes that the largest buffer in the tree is already filled to max buffer length.
    /// There is also no harm using this if the string is less than the max buffer length.
    let private insertLongString (str: UnicodeStringType) maxKeyInTree tree =
        let rec loop curKey loopNum start newTree =
            if start >= str.Length 
            then newTree
            else
                let subStr = str[start..(loopNum * MaxBufferLength) - 1] |> UnicodeString.create
                let nextKey = curKey + 1
                let nextLoopNum = loopNum + 1
                let nextTree = insert nextKey subStr newTree
                let nextStart = start + MaxBufferLength
                loop nextKey nextLoopNum nextStart nextTree
        loop maxKeyInTree 1 0 tree

    /// Append a string to the buffer.
    let private add (str: UnicodeStringType) tree =
        let rec loop = function
<<<<<<< HEAD
            | BE -> (* Only ever matched if whole tree is empty. *)
=======
            | E -> (* Only ever matched if whole tree is empty. *)
>>>>>>> main
                if str.Length >= MaxBufferLength
                then 
                    let nextIndex = MaxBufferLength
                    let str = str[..MaxBufferLength - 1] |> UnicodeString.create
<<<<<<< HEAD
                    let newTree = BT(1, BE, 0, str, BE)
                    PartialAdd(newTree, 0, nextIndex)
                else 
                    let newTree = BT(1, BE, 0, str, BE)
                    FullAdd(newTree)
            | BT(c, a, curKey, curVal, b) -> 
                match b with
                | BE ->
                    if curVal.Length + str.Length <= MaxBufferLength
                    then 
                        let newTree = BT(c, a, curKey, curVal + str, b)
=======
                    let newTree = T(1, E, 0, str, E)
                    PartialAdd(newTree, 0, nextIndex)
                else 
                    let newTree = T(1, E, 0, str, E)
                    FullAdd(newTree)
            | T(c, a, curKey, curVal, b) -> 
                match b with
                | E ->
                    if curVal.Length + str.Length <= MaxBufferLength
                    then 
                        let newTree = T(c, a, curKey, curVal + str, b)
>>>>>>> main
                        FullAdd(newTree)
                    elif curVal.Length = MaxBufferLength
                    then BufferWasFull(curKey)
                    elif curVal.Length < MaxBufferLength && curVal.Length + str.Length > MaxBufferLength
                    then 
                        let remainingBufferLength = MaxBufferLength - curVal.Length
                        let fitString = str[0..remainingBufferLength - 1] |> UnicodeString.create
<<<<<<< HEAD
                        let newTree = BT(c, a, curKey, curVal + fitString, b)
                        PartialAdd(newTree, curKey, fitString.Length)
                    else failwith "unexpected Buffer.tryAppend case"
                | BT(_, _, _, _, _) ->
                    match loop b with
                    | FullAdd newRight ->
                        let reconstructTree = BT(c, a, curKey, curVal, newRight)
                        FullAdd(reconstructTree)
                    | PartialAdd(newRight, maxKey, insLength) ->
                        let reconstructTree = BT(c, a, curKey, curVal, newRight)
=======
                        let newTree = T(c, a, curKey, curVal + fitString, b)
                        PartialAdd(newTree, curKey, fitString.Length)
                    else failwith "unexpected Buffer.tryAppend case"
                | T(_, _, _, _, _) ->
                    match loop b with
                    | FullAdd newRight ->
                        let reconstructTree = T(c, a, curKey, curVal, newRight)
                        FullAdd(reconstructTree)
                    | PartialAdd(newRight, maxKey, insLength) ->
                        let reconstructTree = T(c, a, curKey, curVal, newRight)
>>>>>>> main
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

    let append (str: string) buffer =
        let str = UnicodeString.create str
        let tree = add str buffer.Tree
        { Tree = tree; Length = buffer.Length + str.Length }
                
    /// Create a buffer with a string.
    let createWithString str = append str empty

    /// Find the string associated with a particular key.
    let rec private nodeSubstring key startPos endPos = function
<<<<<<< HEAD
        | BE -> ""
        | BT(_, l, curKey, value, r) ->
=======
        | E -> ""
        | T(_, l, curKey, value, r) ->
>>>>>>> main
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
<<<<<<< HEAD
            | BE -> acc
            | BT(_, l, _, v, r) ->
=======
            | E -> acc
            | T(_, l, _, v, r) ->
>>>>>>> main
                (traverse l acc) + v.Length |> traverse r
        traverse buffer.Tree 0

    /// For testing/debugging purposes. 
    /// Performs an in-order travesal of the buffer's tree and adds each node's length to a list.
    /// Returns a list of ints representing the length of each buffer chronologically.
    let lengthAsList buffer =
        let rec traverse tree (accList: int list) =
            match tree with
<<<<<<< HEAD
            | BE -> accList
            | BT(_,l,_,v,r) ->
=======
            | E -> accList
            | T(_,l,_,v,r) ->
>>>>>>> main
                (traverse l accList) @ [v.Length] |> traverse r
        traverse buffer.Tree []

    /// For testing/debugging purposes, returns all text in a buffer through an in-order traversal.
    /// Highly recommended not to use this in an application as it takes O(n) time and is therefore slow.
    /// Instead, use the Buffer.textInSpan method to get a section of text from the buffer.
    let text buffer =
        let rec traverse tree accText =
            match tree with
<<<<<<< HEAD
            | BE -> accText
            | BT(_,l,_,v,r) ->
=======
            | E -> accText
            | T(_,l,_,v,r) ->
>>>>>>> main
                (traverse l accText) + v[0..v.Length] |> traverse r
        traverse buffer.Tree ""

    /// OOP API for substring method for testing, as SpanType is internal to this assembly.
    type BufferType with
        member this.Substring(index, length) = 
            let span = Span.createWithLength index length
            substring span this
