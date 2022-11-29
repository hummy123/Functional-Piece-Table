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

    let isEmpty = function
        | E -> true
        | T(_,_,_,_,_,_) -> false

    (*
    let rec text (table: TextTableType) tree (acc: string) =
        match tree with
        | E -> acc
        | T(_, l, x, _, r, _) ->
            (text table l acc) + (Piece.text x table) 
            |> text table r
            *)