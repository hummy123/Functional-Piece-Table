namespace PieceTable

open Types

module Tree =
    type SizeLeft = int
    type SizeRight = int
    type Value = PieceType

    type Colour = R | B

    type Tree = 
        | E 
        | T of Colour * SizeLeft * Tree * Value * SizeRight * Tree

    let empty = E

    let rec size = function
        | E -> 0
        | T(_, sizeLeft,_,piece,sizeRight,_) -> sizeLeft + sizeRight + piece.Span.Length

    let make l v r =
        let sizeLeft = size l
        let sizeRight = size r
        T(B, sizeLeft, l, v, sizeRight, r)

    let balance = function                              (* Red nodes in relation to black root *)
        | B, T(R, _, T(R, _, a, x, _, b), y, _, c), z, d            (* Left, left *)
        | B, T(R, _, a, x, _, T(R, _, b, y, _, c)), z, d            (* Left, right *)
        | B, a, x, T(R, _, T(R, _, b, y, _, c), z, _, d)            (* Right, left *)
        | B, a, x, T(R, _, b, y, _, T(R, _, c, z, _, d))            (* Right, right *)
            ->
                let left = make a x b
                let right = make c z d
                T(R, size left, left, y, size right, right)
        | c, l, x, r -> T(c, size l, l, x, size r, r)

    let insert item tree = 
        let rec ins = function
            | E -> T(R, 0, E, item, 0, E)
            | T(c, _, a, y, _, b) as node ->
                if item = y then node
                elif item < y then balance(c, ins a, y, b)
                else balance(c, a, y, ins b)

        match ins tree with
        | E -> failwith "should never return empty node from an insert"
        (* Force root node too be black. *)
        | T(_,sizeL, l, x, sizeR, r) -> T(B, sizeL, l, x, sizeR, r)

    let rec print (spaces : int) = function
        | E -> ()
        | T(c, _, l, x, _, r) ->
            print (spaces + 4) r
            printfn "%s %A%A" (new System.String(' ', spaces)) c x
            print (spaces + 4) l
    