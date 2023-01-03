namespace PieceTable

open Types

module internal Piece =
    /// Creates a Piece specifying the span's start and length instead of the span itself.
    let create start length lines =
        { Span = Span.createWithLength start length; Lines = lines }

    /// Creates a Piece specifying its span.
    let createWithSpan span lines =
        { Span = span; Lines = lines }

    /// Merges two consecutive pieces into one.
    /// Expects to be called only when Piece.isConsecutive returns true. 
    let inline merge a b =
        let span = Span.createWithLength a.Span.Start (a.Span.Length + b.Span.Length)
        let lines =
            if a.Lines.Length = 0 && b.Lines.Length = 0
            then a.Lines
            elif a.Lines.Length = 0
            then b.Lines
            elif b.Lines.Length = 0
            then a.Lines
            else Array.append a.Lines b.Lines
        createWithSpan span lines

    /// Split operation that returns three pieces.
    /// Correct usage of this method assumes that Piece a's span starts before and ends after Piece b's span.
    let inline split (a: PieceType) (difference: int) =
        let p1Length = a.Span.Start + difference
        let p1Span = Span.createWithStop a.Span.Start p1Length
        let p1Lines = Array.filter (fun v -> v < difference) a.Lines
        let p1 = createWithSpan p1Span p1Lines

        let span3 = Span.createWithStop (p1Length) (Span.stop a.Span)
        let p3Lines = Array.filter (fun v -> v >= difference) a.Lines
        let p3 = createWithSpan span3 p3Lines
        (p1, p3)

    let inline deleteInRange curIndex start finish piece =
        let p1Start = piece.Span.Start
        let p1Length = start - curIndex
        let p1Span = Span.createWithLength p1Start p1Length
        let p1Finish = p1Start + p1Length
        let p1Lines =
            if piece.Lines.Length = 0
            then piece.Lines
            elif p1Length < piece.Lines[0]
            then [||]
            else Array.filter (fun p -> p < p1Finish) piece.Lines
        let p1 = createWithSpan p1Span p1Lines

        let p2Start = finish - curIndex + piece.Span.Start
        let p2Stop = Span.stop piece.Span
        let p2Span = Span.createWithStop p2Start p2Stop
        let p2Lines =
            if piece.Lines.Length = 0
            then piece.Lines
            elif p2Start > piece.Lines[piece.Lines.Length - 1]
            then [||]
            else Array.filter (fun p -> p >= p2Start) piece.Lines
        let p2 = createWithSpan p2Span p2Lines
        (p1, p2)

    let inline deleteAtStart curIndex finish piece =
        let newPieceStart = piece.Span.Start + (finish - curIndex)
        let newPieceSpan = Span.createWithStop newPieceStart (Span.stop piece.Span)
        { piece with Span = newPieceSpan }

    let inline deleteAtEnd curIndex start piece =
        let newLength = start - curIndex
        let newSpan = Span.createWithLength piece.Span.Start newLength
        { piece with Span = newSpan }

    let inline text piece table =
        Buffer.substring piece.Span.Start piece.Span.Length table.Buffer

    let inline textInRange curIndex start finish piece table =
        let textStart = start - curIndex + piece.Span.Start
        let textLength = finish - curIndex + piece.Span.Start - textStart
        Buffer.substring textStart textLength table.Buffer

    let inline textAtStart curIndex finish piece table =
        let textStop = piece.Span.Start + (finish - curIndex)
        let substrSpan = Span.createWithStop piece.Span.Start textStop
        Buffer.substring piece.Span.Start substrSpan.Length table.Buffer

    let inline textAtEnd curIndex start piece table =
        let textStart = start - curIndex + piece.Span.Start
        let textStop = Span.stop piece.Span
        Buffer.substring textStart (textStop - textStart) table.Buffer