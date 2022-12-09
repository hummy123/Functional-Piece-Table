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
        | _ -> zipper.Index

    let private prevIndex zipper =
        match zipper.Path with
        | [ p ] -> zipper.Index - p.Span.Length
        | p :: _ -> zipper.Index - p.Span.Length
        | _ -> zipper.Index

    let rec next zipper =
        match zipper.Focus, zipper.Path with
        (* Removes empty pieces from zipper when we traverse over them. *)
        | f :: fs, _ when f.Span.Length <= 0 -> 
            next {zipper with Focus = fs}
         | f :: fs, p :: ps when isConsecutive f p ->
            let mergePiece = Piece.merge f p
            { Focus = fs; Path = mergePiece::ps; Index = nextIndex zipper}
        | f :: fs, _ ->
            { Focus = fs
              Path = f :: zipper.Path
              Index = nextIndex zipper }
        | [], _ ->
            zipper

    let rec prev zipper =
        match zipper.Focus, zipper.Path with
        | _, b :: bs when b.Span.Length <= 0 ->
            prev {zipper with Path = bs}
        | f :: fs, b :: bs when isConsecutive f b ->
            let mergePiece = Piece.merge f b
            { Focus = mergePiece::fs; Path = bs; Index = prevIndex zipper}
        | _, b :: bs ->
            { Focus = b :: zipper.Focus
              Path = bs
              Index = prevIndex zipper }
        | _, [] -> 
            zipper

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
            | LessThanIndex, _, f -> 
                if f.IsEmpty then
                    {zipper with Focus = f @ [piece];}
                else
                    insert insIndex piece (next zipper)
            | GreaterThanIndex, p, f -> 
                if p.IsEmpty then
                    {zipper with Focus = piece::f; Index = 0}
                else
                    insert insIndex piece (prev zipper)
            | _, _, _ -> failwith "unexpected ListZipper.insert case"

    let private deleteList pos curIndex dSpan dPiece =
        match Piece.delete pos curIndex dSpan dPiece with
        | Empty -> [], 0
        | CutOne(p,c) -> [p], c
        | CutTwo(p1,p2,c) -> [p1;p2], c

    let rec private deletePath dSpan zipper (pathAcc: PieceType list) dLength =
        if zipper.Path.IsEmpty then
            pathAcc, dLength
        else
            (* The zipper.Index value contains the index after all the pieces in the zipper,
             *  but the Piece.delete method we want to use asks for the start of a piece's index. *)
            let pieceIndex = zipper.Index - zipper.Path[0].Span.Length
            let pos = Piece.compareWithSpan dSpan pieceIndex zipper.Path[0]
            match pos, zipper.Path with
            | StartOfPieceInSpan, p ->
                let (dList, dNum) = deleteList pos pieceIndex dSpan p[0]
                dList @ pathAcc, dLength + dNum
            | EndOfPieceInSpan, p ->
                let (dList, dNum) = deleteList pos pieceIndex dSpan p[0]
                deletePath dSpan (prev zipper) (dList @ pathAcc) (dLength + dNum)
            | PieceFullyInSpan, p -> 
                deletePath dSpan (prev zipper) pathAcc (p[0].Span.Length + dLength)
            | SpanWithinPiece, p ->
                let (dList, dNum) = deleteList pos pieceIndex dSpan p[0]
                dList @ pathAcc, dLength + dNum
            | LessThanSpan, _ -> 
                pathAcc, dLength
            | GreaterThanSpan, _ -> deletePath dSpan (prev zipper) pathAcc dLength

    let private deleteLeft deleteSpan table =
        if table.Pieces.Index <= deleteSpan.Start then 
            table
        else
            let (newPath, dLength) = deletePath deleteSpan table.Pieces [] 0
            let pieces = {table.Pieces with Path = newPath; Index = table.Pieces.Index - dLength}
            {table with Pieces = pieces }

    let rec private deleteFocus dSpan zipper (focusAcc: PieceType list) table =
        if zipper.Focus.IsEmpty then
            focusAcc
        else
            let pos = Piece.compareWithSpan dSpan zipper.Index zipper.Focus[0]
            match pos, zipper.Focus with
            | StartOfPieceInSpan, f ->
                let (dList, _) = deleteList pos zipper.Index dSpan f[0]
                focusAcc @ dList
            | EndOfPieceInSpan, f ->
                let (dList, _) = deleteList pos zipper.Index dSpan f[0]
                deleteFocus dSpan (next zipper) (focusAcc @ dList) table
            | PieceFullyInSpan, _ -> deleteFocus dSpan (next zipper) focusAcc table
            | SpanWithinPiece, fHead::fList ->
                let (dList, _) = deleteList pos zipper.Index dSpan fHead
                focusAcc @ dList @ fList
            | GreaterThanSpan, _ -> focusAcc
            | LessThanSpan, fHead::_-> deleteFocus dSpan (next zipper) (fHead :: focusAcc) table
            | SpanWithinPiece, _ -> failwith "logically impossible case"
            | LessThanSpan, _ -> failwith "Reached end of zipper in deleteFocus"

    let private deleteRight dSpan table =
        if table.Pieces.Index > Span.stop dSpan then
            table
        else
            let focus = deleteFocus dSpan table.Pieces [] table
            let pieces = {table.Pieces with Focus = focus}
            { table with Pieces = pieces }

    let delete deleteSpan (table: TextTableType) =
        deleteLeft deleteSpan table
        |> deleteRight deleteSpan

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
    
    let textSlice textSpan (table: TextTableType) =
        "" // todo : implement
