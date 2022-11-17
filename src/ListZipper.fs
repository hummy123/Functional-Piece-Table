/// Inspiration from https://github.com/fsprojects/FSharpx.Collections/blob/master/src/FSharpx.Collections.Experimental/ListZipper.fs
namespace PieceTable

open Types

module ListZipper =
    let empty = { Focus = []; Path = []; Index = 0 }

    let createWithPiece piece =
        { Focus = [ piece ]
          Path = []
          Index = 0 }

    let focus zipper =
        match zipper.Focus with
        | f :: _ -> f
        | _ -> failwith "unexpected ListZipper.focus result."

    let private nextIndex zipper =
        match zipper.Focus with
        | [ f ] -> zipper.Index + f.Span.Length
        | f :: _ -> zipper.Index + f.Span.Length
        | _ -> failwith "unexpected ListZipper.nextIndex result"

    let private prevIndex zipper =
        match zipper.Path with
        | [ p ] -> zipper.Index - p.Span.Length
        | p :: _ -> zipper.Index - p.Span.Length
        | _ -> failwith "unexpected ListZipper.prevIndex result"

    let next zipper =
        match zipper.Focus with
        | f :: fs ->
            { Focus = fs
              Path = f :: zipper.Path
              Index = nextIndex zipper }
        | _ -> failwith "unexpected ListZipper.forward result."

    let prev zipper =
        match zipper.Path with
        | b :: bs ->
            { Focus = b :: zipper.Focus
              Path = bs
              Index = prevIndex zipper }
        | _ -> failwith "unexpected ListZipper.back result."

    let rec insert insIndex piece zipper =
        match zipper.Path, zipper.Focus, zipper.Index with
        (* When the zipper is empty. *)
        | [], [], _ -> createWithPiece piece
        (* When we are at the index we want to insert at. *)
        | _, _, curIndex when curIndex = insIndex -> { zipper with Focus = piece :: zipper.Focus }
        (* When we want to insert, after the current index  but before the next one in the focus (in range). *)
        (* Sub condition 1: The focus only has 1 element. *)
        | _, [ f ], curIndex when (insIndex > curIndex) && (insIndex < nextIndex zipper) ->
            let (p1, p2, p3) = Piece.split f piece (insIndex - curIndex)
            { zipper with Focus = [ p1; p2; p3 ] }
        (* Sub condition 2: The focus has more than one element. *)
        | _, f :: fs, curIndex when insIndex > curIndex && insIndex < nextIndex zipper ->
            let (p1, p2, p3) = Piece.split f piece (insIndex - curIndex)
            { zipper with Focus = [ p1; p2; p3 ] @ fs }
        (* When we are before the index we want to insert at. *)
        | _, f, curIndex when curIndex < insIndex && f.Length > 0 -> insert insIndex piece (next zipper)
        (* When we are after the index we want to insert at. *)
        | p, _, curIndex when curIndex > insIndex && p.Length > 0 -> insert insIndex piece (prev zipper)
        | _, _, _ -> failwith "unexpected ListZipper.insert case"

    let ofList zipper = zipper.Path @ zipper.Focus
