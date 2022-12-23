// Inspiration from https://github.com/fsprojects/FSharpx.Collections/blob/master/src/FSharpx.Collections.Experimental/ListZipper.fs
namespace PieceTable

open Types
open Piece
open System.IO

module ListZipper =
    /// Creates an empty ListZipper.
    let empty = { Focus = []; Path = []; Index = 0 }

    /// Creates a ListZipper with a single piece at index 0.
    let createWithPiece piece =
        { Focus = [ piece ]
          Path = []
          Index = 0 }

    /// Calculates the next index from the zipper's current position.
    let private nextIndex zipper =
        match zipper.Focus with
        | [ f ] -> zipper.Index + f.Span.Length
        | f :: _ -> zipper.Index + f.Span.Length
        | _ -> zipper.Index

    /// Calculates the previous index from the zipper's current position.
    let private prevIndex zipper =
        match zipper.Path with
        | [ p ] -> zipper.Index - p.Span.Length
        | p :: _ -> zipper.Index - p.Span.Length
        | _ -> zipper.Index

    /// Moves the zipper forwards if possible.
    let private next zipper =
        match zipper.Focus, zipper.Path with
        | f :: fs, _ ->
            { Focus = fs
              Path = f :: zipper.Path
              Index = nextIndex zipper }
        | [], _ ->
            zipper

    /// Moves the zipper backwards if possible.
    let private prev zipper =
        match zipper.Focus, zipper.Path with
        | _, b :: bs ->
            { Focus = b :: zipper.Focus
              Path = bs
              Index = prevIndex zipper }
        | _, [] -> 
            zipper

    /// Inserts "text" (in actuality a piece representing the text in the buffer)
    /// into a zipper.
    let rec insert insIndex piece zipper =
        if zipper.Path.IsEmpty && zipper.Focus.IsEmpty then
            createWithPiece piece
        elif zipper.Focus.IsEmpty && insIndex >= zipper.Index then
            {zipper with Focus = [piece]}
        else
            let curPos = Piece.compareWithIndex insIndex zipper.Index zipper.Focus[0]
            match curPos, zipper.Path, zipper.Focus with
            | EqualTo, _, _ -> 
                {zipper with Focus = piece::zipper.Focus}
            | AtEndOf, _, [f] ->
                if isConsecutive f piece
                then { zipper with Focus = [(Piece.merge f piece)] }
                else { zipper with Path = f::zipper.Path; Focus = [piece]; Index = zipper.Index + f.Span.Length}
            | AtEndOf, _, fHead::fList ->
                {zipper with Focus = fHead::piece::fList}
            | InRangeOf, p, [f] -> 
                let (p1, p3) = Piece.split f piece (insIndex - zipper.Index) zipper.Index insIndex
                { zipper with Focus = [ piece; p3 ]; Path = p1::p; Index = zipper.Index + p1.Span.Length }
            | InRangeOf, p, fHead::fList -> 
                let (p1,  p3) = Piece.split fHead piece (insIndex - zipper.Index) zipper.Index insIndex
                { zipper with Focus = piece::p3::fList ; Path = p1::p; Index = zipper.Index + p1.Span.Length }
            | LessThanIndex, _, f -> 
                if f.IsEmpty then
                    {zipper with Focus = f @ [piece];}
                else
                    insert insIndex piece (prev zipper)
            | GreaterThanIndex, p, f -> 
                if p.IsEmpty then
                    {zipper with Focus = piece::f; Index = 0}
                else
                    insert insIndex piece (next zipper)
            | _, _, _ -> failwith "unexpected ListZipper.insert case"

    /// Transforms the data returned by Piece.delete into a format
    /// easily used by the zipper.
    let private deleteList pos curIndex dSpan dPiece isPath =
        match Piece.delete pos curIndex dSpan dPiece with
        | Empty -> [], 0
        | CutOne(p,c) -> 
            [p], c
        | CutTwo(p1,p2,c) -> 
            let l = 
                if isPath
                then [p2;p1]
                else [p1;p2]
            l, c

    /// Deletes pieces from the zipper's path if they are covered in the span's range.
    let private deleteLeft deleteSpan table =
        let rec deletePath dSpan zipper (pathAcc: PieceType list) dLength =
            if zipper.Path.IsEmpty then
                pathAcc, dLength
            else
                (* The zipper.Index value contains the index after all the pieces in the zipper,
                 *  but the Piece.delete method we want to use asks for the start of a piece's index. *)
                let pieceIndex = zipper.Index - zipper.Path[0].Span.Length
                let pos = Piece.compareWithSpan dSpan pieceIndex zipper.Path[0]
                match pos, zipper.Path with
                | StartOfPieceInSpan, p ->
                    let (dList, dNum) = deleteList pos pieceIndex dSpan p[0] true
                    dList @ pathAcc, dLength + dNum
                | EndOfPieceInSpan, p ->
                    let (dList, dNum) = deleteList pos pieceIndex dSpan p[0] true
                    deletePath dSpan (prev zipper) (dList @ pathAcc) (dLength + dNum)
                | PieceFullyInSpan, p -> 
                    deletePath dSpan (prev zipper) pathAcc (p[0].Span.Length + dLength)
                | SpanWithinPiece, p ->
                    let (dList, dNum) = deleteList pos pieceIndex dSpan p[0] true
                    dList @ pathAcc, dLength + dNum
                | LessThanSpan, _ -> 
                    pathAcc, dLength
                | GreaterThanSpan, _ -> deletePath dSpan (prev zipper) pathAcc dLength
        if table.Pieces.Index <= deleteSpan.Start then 
            table
        else
            let (newPath, dLength) = deletePath deleteSpan table.Pieces [] 0
            let pieces = {table.Pieces with Path = newPath; Index = table.Pieces.Index - dLength}
            {table with Pieces = pieces }

    /// Deletes pieces from the zipper's focus if they are covered in the span's range.
    let private deleteRight dSpan table =
        let rec deleteFocus dSpan zipper (focusAcc: PieceType list) table =
            if zipper.Focus.IsEmpty then
                focusAcc
            else
                let pos = Piece.compareWithSpan dSpan zipper.Index zipper.Focus[0]
                match pos, zipper.Focus with
                | StartOfPieceInSpan, f ->
                    let (dList, _) = deleteList pos zipper.Index dSpan f[0] false
                    focusAcc @ dList
                | EndOfPieceInSpan, f ->
                    let (dList, _) = deleteList pos zipper.Index dSpan f[0] false
                    deleteFocus dSpan (next zipper) (focusAcc @ dList) table
                | PieceFullyInSpan, _ -> deleteFocus dSpan (next zipper) focusAcc table
                | SpanWithinPiece, fHead::fList ->
                    let (dList, _) = deleteList pos zipper.Index dSpan fHead false
                    focusAcc @ dList @ fList
                | GreaterThanSpan, _ -> focusAcc
                | LessThanSpan, fHead::_-> deleteFocus dSpan (next zipper) (fHead :: focusAcc) table
                | SpanWithinPiece, _ -> failwith "logically impossible case"
                | LessThanSpan, _ -> failwith "Reached end of zipper in deleteFocus"
        if table.Pieces.Index > Span.stop dSpan then
            table
        else
            let focus = deleteFocus dSpan table.Pieces [] table
            let pieces = {table.Pieces with Focus = focus}
            { table with Pieces = pieces }

    /// Deletes "text" (in actuality, pieces or a piece or part of a piece are deleted)
    /// from a ListZipper.
    let delete deleteSpan (table: TextTableType) =
        deleteLeft deleteSpan table
        |> deleteRight deleteSpan

    /// Iterates on each piece in a table, optionally performing some function
    /// in between each piece.
    let private textBuilder fOnIterate (table: TextTableType) =
        let inline add1 acc piece = acc + piece
        let inline add2 scc piece = piece + scc
        let rec buildText fAdd (curList: PieceType list) listPos (acc: string) =
            if listPos = curList.Length then
                acc
            elif listPos = curList.Length - 1 then
                let piece = curList[listPos]
                let text = (Piece.text piece table)
                fOnIterate text
                fAdd acc (Piece.text piece table)
            else
                let piece = curList[listPos]
                let text = (Piece.text piece table)
                fOnIterate text
                fAdd acc (Piece.text piece table) |> buildText fAdd curList (listPos + 1)

        let path = buildText add2 table.Pieces.Path 0 ""
        buildText add1 table.Pieces.Focus 0 path

    /// Returns all text in the table.
    let text table = textBuilder (fun _ -> ()) table

    /// Writes all text in the table to a file using a TextWriter instance.
    let write table (writer: StreamWriter) =
        let writeFunc (str: string) =
            writer.Write(str)
        textBuilder writeFunc table |> ignore
        writer.Close()

    /// Returns text within the span's range located on the path (zipper's left).
    let private textSliceLeft span table =
        let rec textPath span (table: TextTableType) zipper acc =
            if zipper.Path.IsEmpty then
                acc
            else
                let pieceIndex = zipper.Index - zipper.Path[0].Span.Length
                let pos = Piece.compareWithSpan span pieceIndex zipper.Path[0]
                match pos, zipper.Path with
                | StartOfPieceInSpan, p ->
                    let text = Piece.textSlice pos pieceIndex p[0] span table
                    text + acc
                | EndOfPieceInSpan, p ->
                    let text = Piece.textSlice pos pieceIndex p[0] span table
                    textPath span table (prev zipper) (text + acc)
                | PieceFullyInSpan, p ->
                    let text = Piece.text p[0] table
                    textPath span table (prev zipper) (text + acc)
                | SpanWithinPiece, p ->
                    let text = Piece.textSlice pos pieceIndex p[0] span table
                    text + acc
                | LessThanSpan, _ -> acc
                | GreaterThanSpan, _ -> textPath span table (prev zipper) ""
        if table.Pieces.Index <= span.Start then 
            ""
        else
            textPath span table table.Pieces ""
        
    /// Returns text within the span's range located on the focus (zipper's right).
    let private textSliceRight span table =
        let rec textSliceFocus span zipper table acc =
            if zipper.Focus.IsEmpty then
                acc
            else
                let pos = Piece.compareWithSpan span zipper.Index zipper.Focus[0]
                match pos, zipper.Focus with
                | StartOfPieceInSpan, f ->
                    let text = Piece.textSlice pos zipper.Index f[0] span table
                    acc + text
                | EndOfPieceInSpan, f ->
                    let text = Piece.textSlice pos zipper.Index f[0] span table
                    textSliceFocus span (next zipper) table (acc + text)
                | PieceFullyInSpan, f -> 
                    let text = Piece.text f[0] table
                    textSliceFocus span (next zipper) table (acc + text)
                | SpanWithinPiece, f ->
                    let text = Piece.textSlice pos zipper.Index f[0] span table
                    textSliceFocus span (next zipper) table (acc + text)
                | GreaterThanSpan, _ -> acc
                | LessThanSpan, _-> 
                    textSliceFocus span (next zipper) table ""
        if table.Pieces.Index > Span.stop span then
            ""
        else
            textSliceFocus span table.Pieces table ""
    
    /// Returns the text in the table that fits in the span's range.
    let textSlice span (table: TextTableType) =
        let left = textSliceLeft span table
        let right = textSliceRight span table
        left + right