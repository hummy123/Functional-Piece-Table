namespace PieceTable

open Types

module internal Piece =
    /// Creates a Piece specifying the buffer it belongs to, the index of the buffer it should read from and how many characters it should read.
    let create isOriginal start length =
        { IsOriginal = isOriginal
          Span = Span.createWithLength start length }

    /// Creates a Piece specifying which buffer it belongs to and its span.
    let createWithSpan isOriginal span =
        { IsOriginal = isOriginal; Span = span }

    /// Split operation that returns three pieces.
    /// Correct usage of this method assumes that Piece a's span starts before and ends after Piece b's span.
    let split (a: PieceType) (b: PieceType) (difference: int) =
        let p1Length = a.Span.Start + difference
        let p1Span = Span.createWithLength a.Span.Start p1Length
        let p1 = createWithSpan a.IsOriginal p1Span

        let span3 = Span.createWithStop p1Length (Span.stop a.Span)
        let p3 = createWithSpan a.IsOriginal span3
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
        elif span.Start >= curIndex && spanStop <= pieceStop then
            SpanWithinPiece
        elif span.Start <= curIndex && spanStop < pieceStop then
            StartOfPieceInSpan
        elif span.Start > curIndex && spanStop >= pieceStop then
            EndOfPieceInSpan
        elif spanStop > pieceStop then
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

    let private deleteInRange curIndex span spanStop piece =
        let p1Stop = span.Start - piece.Span.Start
        let p1 = { piece with Span = Span.createWithStop piece.Span.Start p1Stop }
        let p2Start = if curIndex < spanStop then spanStop - curIndex else spanStop
        let p2Stop = Span.stop piece.Span
        let p2 = { piece with Span = Span.createWithStop p2Start p2Stop }
        let difference = piece.Span.Length - (p1.Span.Length + p2.Span.Length)
        CutTwo(p1, p2, difference)

    let private deleteAtStart curIndex spanStop piece =
        let newPieceStart = if spanStop < curIndex then spanStop else spanStop - curIndex
        let newPieceSpan = Span.createWithStop newPieceStart (Span.stop piece.Span)
        let difference = piece.Span.Length - newPieceSpan.Length
        CutOne({ piece with Span = newPieceSpan }, difference)

    let private deleteAtEnd span piece =
        let newPieceSpan = Span.createWithStop piece.Span.Start span.Start
        let difference = piece.Span.Length - newPieceSpan.Length
        CutOne({ piece with Span = newPieceSpan }, difference)

    /// Deletes either a part of a piece or a full piece itself.
    /// See the documentation for the DeletedPiece type on how to use this method's return value.
    let delete curIndex (span: SpanType) (piece: PieceType) =
        let spanStop = Span.stop span
        
        match compareWithSpan span curIndex piece with
        | PieceFullyInSpan -> DeletedPiece.Empty
        | SpanWithinPiece -> deleteInRange curIndex span spanStop piece
        | StartOfPieceInSpan -> deleteAtStart curIndex spanStop piece
        | EndOfPieceInSpan -> deleteAtEnd span piece
        | _ -> failwith "Piece.delete error"

    let text piece table =
        match piece.IsOriginal with
        (* Get text from original buffer. *)
        | true -> table.OriginalBuffer.Substring(piece.Span.Start, piece.Span.Length)
        (* Get text from add buffer. *)
        | false -> table.AddBuffer.Substring(piece.Span.Start, piece.Span.Length)

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
