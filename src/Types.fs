namespace PieceTable

open UnicodeString
open System
open System.Runtime.CompilerServices

(* My benchmarks showed that using structs for small types,
 * and for discriminated unions where applicable was slightly faster. *)
module Types =
    [<Struct>]
    type SpanType = { Start: int; Length: int }

    [<Struct>]
    type PieceType = { Span: SpanType; Lines: int array }

    [<Struct>]
    type IndexType = { Left: int; Right: int }

    [<Struct>]
    type LineType = { Left: int; Right: int }

    [<Struct>]
    type PieceNode = { Piece: PieceType; Index: IndexType; Lines: LineType; }

    [<Struct>]
    type BufferNode = { Key: int; Value: UnicodeStringType }

    [<Struct>]
    type NodeType<'a> =
        | Piece of p:PieceNode
        | Buffer of b:BufferNode
        | Other of 'a

    type AaTree<'a> = 
        | E
        | T of int * AaTree<'a> * NodeType<'a> * AaTree<'a>

    (* Type abbreviations for more concise referral. *)
    type BufferTree = AaTree<BufferNode>
    type PieceTree = AaTree<PieceNode>

    type BufferType = { Tree: AaTree<BufferNode>; Length: int }
    
    type TextTableType =
        { Buffer: BufferType
          Pieces: PieceTree
          Length: int }
