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

    type ItemsToRemove = int
    type ZipperIndex = int
    type internal ListDeleteData = 
        | PartialDelete of DeletedPiece * ItemsToRemove * ZipperIndex
        | FullDelete of ItemsToRemove * ZipperIndex

    let rec private deleteLeftAcc deleteSpan zipper listPos =
        match zipper.Path, deleteSpan, zipper.Index with
        | [], _, _-> FullDelete(listPos + 1, 0)
        | [p], dSpan, curIndex ->
            if dSpan.Start <= curIndex + p.Span.Length then
                // I think I have to send curIndex to Piece.delete so it doesn't delete
                // when it does not need to.
                let deleteData = Piece.delete curIndex dSpan p
                PartialDelete(deleteData, listPos + 1, curIndex)
            else
                FullDelete(listPos, curIndex)
        | p::_, dSpan, curIndex ->
            if dSpan.Start <= curIndex then
                deleteLeftAcc dSpan (prev zipper) (listPos + 1)
            else
                let deleteData = Piece.delete curIndex dSpan p
                PartialDelete(deleteData, listPos + 1, curIndex)

    let rec private deleteRightAcc deleteSpan deleteStop zipper listPos =
        match zipper.Focus, deleteStop, zipper.Index with
        | [], _, curIndex -> FullDelete(listPos + 1, curIndex)
        | [p], dStop, curIndex ->
            if dStop > curIndex + p.Span.Length then
                FullDelete(listPos+1, curIndex)
            else
                let deleteData = Piece.delete curIndex deleteSpan p
                PartialDelete(deleteData, listPos+1, curIndex)
        | p:: _, dStop, curIndex ->
            if dStop > curIndex + p.Span.Length then
                printfn "dStop >= curIndex"
                printfn "dStop = %i" dStop
                deleteRightAcc deleteSpan deleteStop (next zipper) (listPos+1)
            else
                let deleteData = Piece.delete curIndex deleteSpan p
                PartialDelete(deleteData, listPos + 1, curIndex)

    let delete deleteSpan (table: TextTableType) =
        let (leftIndex, path) =
            if table.Pieces.Index <= deleteSpan.Start then 
                (table.Pieces.Index, table.Pieces.Path)
            else
                match deleteLeftAcc deleteSpan table.Pieces 0 with
                | PartialDelete(partialPiece, removeNum, index) ->
                    match partialPiece with
                    | Empty -> failwith "unexpected ListZipper.delete case (left delete is Empty)"
                    | CutOne (piece, _) -> 
                        let index = index - piece.Span.Length 
                        (index, piece :: table.Pieces.Path[removeNum..])
                    | CutTwo (p1, p2, _) ->
                        let index = p1.Span.Length + p2.Span.Length + index
                        (index, p1::p2::table.Pieces.Path[removeNum..])
                | FullDelete(removeNum, index) ->
                    (index, table.Pieces.Path[removeNum..])

        let deleteStop = Span.stop deleteSpan
        let focus =
            if table.Pieces.Index > deleteStop then
                table.Pieces.Focus
            else
                match deleteRightAcc deleteSpan deleteStop table.Pieces 0 with
                | PartialDelete(partialPiece, removeNum, _) ->
                    match partialPiece with
                    | Empty -> table.Pieces.Focus[removeNum..]
                    | CutOne(piece, _) ->
                        piece::table.Pieces.Focus[removeNum..]
                    | CutTwo (p1,p2, _) ->
                        p1::p2::table.Pieces.Focus[removeNum..]
                | FullDelete(removeNum, _) ->
                    table.Pieces.Focus[removeNum..]

        let pieces = 
            {Path = path; Focus = focus; Index = leftIndex}

        {table with Pieces = pieces}

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
