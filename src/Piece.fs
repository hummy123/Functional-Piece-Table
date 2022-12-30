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

    let compareWithSpan (start: int) (finish: int) (curIndex: int) curPiece =
        let pieceStop = curIndex + curPiece.Span.Length

        if start <= curIndex && finish >= pieceStop then
            PieceFullyInSpan
        elif start <= curIndex && finish < pieceStop && curIndex < finish then
            StartOfPieceInSpan
        elif start > curIndex && finish >= pieceStop && start <= pieceStop then
            EndOfPieceInSpan
        elif start >= curIndex && finish <= pieceStop then
            SpanWithinPiece
        elif curIndex > start then
            GreaterThanSpan
        else
            LessThanSpan

    let deleteInRange curIndex start finish piece =
        let p1Start = piece.Span.Start
        let p1Length = start - curIndex
        let p1 = {piece with Span = Span.createWithLength p1Start p1Length}

        let p2Start = finish - curIndex + piece.Span.Start
        
        let p2Stop = Span.stop piece.Span
        let p2 = {piece with Span = Span.createWithStop p2Start p2Stop}
        (p1, p2)

    let deleteAtStart curIndex finish piece =
        let newPieceStart = piece.Span.Start + (finish - curIndex)
        let newPieceSpan = Span.createWithStop newPieceStart (Span.stop piece.Span)
        { piece with Span = newPieceSpan }

    let deleteAtEnd curIndex start piece =
        let newLength = start - curIndex
        let newSpan = Span.createWithLength piece.Span.Start newLength
        { piece with Span = newSpan }

    let text piece table =
        Buffer.substring piece.Span table.Buffer

    let private textInRange curIndex start finish piece table =
        let textStart = start - curIndex + piece.Span.Start
        let textStop = finish - curIndex + piece.Span.Start
        Buffer.substring (Span.createWithStop textStart textStop) table.Buffer

    let private textAtStart curIndex finish piece table =
        let textStop = piece.Span.Start + (finish - curIndex)
        let substrSpan = Span.createWithStop piece.Span.Start textStop
        Buffer.substring substrSpan table.Buffer

    let private textAtEnd curIndex start piece table =
        let textStart = start - curIndex + piece.Span.Start
        let textStop = Span.stop piece.Span
        let substrSpan = Span.createWithStop textStart textStop
        Buffer.substring substrSpan table.Buffer

    let inline textSlice pos curIndex piece start finish table =
        match pos with
        | PieceFullyInSpan -> text piece table
        | SpanWithinPiece -> textInRange curIndex start finish piece table
        | StartOfPieceInSpan -> textAtStart curIndex finish piece table
        | EndOfPieceInSpan -> textAtEnd curIndex start piece table
        | _ -> failwith "unexpected Piece.textSlice case"