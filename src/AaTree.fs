namespace PieceTable

open Types

(* Insertion and deletion not provided as that is to be implemented 
 * with different algorithms for each node type.
 * Implementation guided by following paper: https://arxiv.org/pdf/1412.4882.pdf *)

module AaTree =
    /// O(1): Returns a boolean if tree is empty.
    let isEmpty = function
        | E -> true
        | _ -> false

    let sngl = function
        | E -> false
        | T(_, _, _, E) -> true
        | T(lvx, _, _, T(lvy, _, _, _)) -> lvx > lvy

    /// O(1): Returns an empty AaTree.
    let empty = E

    let leaf value = T(1, E, value, E)

    let lvl = function
        | E -> 0
        | T(lvt, _, _, _) -> lvt

    let skew = function
        | T(lvx, T(lvy, a, ky, b), kx, c) when lvx = lvy
            -> T(lvx, a, ky, T(lvx, b, kx, c))
        | t -> t

    let split = function
        | T(lvx, a, kx, T(lvy, b, ky, T(lvz, c, kz, d))) 
            when lvx = lvy && lvy = lvz
              -> T(lvx + 1, T(lvx, a, kx, b), ky, T(lvx, c, kz, d))
        | t -> t

    (* nlvl function fixed according to Isabelle HOL prrof below: *)
    (* https://isabelle.in.tum.de/library/HOL/HOL-Data_Structures/AA_Set.html#:~:text=text%E2%80%B9In%20the%20paper%2C%20the%20last%20case%20of%20%5C%3C%5Econst%3E%E2%80%B9adjust%E2%80%BA%20is%20expressed%20with%20the%20help%20of%20an%0Aincorrect%20auxiliary%20function%20%5Ctexttt%7Bnlvl%7D. *)
    let nlvl = function
        | T(lvt, _, _, _) as t -> 
            if sngl t
            then lvt
            else lvt + 1
        | _ -> failwith "unexpected nlvl case"

    let adjust = function
        | T(lvt, lt, kt, rt) as t when lvl lt >= lvt - 1 && lvl rt >= (lvt - 1) ->
            t
        | T(lvt, lt, kt, rt) when lvl rt < lvt - 1 && sngl lt-> 
            skew <| T(lvt - 1, lt, kt, rt)
        | T(lvt, T(lv1, a, kl, T(lvb, lb, kb, rb)), kt, rt) when lvl rt < lvt - 1 ->
            T(lvb + 1, T(lv1, a, kl, lb), kb, T(lvt - 1, rb, kt, rt))
        | T(lvt, lt, kt, rt) when lvl rt < lvt ->
            split <| T(lvt - 1, lt, kt, rt)
        | T(lvt, lt, kt, T(lvr, (T(lva, c, ka, d) as a), kr, b)) -> 
            T(lva + 1, T(lvt - 1, lt, kt, c), ka, (split (T(nlvl a, d, kr, b))))
        | _ -> failwith "unexpected adjust case"

    (* splitMax fixed as in Isabelle HOL proof below: *)
    (* https://isabelle.in.tum.de/library/HOL/HOL-Data_Structures/AA_Set.html#:~:text=Function%20%E2%80%B9split_max%E2%80%BA%20below%20is%20called%20%5Ctexttt%7Bdellrg%7D%20in%20the%20paper.%0AThe%20latter%20is%20incorrect%20for%20two%20reasons%3A%20%5Ctexttt%7Bdellrg%7D%20is%20meant%20to%20delete%20the%20largest%0Aelement%20but%20recurses%20on%20the%20left%20instead%20of%20the%20right%20subtree%3B%20the%20invariant%0Ais%20not%20restored.%E2%80%BA *)
    let rec splitMax = function
        | T(_, l, v, E) -> (l, v)
        | T(h, l, v, r) as node ->
            let (r', b) = splitMax r
            in adjust <| node, b
        | _ -> failwith "unexpected splitMax case"

    let rec private foldOpt (f: OptimizedClosures.FSharpFunc<_,_,_>) x t =
        match t with
        | E -> x
        | T(_, l, v, r) ->
            let x = foldOpt f x l
            let x = f.Invoke(x,v)
            foldOpt f x r

    /// Executes a function on each element in order (for example: 1, 2, 3 or a, b, c).
    let fold f x t = foldOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) x t

    let rec private foldBackOpt (f: OptimizedClosures.FSharpFunc<_,_,_>) x t =
        match t with
        | E -> x
        | T(_, l, v, r) ->
            let x = foldBackOpt f x r
            let x = f.Invoke(x,v)
            foldBackOpt f x l

    /// Executes a function on each element in reverse order (for example: 3, 2, 1 or c, b, a).
    let foldBack f x t = foldBackOpt (OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)) x t