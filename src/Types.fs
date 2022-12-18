namespace PieceTable

open System.Globalization

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

    (* Start of type definitions for buffer, represented as red black tree... *)
    (* Type abbreviations. *)
    type BufferLength = int
    type InsertedLength = int
    type Key = int (* The node's index (0, 1, 2, 3, etc.). *)
    type Value = StringInfo

    (* Buffer collection as a red black tree. *)
    [<Struct>]
    type Colour = R | B

    type BufferTree = Empty | Tree of Colour * BufferTree * Key * Value * BufferTree

    (* Interface type to tree storing length as well. *)
    type BufferType = { Tree: BufferTree; Length: BufferLength }
    (* ...end of type definitions for buffer. *)

    type ListZipperType =
        { Focus: PieceType list
          Path: PieceType list
          Index: int }

    type TextTableType =
        { Buffer: BufferType
          Pieces: ListZipperType }
