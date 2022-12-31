namespace PieceTable

open Types

module internal Piece =
    /// Creates a Piece specifying the span's start and length instead of the span itself.
    let inline create start length =
        { Span = Span.createWithLength start length }

    /// Creates a Piece specifying its span.
    let inline createWithSpan span =
        { Span = span }

    /// Merges two consecutive pieces into one.
    /// Expects to be called only when Piece.isConsecutive returns true. 
    let inline merge a b =
        createWithSpan <| Span.createWithLength a.Span.Start (a.Span.Length + b.Span.Length)

    /// Split operation that returns three pieces.
    /// Correct usage of this method assumes that Piece a's span starts before and ends after Piece b's span.
    let inline split (a: PieceType) (difference: int) =
        let p1Length = a.Span.Start + difference
        let p1Span = Span.createWithStop a.Span.Start p1Length  
        let p1 = createWithSpan p1Span

        let span3 = Span.createWithStop (p1Length) (Span.stop a.Span)
        let p3 = createWithSpan span3
        (p1, p3)

    let inline deleteInRange curIndex start finish piece =
        let p1Start = piece.Span.Start
        let p1Length = start - curIndex
        let p1 = {piece with Span = Span.createWithLength p1Start p1Length}

        let p2Start = finish - curIndex + piece.Span.Start
        
        let p2Stop = Span.stop piece.Span
        let p2 = {piece with Span = Span.createWithStop p2Start p2Stop}
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