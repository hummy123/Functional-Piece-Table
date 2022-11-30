namespace PieceTable


module Types =
    [<Struct>]
    type SpanType = { Start: int; Length: int }

    [<Struct>]
    type PieceType = { IsOriginal: bool; Span: SpanType }

    [<Struct>]
    type CompareIndex = 
        | EqualTo
        | InRangeOf
        | LessThanIndex
        | GreaterThanIndex

    [<Struct>]
    type CompareSpan =
        | StartOfPieceInSpan
        | EndOfPieceInSpan
        | PieceFullyInSpan
        | SpanWithinPiece
        | LessThanSpan
        | GreaterThanSpan

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

    type Colour = R | B
    type Tree = 
        | E 
        | T of Colour * SizeLeft * Tree * Value * SizeRight * Tree
        
    type Set = int * Tree
    (* ...Tree definitions. *)

    type TextTableType =
        { OriginalBuffer: string
          AddBuffer: string
          Pieces: ListZipperType
        }
