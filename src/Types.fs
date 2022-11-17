namespace PieceTable


module Types =
    type SpanType = { Start: int; Length: int }

    type PieceType = { IsOriginal: bool; Span: SpanType }

    type ListZipperType =
        { Focus: PieceType list
          Path: PieceType list
          Index: int }

    type TextTableType =
        { OriginalBuffer: string
          AddBuffer: string
          Pieces: ListZipperType }
