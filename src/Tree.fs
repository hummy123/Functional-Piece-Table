namespace PieceTable

open Types

module Tree =
    let empty = E

    let size = function
        | E -> 0
        | T(_, sizeLeft,_,piece,sizeRight,_) -> sizeLeft + sizeRight + piece.Span.Length

    let sizeLeft = function
        | E -> 0
        | T(_, sizeLeft, _, _, _, _) -> sizeLeft

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

    let insert insIndex item (tree: Tree) = 
        let rec ins curIndex = function
            | E -> T(R, 0, E, item, 0, E)
            | T(c, _, a, y, _, b) as node ->
                (* If we are at the start of the node we want to insert at. *)
                if insIndex = curIndex then 
                    balance(R, a, item, b)
                (* If we are in range of node we want to insert at. *)
                elif insIndex >= curIndex && insIndex <= curIndex + y.Span.Length then
                    let (p1, p2, p3) = Piece.split y item (insIndex - curIndex)
                    balance(R, (make a p1 E), p2, (make E p3 b))
                (* If we are after the index we want to insert into. *)
                elif insIndex < curIndex 
                then balance(c, ins (curIndex - y.Span.Length) a, y, b)
                (* If we are before the index we want to insert into. *)
                else balance(c, a, y, ins (curIndex + y.Span.Length) b)

        match ins (sizeLeft tree) tree with
        | E -> failwith "should never return empty node from an insert"
        (* Force root node too be black. *)
        | T(_,sizeL, l, x, sizeR, r) -> T(B, sizeL, l, x, sizeR, r)

    let rec print (table: TextTableType) = function
        | E -> ()
        | T(c, _, l, x, _, r) ->
            print table r
            printfn "%s" <| Piece.text x table
            print table l
    