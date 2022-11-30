namespace PieceTable

(* Inspired by Chris Okasaki's Red Black Tree design and F# implementation at https://en.wikibooks.org/wiki/F_Sharp_Programming/Advanced_Data_Structures#Binary_Search_Trees *)

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


    // Keep traversing to right.
    // For the last Tree (not empty) case, check:
    // If buffer is equal to max length (insert new buffer or buffers if string is greater than max length).
    // If buffer + str.Length is greater than ma length (same as above).
    // If none of the above, append str to buffer.
    let append (str: string) tree = 
        let rec loop node =
            match node with
            | T(col, left, key, value, right) ->
                match right with
                | E -> 
                    if value.Length + str.Length <= MaxBufferLength
                    then T(col, left, key, value + str, right)
                    elif value.Length < MaxBufferLength
                    then E (* Split string and add part to this buffer, part to another buffer. *)
                    elif str.Length >= MaxBufferLength
                    then E (* Insert 65535 length string chunks into the tree, splitting the current string. *)
                    else insert (key + 1) value tree
                | T(_,_,_,_,_) as nestedNode -> loop nestedNode
            | E -> 
                if str.Length > MaxBufferLength
                then E (* Split string into multiple pieces and insert. *)
                else insert 0 str tree
        loop tree

    // Create function should come after insert as we would otherwise need to code same logic twice
   (* let create str: Tree =
        if str.Length <= MaxBufferLength then
            T(B, E, 0, str, E)
        else
            *)