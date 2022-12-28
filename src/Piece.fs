namespace PieceTable

open Types

module internal Piece =
    /// Creates a Piece specifying the span's start and length instead of the span itself.
    let create isOriginal start length =
        { Span = Span.createWithLength start length }

    /// Creates a Piece specifying its span.
    let createWithSpan span =
        { Span = span }

    /// Merges two consecutive pieces into one.
    /// Expects to be called only when Piece.isConsecutive returns true. 
    let merge a b =
        createWithSpan <| Span.createWithLength a.Span.Start (a.Span.Length + b.Span.Length)

    /// Split operation that returns three pieces.
    /// Correct usage of this method assumes that Piece a's span starts before and ends after Piece b's span.
    let split (a: PieceType) (difference: int) =
        let p1Length = a.Span.Start + difference
        let p1Span = Span.createWithStop a.Span.Start p1Length  
        let p1 = createWithSpan p1Span

        let span3 = Span.createWithStop (p1Length) (Span.stop a.Span)
        let p3 = createWithSpan span3
        (p1, p3)

    let compareWithSpan (span: SpanType) curIndex curPiece =
        let spanStop = Span.stop span
        let pieceStop = curIndex + curPiece.Span.Length

        if span.Start <= curIndex && spanStop >= pieceStop then
            PieceFullyInSpan
        elif span.Start <= curIndex && spanStop < pieceStop && curIndex < spanStop then
            StartOfPieceInSpan
        elif span.Start > curIndex && spanStop >= pieceStop && span.Start <= pieceStop then
            EndOfPieceInSpan
        elif span.Start >= curIndex && spanStop <= pieceStop then
            SpanWithinPiece
        elif curIndex > span.Start then
            GreaterThanSpan
        else
            LessThanSpan

    /// Specifies how we should handle a Piece given to the delete method.
    /// Empty: We can simply remove this Piece from the list.
    /// CutOne: We can replace the Piece with the newly returned one.
    /// CutTwo: We can replace the Piece given in the parameter with the two pieces returned, where the first is before the second.
    type DeletedPiece =
        | Empty
        | CutOne of PieceType * int
        | CutTwo of PieceType * PieceType * int

    let deleteInRange curIndex span piece =
        let spanStop = Span.stop span 

        let p1Start = piece.Span.Start
        let p1Length = span.Start - curIndex
        let p1 = {piece with Span = Span.createWithLength p1Start p1Length}

        let p2Start = spanStop - curIndex + piece.Span.Start
        
        let p2Stop = Span.stop piece.Span
        let p2 = {piece with Span = Span.createWithStop p2Start p2Stop}
        (p1, p2)

    let deleteAtStart curIndex span piece =
        let spanStop = Span.stop span
        let newPieceStart = piece.Span.Start + (spanStop - curIndex)
        let newPieceSpan = Span.createWithStop newPieceStart (Span.stop piece.Span)
        { piece with Span = newPieceSpan }

    let deleteAtEnd curIndex span piece =
        let newLength = span.Start - curIndex
        let newSpan = Span.createWithLength piece.Span.Start newLength
        { piece with Span = newSpan }

    /// Deletes either a part of a piece or a full piece itself.
    /// See the documentation for the DeletedPiece type on how to use this method's return value.
    (*let delete pos curIndex (span: SpanType) (piece: PieceType) =        
        match pos with
        | PieceFullyInSpan -> DeletedPiece.Empty
        | SpanWithinPiece -> deleteInRange curIndex span piece
        | StartOfPieceInSpan -> deleteAtStart curIndex span piece
        | EndOfPieceInSpan -> deleteAtEnd curIndex span piece
        | _ -> failwith "Piece.delete error"
        *)
    let text piece table =
        Buffer.substring piece.Span table.Buffer

    let private textInRange curIndex span piece table =
        let spanStop = Span.stop span 
        let textStart = span.Start - curIndex + piece.Span.Start
        let textStop = spanStop - curIndex + piece.Span.Start
        Buffer.substring (Span.createWithStop textStart textStop) table.Buffer

    let private textAtStart curIndex span piece table =
        let spanStop = Span.stop span
        let textStop = piece.Span.Start + (spanStop - curIndex)
        let substrSpan = Span.createWithStop piece.Span.Start textStop
        Buffer.substring substrSpan table.Buffer

    let private textAtEnd curIndex span piece table =
        let textStop = Span.stop piece.Span
        let textStart = span.Start - curIndex
        let substrSpan = Span.createWithStop textStart textStop
        Buffer.substring substrSpan table.Buffer

    let textSlice pos curIndex piece span table =
        match pos with
        | PieceFullyInSpan -> text piece table
        | SpanWithinPiece -> textInRange curIndex span piece table
        | StartOfPieceInSpan -> textAtStart curIndex span piece table
        | EndOfPieceInSpan -> textAtEnd curIndex span piece table
        | _ -> failwith "unexpected Piece.textSlice case"