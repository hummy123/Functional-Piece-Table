namespace PieceTable

open UnicodeString

(* My benchmarks showed that using structs for small types,
 * and for discriminated unions where applicable was slightly faster. *)
module Types =
    [<Struct>]
    type SpanType = { Start: int; Length: int }

    [<Struct>]
    type PieceType = { Span: SpanType }

    [<Struct>]
    type IndexType = { LeftSize: int; RightSize: int }

    [<Struct>]
    type LineType = { LeftLines: int; RightLines: int; PieceLines: int }

    (* Start of type definitions for buffer, represented as red black tree... *)
    (* Type abbreviations. *)
    type BufferLength = int
    type InsertedLength = int
    type Key = int (* The node's index (0, 1, 2, 3, etc.). *)
    type Value = UnicodeStringType

    (* Buffer collection as an AA Tree. *)
    type Height = int

    type BufferTree =
        | BE 
        | BT of Height * BufferTree * Key * Value * BufferTree

    (* Interface type to tree storing length as well. *)
    type BufferType = { Tree: BufferTree; Length: BufferLength }
    (* ...end of type definitions for buffer. *)

    (* Piece tree as an AA tree. *)
    type AaTree = 
        | PE
        | PT of AaTree * IndexType * PieceType * AaTree * int
    (* ...end of type definitions for piece tree. *)

    type TextTableType =
        { Buffer: BufferType
          Pieces: AaTree
          Length: int }
