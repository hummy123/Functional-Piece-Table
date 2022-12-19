namespace PieceTable

open System.Globalization
open FSharpx.Collections

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
        | AtEndOf
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

    type BufferType = { HashMap: PersistentHashMap<int, StringInfo>; Length: int }

    type ListZipperType =
        { Focus: PieceType list
          Path: PieceType list
          Index: int }

    type TextTableType =
        { Buffer: BufferType
          Pieces: ListZipperType }
