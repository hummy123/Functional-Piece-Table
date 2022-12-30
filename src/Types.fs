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
<<<<<<< HEAD
=======
    type CompareIndex = 
        | EqualTo
        | InRangeOf
        | AtEndOf
        | LessThanIndex
        | GreaterThanIndex

    [<Struct>]
>>>>>>> main
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
    type Value = UnicodeStringType

    (* Buffer collection as an AA Tree. *)
    type Height = int

    type BufferTree =
<<<<<<< HEAD
        | BE 
        | BT of Height * BufferTree * Key * Value * BufferTree
=======
        | E 
        | T of Height * BufferTree * Key * UnicodeStringType * BufferTree
>>>>>>> main

    (* Interface type to tree storing length as well. *)
    type BufferType = { Tree: BufferTree; Length: BufferLength }
    (* ...end of type definitions for buffer. *)

<<<<<<< HEAD
    (* Piece tree as an AA tree. *)
    type SizeLeft = int
    type SizeRight = int

    type AaTree = 
        | PE
        | PT of int * SizeLeft * AaTree * PieceType * SizeRight * AaTree
    (* ...end of type definitions for piece tree. *)

    type TextTableType =
        { Buffer: BufferType
          Pieces: AaTree
          Length: int }
=======
    type ListZipperType =
        { Focus: PieceType list
          Path: PieceType list
          Index: int }

    type TextTableType =
        { Buffer: BufferType
          Pieces: ListZipperType }
>>>>>>> main
