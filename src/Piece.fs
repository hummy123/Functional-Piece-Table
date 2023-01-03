namespace PieceTable

open Types

module internal Piece =
    (* Make some members more concise to access. *)
    type PieceType with
        member this.Start = this.Span.Start
        member this.Length = this.Span.Length
        member this.Finish = this.Span.Start + this.Span.Length

    /// Creates a Piece specifying the span's start and length instead of the span itself.
    let create start length lines =
        { Span = Span.createWithLength start length; Lines = lines }

    /// Creates a Piece specifying its span.
    let createWithSpan span lines =
        { Span = span; Lines = lines }

    /// Merges two consecutive pieces into one.
    /// Expects to be called only when Piece.isConsecutive returns true. 
    let inline merge (a: PieceType) (b: PieceType) =
        let span = Span.createWithLength a.Start (a.Length + b.Length)
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
        let p1Length = a.Start + difference
        let p1Span = Span.createWithStop a.Start p1Length
        let p1Lines = Array.filter (fun v -> v < difference) a.Lines
        let p1 = createWithSpan p1Span p1Lines

        let span3 = Span.createWithStop (p1Length) a.Finish
        let p3Lines = Array.filter (fun v -> v >= difference) a.Lines
        let p3 = createWithSpan span3 p3Lines
        (p1, p3)

    let inline deleteInRange curIndex start finish (piece: PieceType) =
        let p1Start = piece.Start
        let p1Length = start - curIndex
        let p1Span = Span.createWithLength p1Start p1Length
        let p1Lines = Array.filter (fun p -> p < p1Length) piece.Lines
        let p1 = createWithSpan p1Span p1Lines

        let p2Start = finish - curIndex + piece.Start
        let p2Span = Span.createWithStop p2Start piece.Finish
        let p2Lines = Array.filter (fun p -> p >= p2Start) piece.Lines
        let p2 = createWithSpan p2Span p2Lines
        (p1, p2)

    let inline deleteAtStart curIndex finish (piece: PieceType) =
        let difference = finish - curIndex
        let newStart = piece.Start + difference
        let newSpan = Span.createWithStop newStart piece.Finish
        let newLines = Array.filter (fun i -> i > difference) piece.Lines
        createWithSpan newSpan newLines

    let inline deleteAtEnd curIndex start (piece: PieceType) =
        let newLength = start - curIndex
        let newSpan = Span.createWithLength piece.Start newLength
        let newLines = Array.filter (fun i -> i < newLength) piece.Lines
        createWithSpan newSpan newLines

    let inline text (piece: PieceType) table =
        Buffer.substring piece.Start piece.Length table.Buffer

    let inline textInRange curIndex start finish (piece: PieceType) table =
        let textStart = start - curIndex + piece.Start
        let textLength = finish - curIndex + piece.Start - textStart
        Buffer.substring textStart textLength table.Buffer

    let inline textAtStart curIndex finish (piece: PieceType) table =
        let textStop = piece.Start + (finish - curIndex)
        let substrSpan = Span.createWithStop piece.Start textStop
        Buffer.substring piece.Start substrSpan.Length table.Buffer

    let inline textAtEnd curIndex start (piece: PieceType) table =
        let textStart = start - curIndex + piece.Start
        Buffer.substring textStart (piece.Finish - textStart) table.Buffer
