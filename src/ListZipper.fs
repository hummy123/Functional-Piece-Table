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
        | _ -> failwith "Tried to move zipper forwards when we are already at end."

    let prev zipper =
        match zipper.Path with
        | b :: bs ->
            { Focus = b :: zipper.Focus
              Path = bs
              Index = prevIndex zipper }
        | _ -> failwith "Tried to move zipper backwards when we are already at start."

    let rec insert insIndex piece zipper =
        if zipper.Path.IsEmpty && zipper.Focus.IsEmpty then
            createWithPiece piece
        else
            let curPos = Piece.compareWithIndex insIndex zipper.Index zipper.Focus[0]
            match curPos, zipper.Path, zipper.Focus with
            | EqualTo, _, f -> {zipper with Focus = piece::f}
            | InRangeOf, p, [f] -> 
                let (p1, p2, p3) = Piece.split f piece (insIndex - zipper.Index)
                { zipper with Focus = [ p2; p3 ]; Path = p1::p; Index = zipper.Index + p1.Span.Length }
            | InRangeOf, p, fHead::fList -> 
                let (p1, p2, p3) = Piece.split fHead piece (insIndex - zipper.Index)
                { zipper with Focus = [ p2; p3 ] @ fList ; Path = p1::p; Index = zipper.Index + p1.Span.Length }
            | LessThan, _, f -> 
                if f.IsEmpty then
                    failwith "Bad ListZipper.insert caller case: Insertion index is less than 0."
                else
                    insert insIndex piece (next zipper)
            | GreaterThan, p, _ -> 
                if p.IsEmpty then
                    failwith "Bad ListZipper.insert caller case: Insertion index is less than 0."
                else
                    insert insIndex piece (prev zipper)
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
            printfn "[p]"
            printfn "%A" zipper.Focus
            if dStop > curIndex + p.Span.Length then
                printfn "c1"
                FullDelete(listPos+1, curIndex)
            else
                printfn "c2"
                // there seems to be a problem with Piece.delete
                let deleteData = Piece.delete curIndex deleteSpan p
                printfn "%A" deleteData
                PartialDelete(deleteData, listPos+1, curIndex)
        | p:: _, dStop, curIndex ->
            if deleteSpan.Start > curIndex then
                printfn "d1"
                deleteRightAcc deleteSpan deleteStop (next zipper) (listPos+1)
            elif dStop > curIndex + p.Span.Length && deleteSpan.Start <= curIndex then
                printfn "d2"
                deleteRightAcc deleteSpan deleteStop (next zipper) (listPos+1)
            else
                printfn "d3"
                let deleteData = Piece.delete curIndex deleteSpan p
                PartialDelete(deleteData, listPos + 1, curIndex)

    let delete deleteSpan (table: TextTableType) =
        // I think in left/right case, I should move the zipper to the in-range position first and then delete.
        // Maybe? Because I don't want to delete intermediary positions.
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
