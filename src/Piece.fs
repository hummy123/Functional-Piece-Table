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

    /// Specifies how we should handle a Piece given to the delete method.
    /// Empty: We can simply remove this Piece from the list.
    /// CutOne: We can replace the Piece with the newly returned one.
    /// CutTwo: We can replace the Piece given in the parameter with the two pieces returned, where the first is before the second.
    type DeletedPiece =
        | Empty
        | CutOne of PieceType * int
        | CutTwo of PieceType * PieceType * int

    /// Deletes either a part of a piece or a full piece itself.
    /// See the documentation for the DeletedPiece type on how to use this method's return value.
    let delete (span: SpanType) (piece: PieceType) =
        let spanStop = Span.stop span
        (* This piece is fully within the deletion span's range. *)
        (* Example: |abcdef|. *)
        if span.Start <= piece.Span.Start && spanStop >= Span.stop piece.Span then
            DeletedPiece.Empty
        (* The start of this piece is within the deletion span's range but some part at the end isn't. *)
        (* Example: |ab|cdef. *)
        elif span.Start <= piece.Span.Start then
            let newPieceStart = Span.stop span
            let newPieceSpan = Span.createWithStop newPieceStart (Span.stop piece.Span)
            let difference = piece.Span.Length - newPieceSpan.Length
            CutOne({ piece with Span = newPieceSpan }, difference)
        (* Some part after the piece's start is within the deletion span's range. *)
        (* Example: abcd|ef|. *)
        elif spanStop >= Span.stop piece.Span then
            let newPieceStop = span.Start
            let newPieceSpan = Span.createWithStop piece.Span.Start newPieceStop
            let difference = piece.Span.Length - newPieceSpan.Length
            CutOne({ piece with Span = newPieceSpan }, difference)
        (* The deletion span specifies a part within the piece but not its full range.*)
        (* Example: a|bc|def. *)
        else
            let p1Stop = span.Start - piece.Span.Start
            let p1 = { piece with Span = Span.createWithStop piece.Span.Start p1Stop }
            let p2Stop = Span.stop piece.Span
            let p2 = { piece with Span = Span.createWithStop spanStop p2Stop }
            let difference = piece.Span.Length - (p1.Span.Length + p2.Span.Length)
            CutTwo(p1, p2, difference)
