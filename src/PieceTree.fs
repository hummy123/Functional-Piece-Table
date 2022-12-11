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

    /// O(1): Returns a boolean if tree is empty.
    let isEmpty = function
        | E -> true
        | _ -> false

    /// O(1): Returns an empty AaTree.
    let empty = E

