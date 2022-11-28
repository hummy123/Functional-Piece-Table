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
    type Length = int
    type InsIndex = int

    type Tree = 
        | E 
        | T of SizeLeft * Tree * Value * SizeRight * Tree * Length

    type Ctx =
      | Top
      | Fst of Ctx * SizeLeft * Value * Tree
      | Snd of Tree * Value * SizeRight * Ctx
    (* ...Tree definitions. *)

    type TextTableType =
        { OriginalBuffer: string
          AddBuffer: string
          Pieces: ListZipperType
          Tree: Tree
        }
