/// Inspiration from https://github.com/fsprojects/FSharpx.Collections/blob/master/src/FSharpx.Collections.Experimental/ListZipper.fs
namespace PieceTable

open Types
open Piece

module ListZipper =
    let empty = { Focus = []; Path = []; Index = 0 }

    let createWithPiece piece =
        { Focus = [ piece ]
          Path = []
          Index = 0 }

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
        (* The two cases below throw an error when a caller tries to use the method incorrectly,
         * such as when they try inserting at index 6 when the table contains "12345" or try inserting at an index less than zero.*)
        | _, [], curIndex when curIndex < insIndex ->
            failwith "Bad ListZipper.insert caller case: Insertion index is greater than the whole document."
        | [], _, curIndex when curIndex > insIndex ->
            failwith "Bad ListZipper.insert caller case: Insertion index is less than 0."
        | _, _, _ -> failwith "unexpected ListZipper.insert case"

    let private deletePiece dSpan p =
        match Piece.delete dSpan p with
        | Empty -> []
        | CutOne p -> [ p ]
        | CutTwo (p1, p2) -> [ p1; p2 ]

    let rec private deleteLeft deleteSpan zipper =
        match zipper.Path, deleteSpan, zipper.Index with
        (* When we haev an empty list, just return an empty list back. *)
        | [], _, _ -> []
        (* When we have exactly one element in the path and need to delete at least a part of it. *)
        | [ p ], dSpan, curIndex when dSpan.Start < curIndex + p.Span.Length -> deletePiece dSpan p
        (* When we have exactly one element and don't need to do anything with it. *)
        | [ p ], _, _ -> [ p ]
        (* When we have at least two elements and need to delete at least a part. *)
        | p :: _, dSpan, curIndex when dSpan.Start < curIndex + p.Span.Length ->
            let deleteData = deletePiece dSpan p
            (deleteLeft deleteSpan (prev zipper)) @ deleteData
        (* When we have at least two elements but do not need to do anything with them. *)
        | pl, _, _ -> pl

    let rec private deleteRight deleteSpan zipper =
        let deleteEnd = Span.stop deleteSpan

        match zipper.Focus, deleteEnd, zipper.Index with
        | [], _, _ -> []
        | [ p ], dEnd, curIndex when dEnd <= curIndex + p.Span.Length -> deletePiece deleteSpan p
        | [ p ], _, _ -> [ p ]
        | p :: _, dEnd, curIndex when dEnd <= curIndex + p.Span.Length ->
            let deleteData = deletePiece deleteSpan p
            deleteData @ (deleteRight deleteSpan (next zipper))
        | pl, _, _ -> pl


    let rec delete deleteSpan (table: TextTableType) =
        let left = deleteLeft deleteSpan table.Pieces
        let right = deleteRight deleteSpan table.Pieces

        (* We put the whole list at the focus because that's easier and this list zipper is only temporary. *)
        let pieces =
            { Focus = left @ right
              Path = []
              Index = 0 }

        { table with Pieces = pieces }

    let ofList zipper = zipper.Path @ zipper.Focus
