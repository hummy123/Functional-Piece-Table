namespace PieceTable


module Types =
    type SpanType = { Start: int; Length: int }

    type PieceType = { IsOriginal: bool; Span: SpanType }

    type TextTableType =
        { OriginalBuffer: string
          AddBuffer: string
          DocumentLength: int
          Pieces: List<PieceType> }
