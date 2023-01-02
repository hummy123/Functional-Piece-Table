namespace PieceTable

open Types
open Piece

module PieceTree =
    let inline private size node = 
        match node with
        | PE -> 0
        | PT(_, sl, l, p, sr, r) -> sl + p.Span.Length + sr

    let inline private sizeLeft node = 
        match node with
        | PE -> 0
        | PT(_, sl, _, _, _, _) -> sl

    let inline private sizeRight node =
        match node with
        | PE -> 0
        | PT(_, _, _, _, sr, _) -> sr
 
    let inline private skew node =
        match node with
        | PT(lvx, _, PT(lvy, _, a, ky, _, b), kx, _, c) when lvx = lvy -> 
            let inner = PT(lvy, size b, b, kx, size c, c)
            PT(lvx, size a, a, ky, size inner, inner)
        | t -> t

    let inline private split node =
        match node with
        | PT(lvx, sizeA, a, kx, sr1, PT(lvy, sizeB, b, ky, sr2, PT(lvz, sizeC, c, kz, sizeD, d))) 
            when lvx = lvy && lvy = lvz -> 
                let right = PT(lvx, sizeC, c, kz, sizeD, d)
                let left = PT(lvx, sizeA, a, kx, sizeB, b)
                PT(lvx + 1, size left, left, ky, size right, right)
        | t -> t

    let private sngl = function
        | PE -> false
        | PT(_, _, _, _, _, PE) -> true
        | PT(lvx, _, _, _, _, PT(lvy, _, _, _, _, _)) -> lvx > lvy

    let private lvl = function
        | PE -> 0
        | PT(lvt, _, _, _, _, _) -> lvt

    let private nlvl = function
        | PT(lvt, _, _, _, _, _) as t -> 
            if sngl t
            then lvt
            else lvt + 1
        | _ -> failwith "unexpected nlvl case"

    let private adjust = function
        | PT(lvt, _, lt, _, _, rt) as t when lvl lt >= lvt - 1 && lvl rt >= (lvt - 1) ->
            t
        | PT(lvt, sl, lt, kt, sr, rt) when lvl rt < lvt - 1 && sngl lt-> 
            skew <| PT(lvt - 1, sl, lt, kt, sr, rt)
        | PT(lvt, sl1, PT(lv1, sl2, a, kl, sr2, PT(lvb, sl3, lb, kb, sr3, rb)), kt, sr1, rt) when lvl rt < lvt - 1 ->
            let left = PT(lv1, sl2, a, kl, sl3, lb)
            let sizeLeft = sl2 + kl.Span.Length + sl3
            let right = PT(lvt - 1, sr3, rb, kt, sr1, rb)
            let sizeRight = sr3 + kt.Span.Length + sr1
            PT(lvb + 1, sizeLeft, left, kb, sizeRight, right)
        | PT(lvt, sl, lt, kt, sr, rt) when lvl rt < lvt ->
            split <| PT(lvt - 1, sl, lt, kt, sr, rt)
        | PT(lvt, slt, lt, kt, sr1, PT(lvr, _, PT(lva, sizeC, c, ka, sizeD, d), kr, sizeB, b)) -> 
            let a = PT(lva, sizeC, c, ka, sizeD, d)
            let left = PT(lvt - 1, slt, lt, kt, sizeC, c)
            let sizeLeft = slt + kt.Span.Length + sizeC
            let right = split <| PT(nlvl a, sizeD, d, kr, sizeB, b)
            let sizeRight = size right
            PT(lva + 1, sizeLeft, left, ka, sizeRight, right)
        | _ -> failwith "unexpected adjust case"

    let rec private splitMax = function
        | PT(_, _, l, v, _, PE) -> (l, v)
        | PT(h, _, l, v, _, r) as node ->
            match splitMax r with
            | l, b -> adjust <| node, b
        | _ -> failwith "unexpected splitMax case"

    let inline private pieceLength node = 
        match node with
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
            let text = (Buffer.substring piece.Span.Start piece.Span.Length table.Buffer)
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
            | PT(h, sl, l, v, sr, r) ->
                let nodeEndIndex = curIndex + v.Span.Length
                if insIndex > nodeEndIndex then 
                    let newSr = sr + piece.Span.Length
                    let nextIndex = nodeEndIndex + sizeLeft r
                    split <| (skew <| PT(h, sl, l, v, newSr, ins nextIndex r))
                elif insIndex < curIndex then
                    let newSl = sl + piece.Span.Length
                    let nextIndex = curIndex - pieceLength l - sizeRight l
                    split <| (skew <| PT(h, newSl, ins nextIndex l, v, sr, r))
                elif curIndex = insIndex then
                    let newLeft = insMax piece l
                    split <| (skew <| PT(h, sl + piece.Span.Length, newLeft, v, sr, r))
                elif insIndex = nodeEndIndex then
                    let newPiece = Piece.merge v piece
                    PT(h, sl, l, newPiece, sr, r)
                else
                    // We are in range.
                    let (p1, p3) = Piece.split v (insIndex - curIndex)
                    let newLeft = insMax p1 l
                    let newRight = insMin p3 r
                    split <| (skew <| PT(h, sl + p1.Span.Length, newLeft, piece, sr + p3.Span.Length, newRight))

        ins (sizeLeft tree) tree

    (* Repeated if-statements used in both delete and substring. *)
    let inline private inRange start curIndex finish nodeEndIndex =
        start <= curIndex && finish >= nodeEndIndex

    let inline private startIsInRange start curIndex finish nodeEndIndex =
        start <= curIndex && finish < nodeEndIndex && curIndex < finish

    let inline private endIsInRange start curIndex finish nodeEndIndex =
        start > curIndex && finish >= nodeEndIndex && start <= nodeEndIndex

    let inline private middleIsInRange start curIndex finish nodeEndIndex =
        start >= curIndex && finish <= nodeEndIndex

    let substring (start: int) (length: int) table =
        let finish = start + length
        let rec sub curIndex node acc =
            match node with
            | PE -> acc
            | PT(_, _, l, v, _, r) ->
                let left =
                    if start < curIndex
                    then sub (curIndex - pieceLength l - sizeRight l) l acc
                    else acc

                let nodeEndIndex = curIndex + v.Span.Length
                let middle = 
                    if inRange start curIndex finish nodeEndIndex then
                        left + Piece.text v table
                    elif startIsInRange start curIndex finish nodeEndIndex then
                        left + Piece.textAtStart curIndex finish v table
                    elif endIsInRange start curIndex finish nodeEndIndex then
                        left + Piece.textAtEnd curIndex start v table
                    elif middleIsInRange start curIndex finish nodeEndIndex then
                        left + Piece.textInRange curIndex start finish v table
                    else
                        left

                if finish > nodeEndIndex
                then sub (nodeEndIndex + sizeLeft r) r middle
                else middle

        sub (sizeLeft table.Pieces) table.Pieces ""

    let delete (start: int) (length: int) (tree: AaTree): AaTree =
        let finish: int = start + length
        let rec del (curIndex: int) (node: AaTree) =
            match node: AaTree with
            | PE -> PE
            | PT(h, _, l, v, _, r) ->
                let left =
                    if start < curIndex
                    then del (curIndex - pieceLength l - sizeRight l) l
                    else l
                let nodeEndIndex: int = curIndex + v.Span.Length
                let right =
                    if finish > nodeEndIndex
                    then del (nodeEndIndex + sizeLeft r) r
                    else r
                
                if inRange start curIndex finish nodeEndIndex then
                    if left = PE
                    then right
                    else 
                        let (newLeft, newVal) = splitMax left
                        adjust <| PT(h, size newLeft, newLeft, newVal, size right, right)
                elif startIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtStart curIndex finish v
                    split <| (skew <| PT(h, size left, left, newPiece, size right, right))
                elif endIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtEnd curIndex start v
                    PT(h, size left, left, newPiece, size right, right)
                elif middleIsInRange start curIndex finish nodeEndIndex then
                    let (p1, p2) = Piece.deleteInRange curIndex start finish v
                    let newLeft = insMax p1 left
                    split <| (skew <| PT(h, size newLeft, newLeft, p2, size right, right))
                else
                    split <| (skew <| PT(h, size left, left, v, size right, right))
                
        del (sizeLeft tree) tree 
    
