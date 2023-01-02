namespace PieceTable

open Types
open Piece
open Node

module PieceTree =
    let inline private size node = 
        match node with
        | PE -> 0
        | PT(h, l, n, r) -> n.LeftSize + n.RightSize + n.Length

    let inline private sizeLeft node = 
        match node with
        | PE -> 0
        | PT(_, _, n, _, _) -> n.LeftSize

    let inline private sizeRight node =
        match node with
        | PE -> 0
        | PT(_, _ , n, _, _) -> n.RightSize
 
    let inline private skew node =
        match node with
        | PT(lvx, PT(lvy, a, ky, b), kx, c) when lvx = lvy ->
            let innerIdx = Index.setLeft ky.RightSize kx.Index
            let inner = PT(lvy, b, kx.SetIndex innerIdx, c)
            let outerIdx = Index.setRight (size inner) ky.Index
            PT(lvx, a, ky.SetIndex outerIdx, inner)
        | t -> t

    let inline private split node =
        match node with
        | PT(lvx, a, kx, PT(lvy, b, ky, PT(lvz, c, kz, d))) 
            when lvx = lvy && lvy = lvz -> 
                let right = PT(lvz, c, kz, d)
                let leftIdx = Index.setRight ky.LeftSize kx.Index
                let left = PT(lvx, a, kx.SetIndex leftIdx, b)
                let leftSize = Index.size leftIdx + kx.Length
                let rightSize = Index.size kz.Index + kz.Length
                let newIdx = Index.create leftSize rightSize
                PT(lvx + 1, left, ky, right)
        | t -> t

    let private sngl = function
        | PE -> false
        | PT(_, _, _, PE) -> true
        | PT(lvx, _, _, PT(lvy, _, _, _)) -> lvx > lvy

    let private lvl = function
        | PE -> 0
        | PT(lvt, _, _, _) -> lvt

    let private nlvl = function
        | PT(lvt, _, _, _) as t -> 
            if sngl t
            then lvt
            else lvt + 1
        | _ -> failwith "unexpected nlvl case"

    let private adjust = function
        | PT(lvt, lt, kt, rt) as t when lvl lt >= lvt - 1 && lvl rt >= (lvt - 1) 
            t
        | PT(lvt, lt, kt, rt) when lvl rt < lvt - 1 && sngl lt -> 
            skew <| PT(lvt - 1, lt, kt, rt)
        | PT(lvt, PT(lv1, a, kl, PT(lvb, lb, kb, rb)), kt, rt) when lvl rt < lvt - 1 ->
            let leftIndex = Index.setRight kb.LeftSize kl.Index
            let left = PT(a, kl.SetIndex leftIndex, lb, lv1)
            let sizeLeft = size left
            let rightIndex = Index.setLeft kb.RightSize kt.Index
            let right = PT(lvt - 1, rb, kt.SetIndex rightIndex, rt)
            let sizeRight = size right
            let outerIndex = Index.create sizeLeft sizeRight
            PT(left, kb.SetIndex outerIndex, right, lvb + 1)
        | PT(lvt, lt, kt, rt) when lvl rt < lvt ->
            split <| PT(lvt - 1, lt, kt, rt)
        | PT(lvt, lt, kt, PT(lvr, PT(lva, c, ka, d), kr, b)) -> 
            let indexLeft = Index.setRight kr.LeftSize kt.Index
            let left = PT(lt, kt.SetIndex indexLeft, c, lvt - 1)
            let sizeLeft = size left
            let indexRight = Index.setLeft ka.RightSize kr.Index
            let right = split <| PT(d, kr.SetIndex indexRight, b, nlvl a)
            let sizeRight = size right
            let outerIndex = Index.create sizeLeft sizeRight
            PT(left, ka.SetIndex outerIndex, right, lva + 1)
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
            split <| (skew <| PT(l, idx, v, insMax piece r, h))

    let rec private foldOpt (f: OptimizedClosures.FSharpFunc<_,_,_>) x t =
        match t with
        | PE -> x
        | PT(l, _, v, r, _) ->
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
            | PT(l, index, v, r, h) ->
                let nodeEndIndex = curIndex + v.Span.Length
                if insIndex > nodeEndIndex then 
                    let newIndex = Index.plusRight piece.Span.Length index
                    let nextIndex = nodeEndIndex + sizeLeft r
                    split <| (skew <| PT(l, newIndex, v, ins nextIndex r, h))
                elif insIndex < curIndex then
                    let newIndex = Index.plusLeft piece.Span.Length index
                    let nextIndex = curIndex - pieceLength l - sizeRight l
                    split <| (skew <| PT(ins nextIndex l, newIndex, v, r, h))
                elif curIndex = insIndex then
                    let newIndex = Index.plusLeft piece.Span.Length index
                    split <| (skew <| PT(insMax piece l, newIndex, v, r, h))
                elif insIndex = nodeEndIndex then
                    let newPiece = Piece.merge v piece
                    PT(l, index, newPiece, r, h)
                else
                    // We are in range.
                    let (p1, p3) = Piece.split v (insIndex - curIndex)
                    let newLeft = insMax p1 l
                    let newRight = insMin p3 r
                    let newIndex = Index.create (size newLeft) (size newRight)
                    split <| (skew <| PT(newLeft, newIndex, piece, newRight, h))

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
            | PT(l, idx, v, r, h) ->
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
            | PT(l, idx, v, r, h) as node ->
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
                        let idx = Index.setLeft (size newLeft) idx
                        adjust <| PT(newLeft, idx, newVal, right, h)
                elif startIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtStart curIndex finish v
                    split <| (skew <| PT(l, idx, newPiece, right, h))
                elif endIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtEnd curIndex start v
                    split <| (skew <| PT(l, idx, newPiece, right, h))
                elif middleIsInRange start curIndex finish nodeEndIndex then
                    let (p1, p2) = Piece.deleteInRange curIndex start finish v
                    let newLeft = insMax p1 left
                    let idx = Index.setLeft (idx.LeftSize + p1.Span.Length) idx
                    split <| (skew <| PT(l, idx, p2, right, h))
                else
                    split <| (skew <| node)
                
        del (sizeLeft tree) tree 
    
