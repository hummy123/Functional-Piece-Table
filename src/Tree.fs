namespace PieceTable

open Types

module Tree =
    let empty = E

    let length = function
        | E -> 0
        |T(_, _, _, _, _, length) -> length

    let size = function
        | E -> 0
        | T(sizeLeft,_,piece,sizeRight,_, _) -> sizeLeft + sizeRight + piece.Span.Length

    let sizeLeft = function
        | E -> 0
        | T(sizeLeft, _, _, _, _, _) -> sizeLeft

    let isEmpty = function
        | E -> true
        | T(_,_,_,_,_,_) -> false

    let findCtxm insIndex tree =
        let rec loop (curIndex: int) (ctx: Ctx) this =
            match this with
            | E -> ctx, this
            | T(sizeL, left, p, sizeR, right,_) ->
                (* If in range *)
                if insIndex >= curIndex && insIndex <= curIndex + p.Span.Length then
                    ctx, this
                (* If after *)
                elif insIndex < curIndex then
                    loop (curIndex - p.Span.Length) (Fst(ctx, sizeL, p, right)) left
                (* If before *)
                else
                    loop (curIndex + p.Span.Length) (Snd(left, p, sizeR, ctx)) right

        loop (sizeLeft tree) Top tree


    let rec text (table: TextTableType) tree (acc: string) =
        match tree with
        | E -> acc
        | T(_, l, x, _, r, _) ->
            (text table l acc) + (Piece.text x table) 
            |> text table r