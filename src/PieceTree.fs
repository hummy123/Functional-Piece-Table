namespace PieceTable

open Types
open Piece

module PieceTree =
    let private sngl = function
        | E -> false
        | T(_, _, _, _, _, E) -> true
        | T(lvx,_, _, _, _, T(lvy, _, _, _, _, _)) -> lvx > lvy

    let private lvl = function
        | E -> 0
        | T(lvt, _, _, _, _, _) -> lvt

    let private nlvl = function
        | T(lvt, _, _, _, _, _) as t -> 
            if sngl t
            then (lvt - 1)
            else lvt
        | _ -> failwith "unexpected nlvl case"

    let private size = function
        | E -> 0
        | T(_, sl, _, p, sr, _) -> sl + p.Span.Length + sr

    let private sizeLeft = function
        | E -> 0
        | T(_, sl, _, _, _, _) -> sl
 
    let private skew = function
        | T(lvx, _, T(lvy, _, a, ky, _, b), kx, _, c) when lvx = lvy -> 
            let inner = T(lvy, size b, b, kx, size c, c)
            T(lvx, size a, a, ky, size inner, inner)
        | t -> t

    let private split = function
        | T(lvx, sizeA, a, kx, sr1, T(lvy, sizeB, b, ky, sr2, T(lvz, sizeC, c, kz, sizeD, d))) 
            when lvx = lvy && lvy = lvz -> 
                let right = T(lvx, sizeC, c, kz, sizeD, d)
                let left = T(lvx, sizeA, a, kx, sizeB, b)
                T(lvx + 1, size left, left, ky, size right, right)
        | t -> t
            
    let rec private add item = function
        | E -> T(1, 0, E, item, 0, E)
        | T(h, sl, l, v, sr, r) as node ->
            if item < v
            then
                let newLeft = add item l
                split <| (skew <| T(h, size newLeft, newLeft, v, sr, r))
            elif item > v
            then 
                let newRight = add item r
                split <| (skew <| T(h, sl, l, v, size newRight, newRight))
            else node

    let rec private dellrg = function
        | T(_, _, l, v, _, E) -> (l, v)
        | T(h, _, l, v, sr, r) ->
            let (newLeft, newVal) = dellrg l
            T(h, size newLeft, newLeft, v, sr, r), newVal
        | _ -> failwith "unexpected dellrg case"

    let private adjust = function
        | T(lvt, _, lt, kt, _, rt) as t when lvl lt >= lvt - 1 && lvl rt >= (lvt - 1) 
            -> t
        | T(lvt, sl, lt, kt, sr, rt) when lvl rt < lvt - 1 && sngl lt-> 
            skew <| T(lvt - 1, sl, lt, kt, sr, rt)
        | T(lvt, _, T(lv1, sizeA, a, kl, _, T(lvb, sizeLb, lb, kb, sizeRb, rb)), kt, sizeRt, rt) 
            when lvl rt < lvt - 1 -> 
                let left = T(lv1, sizeA, a, kl, sizeLb, lb)
                let right = T(lvt - 1, sizeRb, rb, kt, sizeRt, rt)
                T(lvb + 1, size left, left, kb, size right, right)
        | T(lvt, sl, lt, kt, sr, rt) when lvl rt < lvt -> 
            split <| T(lvt - 1, sl, lt, kt, sr, rt)
        | T(lvt, sizeLt, lt, kt, sr1, T(lvr, sl2, T(lva, sizeC, c, ka, sizeD, d), kr, sizeB, b)) -> 
            let a = T(lva, sizeC, c, ka, sizeD, d)
            let right = split <| T(nlvl a, sizeD, d, kr, sizeB, b)
            let left = T(lvt-1, sizeLt, lt, kt, sizeC, c)
            T(lva + 1, size left, left,  ka, size right, right)
        | _ -> failwith "unexpected adjust case"

    let rec remove item = function
        | E -> E
        | T(_, _, E, v, _, rt) when item = v -> rt
        | T(_, _, lt, v, _, E) when item = v -> lt
        | T(h, sl, l, v, sr, r) as node ->
            if item < v then
                let newLeft = remove item l
                adjust <| T(h, size newLeft, newLeft, v, sr, r)
            elif item > v then 
                let newRight = remove item r
                T(h, sl, l, v, size newRight, newRight)
            else 
                let (newLeft, newVal) = dellrg l
                T(h, size newLeft, newLeft, newVal, sr, r)

    let rec private foldOpt (f: OptimizedClosures.FSharpFunc<_,_,_>) x t =
        match t with
        | E -> x
        | T(_, _, l, v, _, r) ->
            let x = foldOpt f x l
            let x = f.Invoke(x,v)
            foldOpt f x r

    /// Executes a function on each element in order (for example: 1, 2, 3 or a, b, c).
    let fold f x t = foldOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) x t

    /// O(1): Returns a boolean if tree is empty.
    let isEmpty = function
        | E -> true
        | _ -> false

    /// O(1): Returns an empty AaTree.
    let empty = E

    let rec insert insIndex piece tree =
        let rec ins curIndex node =
            match node with
            | E -> T(1, 0, E, piece, 0, E)
            | T(h, sl, l, v, sr, r) as node ->
                let nodeEndIndex = curIndex + v.Span.Length
                if curIndex < insIndex then 
                    let newSr = sr + v.Span.Length
                    let nextIndex = curIndex + v.Span.Length
                    T(h, sl, l, v, newSr, ins nextIndex r)
                elif curIndex > nodeEndIndex then
                    let newSl = sl + v.Span.Length
                    let nextIndex = curIndex - v.Span.Length
                    T(h, newSl, ins nextIndex l, v, sr, r)
                elif curIndex = insIndex then
                    // How do I move the current piece to the right in a balanced way?
                    node
                elif curIndex = nodeEndIndex then
                    // How do I move the insertion piece to the right in a balanced way?
                    node
                else
                    // We are in range: what now?
                    node

        ins (sizeLeft tree) tree