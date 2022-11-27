namespace PieceTable


module Types =
    type SpanType = { Start: int; Length: int }

    type PieceType = { IsOriginal: bool; Span: SpanType }

    type ListZipperType =
        { Focus: PieceType list
          Path: PieceType list
          Index: int }

    (* Tree defintions... *)
    type SizeLeft = int
    type SizeRight = int
    type Value = PieceType

    type Colour = R | B

    type Tree = 
        | E 
        | T of Colour * SizeLeft * Tree * Value * SizeRight * Tree
    (* ...Tree definitions. *)

    type TextTableType =
        { OriginalBuffer: string
          AddBuffer: string
          Pieces: ListZipperType
          Tree: Tree
        }
