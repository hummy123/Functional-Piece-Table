namespace PieceTable

open Types

module internal Piece =
    /// Creates a Piece specifying the span's start and length instead of the span itself.
    let create isOriginal start length =
        { Span = Span.createWithLength start length }

    /// Creates a Piece specifying its span.
    let createWithSpan span =
        { Span = span }

    /// Checks if two Pieces are consecutive (from the same buffer
    /// and one stopping where the other starts).
    /// Can be used to merge pieces for memory efficiency.
    let isConsecutive a b =
        let aStop = Span.stop a.Span
        let bStop = Span.stop b.Span
        if aStop > bStop then
            if aStop + 1 <= bStop then
                true
            else 
                false
        elif bStop + 1 <= aStop then
            true
        else false

    /// Merges two consecutive pieces into one.
    /// If Piece.isConsecutive returns false with these two pieces, this method throws an error.
    let merge a b =
        match isConsecutive a b with
        | false -> failwith "Piece.merge caller error: Tried to merge non-consecutive pieces."
        | true ->
            let mergedStart = 
                if a.Span.Start < b.Span.Start 
                then a.Span.Start
                else b.Span.Start

            let fStop = Span.stop a.Span
            let pStop = Span.stop b.Span
            let mergeStop =
                if pStop > fStop
                then pStop
                else fStop

            let mergeSpan = Span.createWithStop mergedStart mergeStop
            createWithSpan mergeSpan

    /// Split operation that returns three pieces.
    /// Correct usage of this method assumes that Piece a's span starts before and ends after Piece b's span.
    let split (a: PieceType) (b: PieceType) (difference: int) =
        let p1Length = a.Span.Start + difference
        let p1Span = Span.createWithLength a.Span.Start p1Length
        let p1 = createWithSpan p1Span

        let span3 = Span.createWithStop p1Length (Span.stop a.Span)
        let p3 = createWithSpan span3
        (p1, b, p3)

    /// Given a search index (for example the index we want to insert at), 
    /// a current index (keeping track of current index in a loop) 
    /// and a piece, returns a DU telling us where we are.
    let compareWithIndex searchIndex curIndex curPiece =
        if searchIndex = curIndex then
            EqualTo
        elif searchIndex >= curIndex && searchIndex <= curIndex + curPiece.Span.Length then
            InRangeOf
        elif searchIndex < curIndex then
            LessThanIndex
        else
            GreaterThanIndex

    let compareWithSpan (span: SpanType) curIndex curPiece =
        let spanStop = Span.stop span
        let pieceStop = curIndex + curPiece.Span.Length

        if span.Start <= curIndex && spanStop >= pieceStop then
            PieceFullyInSpan
        elif span.Start <= curIndex && spanStop < pieceStop then
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

    let private deleteInRange curIndex span piece =
        let spanStop = Span.stop span 

        let p1Start = piece.Span.Start
        let p1Length = span.Start - curIndex
        let p1 = {piece with Span = Span.createWithLength p1Start p1Length}

        let p2Start = spanStop - curIndex + piece.Span.Start
        
        let p2Stop = Span.stop piece.Span
        let p2 = {piece with Span = Span.createWithStop p2Start p2Stop}
        CutTwo(p1, p2, span.Length)

    let private deleteAtStart curIndex span piece =
        let spanStop = Span.stop span
        let newPieceStart = piece.Span.Start + (spanStop - curIndex)
        let newPieceSpan = Span.createWithStop newPieceStart (Span.stop piece.Span)
        let difference = piece.Span.Length - newPieceSpan.Length
        CutOne({ piece with Span = newPieceSpan }, difference)

    let private deleteAtEnd curIndex span piece =
        let newLength = span.Start - curIndex
        let newSpan = Span.createWithLength piece.Span.Start newLength
        let difference = piece.Span.Length - newSpan.Length
        CutOne({ piece with Span = newSpan }, difference)

    /// Deletes either a part of a piece or a full piece itself.
    /// See the documentation for the DeletedPiece type on how to use this method's return value.
    let delete pos curIndex (span: SpanType) (piece: PieceType) =        
        match pos with
        | PieceFullyInSpan -> DeletedPiece.Empty
        | SpanWithinPiece -> deleteInRange curIndex span piece
        | StartOfPieceInSpan -> deleteAtStart curIndex span piece
        | EndOfPieceInSpan -> deleteAtEnd curIndex span piece
        | _ -> failwith "Piece.delete error"

    let text piece table =
        Buffer.substring piece.Span table.Buffer

    let textSlice startIndex length piece table =
        let pieceText = text piece table        
        let pieceLength = table.Pieces.Focus[0].Span.Length

        match startIndex, length, table.Pieces.Index, pieceLength with
        (* If Piece fits within range (start index + length). *)
        | curStart, curLen, pieceStart, pieceLen when curStart = pieceStart && curLen >= pieceLen ->
            pieceText
        (* If Piece does not fit within range. *)
        | curStart, curLen, pieceStart, pieceLen ->
            let sliceStart = curStart - pieceStart
            let remainingLength = pieceText.Length - sliceStart
            let sliceStop = if remainingLength < curLen then remainingLength else curLen
            pieceText.Substring(sliceStart, sliceStop)
