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

    let private skew = function
        | T(lvx, _, T(lvy, _, a, ky, _, b), kx, _, c) when lvx = lvy -> 
            let inner = T(lvy, size b, b, kx, size c, c)
            T(lvx, size a, a, ky, size inner, inner)
        | t -> t

    let private split = function
        | T(lvx, sl1, a, kx, sr1, T(lvy, sl2, b, ky, sr2, T(lvz, sl3, c, kz, sr3, d))) 
            when lvx = lvy && lvy = lvz -> 
                let right = T(lvx, size c, c, kz, size d, d)
                let left = T(lvx, size a, a, kx, size b, b)
                T(lvx + 1, size left, left, ky, size right, right)
        | t -> t

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

    /// O(1): Returns a boolean if tree is empty.
    let isEmpty = function
        | E -> true
        | _ -> false

    /// O(1): Returns an empty AaTree.
    let empty = E

