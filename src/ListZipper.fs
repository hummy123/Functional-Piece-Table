// Inspiration from https://github.com/fsprojects/FSharpx.Collections/blob/master/src/FSharpx.Collections.Experimental/ListZipper.fs
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
        | Empty -> ([], 0)
        | CutOne(p,d) -> ([ p ], d)
        | CutTwo (p1, p2, d) -> ([ p1; p2 ], d)

    let rec private deleteLeft deleteSpan zipper =
        match zipper.Path, deleteSpan, zipper.Index with
        (* When we have an empty list, just return an empty list back. *)
        | [], _, _ -> ([], 0)
        (* When we have exactly one element in the path and need to delete at least a part of it. *)
        | [ p ], dSpan, curIndex when dSpan.Start < curIndex + p.Span.Length -> 
            let (deleteList, deletedChars) = deletePiece dSpan p
            (deleteList, deletedChars)
        (* When we have exactly one element and don't need to do anything with it. *)
        | [ p ], _, _ -> ([ p ], 0)
        (* When we have at least two elements and need to delete at least a part. *)
        | p :: _, dSpan, curIndex when dSpan.Start < curIndex + p.Span.Length ->
            let (deleteList, deletedChars) = deletePiece dSpan p
            let (newList, newlyDeletedChars) = (deleteLeft deleteSpan (prev zipper))
            (newList @ deleteList, deletedChars + newlyDeletedChars)
        (* When we have at least two elements but do not need to do anything with them. *)
        | pl, _, _ -> (pl, 0)

    let rec private deleteRight deleteSpan zipper =
        let deleteEnd = Span.stop deleteSpan

        match zipper.Focus, deleteEnd, zipper.Index with
        | [], _, _ -> []
        | [ p ], dEnd, curIndex when dEnd <= curIndex + p.Span.Length -> 
            let (deleteList, _) = deletePiece deleteSpan p
            deleteList
        | [ p ], _, _ -> [ p ]
        | p :: _, dEnd, curIndex when dEnd <= curIndex + p.Span.Length ->
            let (deleteData, _) = deletePiece deleteSpan p
            deleteData @ (deleteRight deleteSpan (next zipper))
        | pl, _, _ -> pl


    let delete deleteSpan (table: TextTableType) =
        let (leftList, leftDeleted) = deleteLeft deleteSpan table.Pieces
        let right = deleteRight deleteSpan table.Pieces

        (* Calculate the new index (the current index minus how many characters we deleted to the left. )*)
        let newIndex = table.Pieces.Index - leftDeleted

        let pieces =
            { Focus = right
              Path = leftList
              Index = newIndex }

        { table with Pieces = pieces }

    let ofList zipper = zipper.Path @ zipper.Focus

    /// Retrieve the string contained within a TextTableType. Building this string takes O(n).
    /// Note that .NET has a size limit of 2 GB on objects and thus you cannot retrieve a string longer than that.
    let text (table: TextTableType) =
        let pieces = ofList table.Pieces

        let rec buildText listPos (acc: string) =
            let piece = pieces[listPos]

            if listPos = pieces.Length - 1 then
                acc + (Piece.text piece table)
            else
                acc + (Piece.text piece table) |> buildText (listPos + 1)

        buildText 0 ""

    let rec private textSliceLeft textSpan table acc = 
        let zipper = table.Pieces
        match zipper.Path, textSpan, zipper.Index with
        | [], _, _ -> (acc, table)
        | [p], tSpan, curIndex when tSpan.Start < curIndex + p.Span.Length ->
            let pieceText = Piece.textSlice curIndex tSpan.Length p table
            (pieceText + acc, table)
        | [_], _, _ -> (acc, table)
        | p :: _, tSpan, curIndex when tSpan.Start < curIndex + p.Span.Length ->
            let pieceText = Piece.textSlice curIndex tSpan.Length p table
            let leftTable = {table with Pieces = prev zipper; }
            textSliceLeft tSpan leftTable (pieceText + acc)
        | _, _, _ -> (acc, table)

    let rec private textSliceRight tSpan textStop table acc =
        let zipper = table.Pieces
        match zipper.Focus, textStop, zipper.Index with
        | [], _, _ -> (acc, table)
        | [p], tStop, curIndex when tStop <= curIndex + p.Span.Length ->
            let pieceText = Piece.textSlice curIndex tSpan.Length p table
            (acc + pieceText, table)
        | [_], _, _ -> (acc, table)
        | p:: _, tStop, curIndex when tStop <= curIndex + p.Span.Length ->
            let pieceText = Piece.textSlice curIndex tSpan.Length p table
            let rightTable = {table with Pieces = next zipper}
            textSliceRight tSpan textStop rightTable (acc + pieceText)
        | _, _, _ -> (acc, table)
    
    let textSlice textSpan (table: TextTableType) =
        let (leftText, leftTable) = textSliceLeft textSpan table ""
        let (fullText, rightTable) = textSliceRight textSpan (Span.stop textSpan) table leftText
        if textSpan.Start <= table.Pieces.Index then
            (fullText, leftTable)
        else
            (fullText, rightTable)
