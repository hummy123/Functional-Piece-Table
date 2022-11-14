namespace PieceTable

open Types

module internal Piece =
    let create isOriginal start length =
        { IsOriginal = isOriginal
          Span = Span.createWithLength start length }

    let createWithSpan isOriginal span =
        { IsOriginal = isOriginal; Span = span }


    type SplitPieces =
        | Merge of PieceType
        | Split of PieceType * PieceType * PieceType

    let split (a: PieceType) (b: PieceType) (difference: int) =
        match a.IsOriginal = b.IsOriginal with
        (* Just merge the two pieces into one. *)
        | true ->
            let p = createWithSpan a.IsOriginal (Span.union a.Span b.Span)
            Merge(p)
        (* Split into three pieces. *)
        | false ->
            let p1Length = a.Span.Start + difference
            let p1Span = Span.createWithLength a.Span.Start p1Length
            let p1 = createWithSpan a.IsOriginal p1Span

            let span3 = Span.createWithStop p1Length (Span.stop a.Span)
            let p3 = createWithSpan a.IsOriginal span3
            Split(p1, b, p3)

    type DeletedPiece =
        | Empty
        | CutOne of PieceType
        | CutTwo of PieceType * PieceType

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
            CutOne({ piece with Span = newPieceSpan })
        (* Some part after the piece's start is within the deletion span's range. *)
        (* Example: abcd|ef|. *)
        elif spanStop >= Span.stop piece.Span then
            let newPieceStop = span.Start
            let newPieceSpan = Span.createWithStop piece.Span.Start newPieceStop
            CutOne({ piece with Span = newPieceSpan })
        (* The deletion span specifies a part within the piece but not its full range.*)
        (* Example: a|bc|def. *)
        else
            let p1Stop = span.Start - piece.Span.Start
            let p1 = { piece with Span = Span.createWithStop piece.Span.Start p1Stop }
            let p2Stop = Span.stop piece.Span
            let p2 = { piece with Span = Span.createWithStop spanStop p2Stop }
            CutTwo(p1, p2)
