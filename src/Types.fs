﻿namespace PieceTable

(* My benchmarks showed that using structs for small types,
 * and for discriminated unions where applicable was slightly faster. *)
module Types =
    [<Struct>]
    type SpanType = { Start: int; Length: int }

    [<Struct>]
    type PieceType = { Span: SpanType }

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

    [<Struct>]
    type Colour = R | B

    type Tree = 
        | E 
        | T of Colour * SizeLeft * Tree * Value * SizeRight * Tree
        
    type Set = int * Tree
    (* ...Tree definitions. *)

    type TextTableType =
        { Buffer: string
          Pieces: ListZipperType
        }
