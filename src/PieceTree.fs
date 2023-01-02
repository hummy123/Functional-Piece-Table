﻿namespace PieceTable

open Types
open Piece

module PieceTree =
    let inline private size node = 
        match node with
        | PE -> 0
        | PT(l, index, p, r, _) -> index.LeftSize + index.RightSize + p.Span.Length

    let inline private sizeLeft node = 
        match node with
        | PE -> 0
        | PT(_, index, _, _, _) -> index.LeftSize

    let inline private sizeRight node =
        match node with
        | PE -> 0
        | PT(_, index, _, _, _) -> index.RightSize
 
    let inline private skew node =
        match node with
        | PT(PT(a, idx2, ky, b, lvy), idx1, kx, c, lvx) when lvx = lvy -> 
            let innerIdx = Index.create idx2.RightSize idx1.RightSize
            let inner = PT(b, innerIdx, kx, c, lvy)
            let outerIdx = Index.changeRight (size inner) idx2
            PT(a, outerIdx, ky, inner, lvx)
        | t -> t

    let inline private split node =
        match node with
        | PT(a, idx1, kx, PT(b, idx2, ky, PT(c, idx3, kz, d, lvz), lvy), lvx) 
            when lvx = lvy && lvy = lvz -> 
                let right = PT(c, idx3, kz, d, lvx)
                let leftIdx = Index.changeRight idx2.LeftSize idx1
                let left = PT(a, leftIdx, kx, b, lvx)
                let leftSize = Index.size leftIdx + kx.Span.Length
                let rightSize = Index.size idx3 + kz.Span.Length
                let newIdx = Index.create leftSize rightSize
                PT(left, newIdx, ky, right, lvx + 1)
        | t -> t

    let private sngl = function
        | PE -> false
        | PT(_, _, _, PE, _) -> true
        | PT(_, _, _, PT(_, _, _, _, lvy), lvx) -> lvx > lvy

    let private lvl = function
        | PE -> 0
        | PT(_, _, _, _, lvt) -> lvt

    let private nlvl = function
        | PT(_, _, _, _, lvt) as t -> 
            if sngl t
            then lvt
            else lvt + 1
        | _ -> failwith "unexpected nlvl case"

    let private adjust = function
        | PT(lt, _, _, rt, lvt) as t when lvl lt >= lvt - 1 && lvl rt >= (lvt - 1) ->
            t
        | PT(lt, idx, kt, rt, lvt) when lvl rt < lvt - 1 && sngl lt-> 
            skew <| PT(lt, idx, kt, rt, lvt - 1)
        | PT(PT(a, idx2, kl, PT(lb, idx3, kb, rb, lvb), lv1), idx1, kt, rt, lvt) when lvl rt < lvt - 1 ->
            let leftIndex = Index.changeRight idx3.LeftSize idx2
            let left = PT(a, leftIndex, kl, lb, lv1)
            let sizeLeft = size left
            let rightIndex = Index.create idx3.RightSize idx1.RightSize
            let right = PT(rb, rightIndex, kt, rt, lvt - 1)
            let sizeRight = size right
            let outerIndex = Index.create sizeLeft sizeRight
            PT(left, outerIndex, kb, right, lvb + 1)
        | PT(lt, idx, kt, rt, lvt) when lvl rt < lvt ->
            split <| PT(lt, idx, kt, rt, lvt - 1)
        | PT(lt, idx1, kt, PT((PT(c, idx3, ka, d, lva) as a), idx2, kr, b, lvr), lvt) -> 
            let indexLeft = Index.changeRight idx3.LeftSize idx1
            let left = PT(lt, indexLeft, kt, c, lvt - 1)
            let sizeLeft = size left
            let indexRight = Index.changeLeft idx3.RightSize idx2
            let right = split <| PT(d, indexRight, kr, b, nlvl a)
            let sizeRight = size right
            let outerIndex = Index.create sizeLeft sizeRight
            PT(left, outerIndex, ka, right, lva + 1)
        | _ -> failwith "unexpected adjust case"

    let rec private splitMax = function
        | PT(l, _, v, PE, _) -> (l, v)
        | PT(l, _, v, r, h) as node ->
            match splitMax r with
            | l, b -> adjust <| node, b
        | _ -> failwith "unexpected splitMax case"

    let inline private pieceLength node = 
        match node with
        | PE -> 0
        | PT(_, _, piece, _, _) -> piece.Span.Length

    let rec private insMin piece node =
        match node with
        | PE -> PT(PE, Index.empty, piece, PE, 1)
        | PT(l, idx,  v, r, h) ->
            let idx = Index.plusLeft piece.Span.Length idx
            split <| (skew <| PT(insMin piece l, idx, v, r, h))

    let rec private insMax piece node =
        match node with
        | PE -> PT(PE, Index.empty, piece, PE, 1)
        | PT(l, idx,  v, r, h) ->
            let idx = Index.plusRight piece.Span.Length idx
            split <| (skew <| PT(l, idx, v, insMin piece r, h))

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
            | PE -> PT(PE, Index.empty, piece, PE, 1)
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
    
