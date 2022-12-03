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

    let append (str: string) tree: Tree =
        let rec ins (loopKey: int) strIndex = function
            | E -> 
                let strRemainLength = str.Length - strIndex
                let strEnd = strIndex + MaxBufferLength
                let strPart = str[strIndex..(strEnd - 1)]

                if strRemainLength <= MaxBufferLength 
                then T(R, E, loopKey, strPart, E)
                else
                    let right = ins (loopKey + 1) strEnd E
                    balance(R, E, loopKey, strPart, right)
            | T(c, a, curKey, curVal, b) ->
                match b with
                | E ->
                    (* If insert value fits in this buffer. *)
                    if curVal.Length + str.Length <= MaxBufferLength
                    then T(c, a, loopKey, curVal + str, b)
                    (* If this buffer is full. *)
                    elif str.Length = MaxBufferLength
                    then balance(c, a, curKey, curVal, ins loopKey strIndex b)
                    (* If part of the insert value fits in this buffer *)
                    elif str.Length < MaxBufferLength && str.Length + str.Length > MaxBufferLength
                        then 
                            let remainingBufferLength = MaxBufferLength - str.Length
                            let fitString = str[0..remainingBufferLength - 1]
                            balance(c, a, curKey, curVal + fitString, ins (curKey + 1) remainingBufferLength b)
                    else failwith "unexpected Buffer.insert case"
                | T(_, _, nextKey, _, _) ->
                    T(c, a, curKey, curVal, ins nextKey 0 b)

        (* Forcing root node to be black *)                
        match ins 0 0 tree with
            | E -> failwith "Should never return empty from an insert"
            | T(_, l, k, v, r) -> T(B, l, k, v, r)

    (* Try to put the apend and insertLongString functions into ins/insert 
     * for correctness and performance. *)

    let createWithString str = append str E

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

    let print buffer =
        let rec traverse tree =
            match tree with
            | E -> ()
            | T(_,l,_,v,r) ->
                traverse l
                printfn "%s" v
                traverse r
        traverse buffer

    /// Gets text in a buffer at a specific span.
    let textInSpan (span: SpanType) buffer = 0