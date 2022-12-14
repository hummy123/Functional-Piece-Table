namespace PieceTable

open Types
open Piece

module PieceTree =
    let inline private size node = 
        match node with
        | PE -> 0
        | PT(l, index, p, r, _, _) -> index.Left + index.Right + p.Length

    let inline private lineSize node =
        match node with
        | PE -> 0
        | PT(l, index, p, r, _, lines) -> lines.Left + lines.Right + p.Lines.Length

    let inline private sizeLeft node = 
        match node with
        | PE -> 0
        | PT(_, index, _, _, _, _) -> index.Left

    let inline private sizeRight node =
        match node with
        | PE -> 0
        | PT(_, index, _, _, _, _) -> index.Right
 
    let inline private skew node =
        match node with
        | PT(PT(a, idx2, ky, b, lvy), idx1, kx, c, lvx) when lvx = lvy -> 
            let innerIdx = Meta.create idx2.RightSize idx1.RightSize
            let inner = PT(b, innerIdx, kx, c, lvy)
            let outerIdx = Meta.setRight (size inner) idx2
            PT(a, outerIdx, ky, inner, lvx)
        | t -> t

    let inline private split node =
        match node with
        | PT(a, idx1, kx, PT(b, idx2, ky, PT(c, idx3, kz, d, lvz), lvy), lvx) 
            when lvx = lvy && lvy = lvz -> 
                let right = PT(c, idx3, kz, d, lvx)
                let leftIdx = Meta.setRight idx2.LeftSize idx1
                let left = PT(a, leftIdx, kx, b, lvx)
                let leftSize = Meta.size leftIdx + kx.Length
                let rightSize = Meta.size idx3 + kz.Length
                let newIdx = Meta.create leftSize rightSize
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
            let leftIndex = Meta.create idx2.LeftSize idx3.LeftSize
            let left = PT(a, leftIndex, kl, lb, lv1)
            let sizeLeft = size left
            let rightIndex = Meta.create idx3.RightSize idx1.RightSize
            let right = PT(rb, rightIndex, kt, rt, lvt - 1)
            let sizeRight = size right
            let outerIndex = Meta.create sizeLeft sizeRight
            PT(left, outerIndex, kb, right, lvb + 1)
        | PT(lt, idx, kt, rt, lvt) when lvl rt < lvt ->
            split <| PT(lt, idx, kt, rt, lvt - 1)
        | PT(lt, idx1, kt, PT((PT(c, idx3, ka, d, lva) as a), idx2, kr, b, lvr), lvt) -> 
            let indexLeft = Meta.setRight idx3.LeftSize idx1
            let left = PT(lt, indexLeft, kt, c, lvt - 1)
            let indexRight = Meta.setLeft idx3.RightSize idx2
            let right = split <| PT(d, indexRight, kr, b, nlvl a)
            let outerIndex = Meta.create (size left) (size right)
            PT(left, outerIndex, ka, right, lva + 1)
        | _ -> failwith "unexpected adjust case"

    let rec private splitMax = function
        | PT(l, _, v, PE, _, _) -> (l, v)
        | PT(l, _, v, r, h, _) as node ->
            let (r', b) = splitMax r
            in adjust <| node, b
        | _ -> failwith "unexpected splitMax case"

    let inline private pieceLength node = 
        match node with
        | PE -> 0
        | PT(_, _, piece, _, _, _) -> piece.Length

    let rec private insMin piece node =
        match node with
        | PE -> PT(PE, Meta.empty, piece, PE, 1, Meta.empty)
        | PT(l, idx,  v, r, h, lines) ->
            let idx = Meta.plusLeft piece.Length idx
            let lns = Meta.plusLeft piece.Lines.Length lines
            split <| (skew <| PT(insMin piece l, idx, v, r, h, lns))

    let rec private insMax (piece: PieceType) node =
        match node with
        | PE -> PT(PE, Meta.empty, piece, PE, 1, Meta.empty)
        | PT(l, idx,  v, r, h, lines) ->
            let idx = Meta.plusRight piece.Length idx
            let lns = Meta.plusRight piece.Lines.Length lines
            split <| (skew <| PT(l, idx, v, insMax piece r, h, lns))

    let rec private foldOpt (f: OptimizedClosures.FSharpFunc<_,_,_>) x t =
        match t with
        | PE -> x
        | PT(l, _, v, r, _, _) ->
            let x = foldOpt f x l
            let x = f.Invoke(x,v)
            foldOpt f x r

    /// Executes a function on each element in order (for example: 1, 2, 3 or a, b, c).
    let fold f x t = foldOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) x t

    /// Returns the text contained in the PieceTree.
    let text table = 
        let folder = (fun (acc: string) (piece: PieceType) ->
            let text = (Buffer.substring piece.Start piece.Length table.Buffer)
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
            | PE -> PT(PE, Meta.empty, piece, PE, 1, Meta.empty)
            | PT(l, index, v, r, h, lines) ->
                let nodeEndIndex = curIndex + v.Length
                if insIndex > nodeEndIndex then 
                    let newIndex = Meta.plusRight piece.Length index
                    let newLines = Meta.plusRight piece.Lines.Length lines
                    let nextIndex = nodeEndIndex + sizeLeft r
                    split <| (skew <| PT(l, newIndex, v, ins nextIndex r, h, newLines))
                elif insIndex < curIndex then
                    let newIndex = Meta.plusLeft piece.Length index
                    let newLines = Meta.plusLeft piece.Lines.Length lines
                    let nextIndex = curIndex - pieceLength l - sizeRight l
                    split <| (skew <| PT(ins nextIndex l, newIndex, v, r, h, newLines))
                elif curIndex = insIndex then
                    let newIndex = Meta.plusLeft piece.Length index
                    let newLines = Meta.plusLeft piece.Lines.Length lines
                    split <| (skew <| PT(insMax piece l, newIndex, v, r, h, newLines))
                elif insIndex = nodeEndIndex then
                    let newPiece = Piece.merge v piece
                    PT(l, index, newPiece, r, h, lines)
                else
                    // We are in range.
                    let (p1, p3) = Piece.split v (insIndex - curIndex)
                    let newLeft = insMax p1 l
                    let newRight = insMin p3 r
                    let newIndex = Meta.create (size newLeft) (size newRight)
                    let newLines = Meta.create (lineSize newLeft) (lineSize newRight) 
                    split <| (skew <| PT(newLeft, newIndex, piece, newRight, h, newLines))

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
            | PT(l, idx, v, r, h, _) ->
                let left =
                    if start < curIndex
                    then sub (curIndex - pieceLength l - sizeRight l) l acc
                    else acc

                let nodeEndIndex = curIndex + v.Length
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
            | PT(l, idx, v, r, h, lines) as node ->
                let left =
                    if start < curIndex
                    then del (curIndex - pieceLength l - sizeRight l) l
                    else l
                let nodeEndIndex: int = curIndex + v.Length
                let right =
                    if finish > nodeEndIndex
                    then del (nodeEndIndex + sizeLeft r) r
                    else r
                
                if inRange start curIndex finish nodeEndIndex then
                    if left = PE
                    then right
                    else 
                        let (newLeft, newVal) = splitMax left
                        let idx = Meta.create (size newLeft) (size right)
                        PT(newLeft, idx, newVal, right, h) |> adjust
                elif startIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtStart curIndex finish v
                    let idx = Meta.create (size left) (size right)
                    PT(left, idx, newPiece, right, h) |> skew |> split
                elif endIsInRange start curIndex finish nodeEndIndex then
                    let newPiece = Piece.deleteAtEnd curIndex start v
                    let idx = Meta.create (size left) (size right)
                    PT(left, idx, newPiece, right, h) |> adjust
                elif middleIsInRange start curIndex finish nodeEndIndex then
                    let (p1, p2) = Piece.deleteInRange curIndex start finish v
                    let newLeft = insMax p1 left
                    let idx = Meta.create (size newLeft) (size right)
                    PT(newLeft, idx, p2, right, h) |> skew |> split
                else
                    let idx = Meta.create (size left) (size right)
                    PT(left, idx, v, right, h) |> adjust
                
        del (sizeLeft tree) tree 
    
