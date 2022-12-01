namespace PieceTable

open Types

(* Inspired by Chris Okasaki's Red Black Tree design and F# implementation at 
 * https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Binary_Search_Trees *)

module Buffer =
    [<Struct>]
    type Colour = R | B
    type private Key = int (* The node's index (0, 1, 2, 3, etc.). *)
    type private Value = string
    type Tree = E | T of Colour * Tree * Key * Value * Tree

    [<Literal>]
    (* The maximum number of characters permitted in a buffer, before we create a new one. *)
    let private MaxBufferLength = 65535

    let empty = E

    let balance = function                                          (* Red nodes in relation to black root *)
        | B, T(R, T(R, a, x, s1, b), y, s2, c), z, s3, d            (* Left, left *)
        | B, T(R, a, x, s1, T(R, b, y, s2, c)), z, s3, d            (* Left, right *)
        | B, a, x, s1, T(R, T(R, b, y, s2, c), z, s3, d)            (* Right, left *)
        | B, a, x, s1, T(R, b, y, s2, T(R, c, z, s3, d))            (* Right, right *)
            -> T(R, T(B, a, x, s1, b), y, s2, T(B, c, z, s3, d))
        | c, l, x, s1, r -> T(c, l, x, s1, r)

    let insert key value tree: Tree =
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

    type private AppendType =
        | AppendedToNode of Tree
        | AddedNewNode of Tree
        | Partial of Tree * Key * Value

    // Keep traversing to right.
    // For the last Tree (not empty) case, check:
    // If buffer is equal to max length (insert new buffer or buffers if string is greater than max length).
    // If buffer + str.Length is greater than max length (same as above).
    // If none of the above, append str to buffer.
    let append (str: string) tree = 
        let rec loop node =
            match node with
            | T(col, left, key, value, right) ->
                match right with
                | E -> 
                    if value.Length + str.Length <= MaxBufferLength
                    then 
                        let lastNode = T(col, left, key, value + str, right)
                        AppendedToNode(lastNode)
                    elif value.Length = MaxBufferLength
                    then 
                        let tree = insertLongString str key tree
                        AddedNewNode(tree)
                    elif value.Length < MaxBufferLength && value.Length + str.Length > MaxBufferLength
                    then 
                        let remainingBufferLength = MaxBufferLength - value.Length
                        let startStr = str[0..remainingBufferLength - 1]
                        let lastNode = T(col, left, key, value + startStr, right)
                        let endStr = str[remainingBufferLength..]
                        Partial(lastNode, key, endStr)
                    else 
                        let tree = insertLongString str key tree
                        AddedNewNode(tree)
                | T(c,l,k,v,_) as nestedNode -> 
                    match loop nestedNode with
                    | AppendedToNode r ->
                        AppendedToNode(T(c, l, k, v, r))
                    | AddedNewNode t -> AddedNewNode(t)
                    | Partial(r, lastKey, remainingStr) ->
                        let withAppended = T(c,l,k,v,r)
                        Partial(withAppended, lastKey, remainingStr)
            (* The below empty case should only ever match if the tree root is empty. *)
            | E -> 
                let newTree = insertLongString str 0 tree
                AddedNewNode(newTree)

        match loop tree with
        | AppendedToNode t -> t
        | AddedNewNode t -> t
        | Partial(t, key, remainString) ->
            insertLongString remainString key t

    let createWithString str = insertLongString str -1 E

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

    let textInSpan (span: SpanType) buffer