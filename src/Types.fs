namespace PieceTable

open System.Text

module Types =
    type SpanType = { Start: int; Length: int }

    type PieceType = { IsOriginal: bool; Span: SpanType }

    (* This type uses a StringBuilder, rather than a string, is the only mutable part of this implementation.
     * It is likely faster than appending a string in some scenarios, so I think the mutability is worth it. *)
    (* To do 1: Use a balanced-tree data structure to store the pieces rather than a list. *)
    (* To do 2: Cache the start index of each line (likely best to use just an array) for fast line lookup. *)
    (* To do 3: Allow reading OriginalBuffer on disk for lower memory usage. *)
    type TextTableType =
        { OriginalBuffer: string
          AddBuffer: StringBuilder
          DocumentLength: int
          Pieces: List<PieceType> }
