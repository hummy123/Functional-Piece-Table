namespace PieceTable

open Types
open Index

module internal Node =
    let inline create piece index = {Piece = piece; Index = index}
    let inline newPiece piece = {Index = Index.empty; Piece = piece}
    (* Just make various attributes more concise to access. *)
    type NodeType with
        member inline this.LeftSize = this.Index.LeftSize
        member inline this.RightSize = this.Index.RightSize
        member inline this.Start = this.Piece.Span.Start
        member inline this.Length = this.Piece.Span.Length
        member inline this.WithPiece piece = { this with Piece = piece }
        member inline this.WithIndex index = { this with Index = index }