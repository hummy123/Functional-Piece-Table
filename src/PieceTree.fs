namespace PieceTable

open Types
open Piece

module PieceTree =
    let private sngl = function
        | PE -> false
        | PT(_, _, _, _, _, PE) -> true
        | PT(lvx,_, _, _, _, PT(lvy, _, _, _, _, _)) -> lvx > lvy

    let private lvl = function
        | PE -> 0
        | PT(lvt, _, _, _, _, _) -> lvt

    let private nlvl = function
        | PT(lvt, _, _, _, _, _) as t -> 
            if sngl t
            then (lvt - 1)
            else lvt
        | _ -> failwith "unexpected nlvl case"

    let private size = function
        | PE -> 0
        | PT(_, sl, _, p, sr, _) -> sl + p.Span.Length + sr

    let private sizeLeft = function
        | PE -> 0
        | PT(_, sl, _, _, _, _) -> sl
 
    let private skew = function
        | PT(lvx, _, PT(lvy, _, a, ky, _, b), kx, _, c) when lvx = lvy -> 
            let inner = PT(lvy, size b, b, kx, size c, c)
            PT(lvx, size a, a, ky, size inner, inner)
        | t -> t

    let private split = function
        | PT(lvx, sizeA, a, kx, sr1, PT(lvy, sizeB, b, ky, sr2, PT(lvz, sizeC, c, kz, sizeD, d))) 
            when lvx = lvy && lvy = lvz -> 
                let right = PT(lvx, sizeC, c, kz, sizeD, d)
                let left = PT(lvx, sizeA, a, kx, sizeB, b)
                PT(lvx + 1, size left, left, ky, size right, right)
        | t -> t
            
    let rec private add item = function
        | PE -> PT(1, 0, PE, item, 0, PE)
        | PT(h, sl, l, v, sr, r) as node ->
            if item < v
            then
                let newLeft = add item l
                split <| (skew <| PT(h, size newLeft, newLeft, v, sr, r))
            elif item > v
            then 
                let newRight = add item r
                split <| (skew <| PT(h, sl, l, v, size newRight, newRight))
            else node

    let rec private dellrg = function
        | PT(_, _, l, v, _, PE) -> (l, v)
        | PT(h, _, l, v, sr, r) ->
            let (newLeft, newVal) = dellrg l
            PT(h, size newLeft, newLeft, v, sr, r), newVal
        | _ -> failwith "unexpected dellrg case"

    let private adjust = function
        | PT(lvt, _, lt, kt, _, rt) as t when lvl lt >= lvt - 1 && lvl rt >= (lvt - 1) 
            -> t
        | PT(lvt, sl, lt, kt, sr, rt) when lvl rt < lvt - 1 && sngl lt-> 
            skew <| PT(lvt - 1, sl, lt, kt, sr, rt)
        | PT(lvt, _, PT(lv1, sizeA, a, kl, _, PT(lvb, sizeLb, lb, kb, sizeRb, rb)), kt, sizeRt, rt) 
            when lvl rt < lvt - 1 -> 
                let left = PT(lv1, sizeA, a, kl, sizeLb, lb)
                let right = PT(lvt - 1, sizeRb, rb, kt, sizeRt, rt)
                PT(lvb + 1, size left, left, kb, size right, right)
        | PT(lvt, sl, lt, kt, sr, rt) when lvl rt < lvt -> 
            split <| PT(lvt - 1, sl, lt, kt, sr, rt)
        | PT(lvt, sizeLt, lt, kt, sr1, PT(lvr, sl2, PT(lva, sizeC, c, ka, sizeD, d), kr, sizeB, b)) -> 
            let a = PT(lva, sizeC, c, ka, sizeD, d)
            let right = split <| PT(nlvl a, sizeD, d, kr, sizeB, b)
            let left = PT(lvt-1, sizeLt, lt, kt, sizeC, c)
            PT(lva + 1, size left, left,  ka, size right, right)
        | _ -> failwith "unexpected adjust case"

    let rec remove item = function
        | PE -> PE
        | PT(_, _, PE, v, _, rt) when item = v -> rt
        | PT(_, _, lt, v, _, PE) when item = v -> lt
        | PT(h, sl, l, v, sr, r) as node ->
            if item < v then
                let newLeft = remove item l
                adjust <| PT(h, size newLeft, newLeft, v, sr, r)
            elif item > v then 
                let newRight = remove item r
                PT(h, sl, l, v, size newRight, newRight)
            else 
                let (newLeft, newVal) = dellrg l
                PT(h, size newLeft, newLeft, newVal, sr, r)

    let private pieceLength = function
        | PE -> 0
        | PT(_,_,_,piece,_,_) -> piece.Span.Length

    let rec private insMin piece node =
        match node with
        | PE -> PT(1, 0, PE, piece, 0, PE)
        | PT(h, sl, l, v, sr, r) ->
            split <| (skew <| PT(h, sl + piece.Span.Length, insMin piece l, v, sr, r))

    let rec private insMax piece node =
        match node with
        | PE -> PT(1, 0, PE, piece, 0, PE)
        | PT(h, sl, l, v, sr, r) ->
            split <| (skew <| PT(h, sl, l, v, sr + piece.Span.Length, insMax piece r))

    let rec private foldOpt (f: OptimizedClosures.FSharpFunc<_,_,_>) x t =
        match t with
        | PE -> x
        | PT(_, _, l, v, _, r) ->
            let x = foldOpt f x l
            let x = f.Invoke(x,v)
            foldOpt f x r

    /// Executes a function on each element in order (for example: 1, 2, 3 or a, b, c).
    let fold f x t = foldOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) x t

    /// Returns the text contained in the PieceTree.
    let text table = 
        let folder = (fun (acc: string) (piece: PieceType) ->
            let text = (Buffer.substring piece.Span table.Buffer)
            acc + text
        )
        fold folder "" table.Pieces

    /// O(1): Returns a boolean if tree is empty.
    let isEmpty = function
        | PE -> true
        | _ -> false

    /// O(1): Returns an empty AaTree.
    let empty = PE

    let insert insIndex piece tree =
        let rec ins curIndex node =
            match node with
            | PE -> PT(1, 0, PE, piece, 0, PE)
            | PT(h, sl, l, v, sr, r) as node ->
                let nodeEndIndex = curIndex + v.Span.Length
                if insIndex > nodeEndIndex then 
                    let newSr = sr + v.Span.Length
                    let nextIndex = curIndex + v.Span.Length
                    split <| (skew <| PT(h, sl, l, v, newSr, ins nextIndex r))
                elif insIndex < curIndex then
                    let newSl = sl + v.Span.Length
                    let nextIndex = curIndex - (pieceLength l)
                    split <| (skew <| PT(h, newSl, ins nextIndex l, v, sr, r))
                elif curIndex = insIndex then
                    let newLeft = insMax piece l
                    split <| (skew <| PT(h, size newLeft, newLeft, v, sr, r))
                elif insIndex = nodeEndIndex then
                    let newPiece = Piece.merge v piece
                    PT(h, sl, l, newPiece, sr, r)
                else
                    // We are in range.
                    let (p1, p3) = Piece.split v (insIndex - curIndex)
                    let newLeft = insMax p1 l
                    let newRight = insMin p3 r
                    split <| (skew <| PT(h, size newLeft, newLeft, piece, size newRight, newRight))

        ins (sizeLeft tree) tree

    let substring (span: SpanType) table =
        let rec sub curIndex node acc =
            match node with
            | PE -> acc
            | PT(h, sl, l, v, sr, r) ->
                let nodeEndIndex = curIndex + v.Span.Length
                let left = 
                    if span.Start < curIndex
                    then sub (curIndex - (pieceLength l)) l acc
                    else acc

                let pos = Piece.compareWithSpan span curIndex v
                let middle =
                    match pos with
                    | GreaterThanSpan
                    | LessThanSpan -> left
                    | _ -> left + (Piece.textSlice pos curIndex v span table)

                if span.Start + span.Length > nodeEndIndex
                then sub (curIndex + v.Span.Length) r middle
                else middle

        sub (sizeLeft table.Pieces) table.Pieces ""

    let delete (span: SpanType) tree =
        let spanEnd = span.Start + span.Length
        let rec del curIndex node =
            match node with
            | PE -> PE
            | PT(h, sl, l, v, sr, r) ->
                let nodeEndIndex = curIndex + v.Span.Length
                let left = 
                    if span.Start < curIndex
                    then del (curIndex - (pieceLength l)) l
                    else l

                let right =
                    if spanEnd > nodeEndIndex
                    then del (curIndex + v.Span.Length) r
                    else r

                let pos = Piece.compareWithSpan span curIndex v
                let middle =
                    match pos with
                    | GreaterThanSpan 
                    | LessThanSpan -> 
                        split <| (skew <| PT(h, size left, left, v, size right, right))
                    | PieceFullyInSpan ->
                        try
                            let (newLeft, newVal) = dellrg l
                            PT(h, size newLeft, newLeft, newVal, size right, right)
                        with
                        | _ ->    
                            let newVal = Piece.create true 0 0
                            PT(h, size left, left, newVal, size right, right)
                    | SpanWithinPiece ->
                        let (p1, p2) = Piece.deleteInRange curIndex span v
                        let newLeft = insMax p1 left
                        split <| (skew <| PT(h, size newLeft, newLeft, p2, size right, right))
                    | StartOfPieceInSpan ->
                        let newPiece = Piece.deleteAtStart curIndex span v
                        split <| (skew <| PT(h, size left, left, newPiece, size right, right))
                    | EndOfPieceInSpan ->
                        let newPiece = Piece.deleteAtEnd curIndex span v
                        PT(h, size left, left, newPiece, size right, right)
                middle
        del (sizeLeft tree) tree 
    
    let rec printPieces = function
        | PE -> ()
        | PT(_, _, l, v, _, r) ->
            printPieces l
            printfn "%A" v
            printPieces r