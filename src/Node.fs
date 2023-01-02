namespace PieceTable

open Types

module internal Node =
    (* Just make various attributes more concise to access. *)
    type NodeType with
        member inline this.LeftSize = this.Index.LeftSize
        member inline this.RightSize = this.Index.RightSize
        member inline this.Start = this.Piece.Span.Start
        member inline this.Length = this.Piece.Span.Length
        member inline this.SetPiece piece = { this with Piece = piece }
        member inline this.SetIndex index = { this with Index = index }