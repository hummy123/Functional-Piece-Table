namespace PieceTable

open Types
open PieceTree

module TextTable =
    /// Create a TextTableType given a string,
    let create str =
        let (buffer, pieces) =
            if str = ""
            then Buffer.empty, PieceTree.empty
            else 
                let buffer = Buffer.createWithString str
                let piece = Piece.create true 0 str.Length
                let pieces = PieceTree.insert 0 piece PieceTree.empty
                (buffer, pieces)

        { Buffer = buffer
          Pieces = pieces }

    let text table = 
        let folder = (fun (acc: string) (piece: PieceType) ->
            let text = (Buffer.substring piece.Span table.Buffer)
            acc + text
        )
        PieceTree.fold folder "" table.Pieces

    /// Consolidates a table into a buffer with only used characters and a single piece.
    /// Recommended to call this in another thread: do not use it synchronously.
    let consolidate (table: TextTableType) =
        let folder = (fun (acc: BufferType) (piece: PieceType) -> 
            let text = (Buffer.substring piece.Span table.Buffer)
            Buffer.append text acc
         )
        let buffer = PieceTree.fold folder Buffer.empty table.Pieces
        let piece = Piece.create false 0 buffer.Length
        let tree = PieceTree.insert 0 piece PieceTree.empty
        {Pieces = tree; Buffer = buffer;}

    /// Returns a new table with the string inserted.
    let insert index (str: string) (table: TextTableType) =
        let buffer = Buffer.append str table.Buffer
        let piece = Piece.create false 0 str.Length
        let pieces = PieceTree.insert index piece table.Pieces

        { table with
            Pieces = pieces
            Buffer = buffer }

    (* Alternative OOP API. *)
    type TextTableType with
        member this.Insert(index, str) = insert index str this
        member this.Text() = text this

