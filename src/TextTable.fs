namespace PieceTable

open Types
open PieceTree

module TextTable =
    let empty = { Buffer = Buffer.empty; Pieces = PieceTree.empty; Length = 0 }

    /// Create a TextTableType given a string,
    let inline create str =
        if str = ""
        then 
            empty
        else 
            let buffer = Buffer.createWithString str
            let piece = Piece.create 0 str.Length
            let pieces = PieceTree.insert 0 piece PieceTree.empty
            { Buffer=buffer; Pieces=pieces; Length=str.Length }

    let text table = PieceTree.text table

    /// Consolidates a table into a buffer with only used characters and a single piece.
    /// Recommended to call this in another thread: do not use it synchronously.
    let inline consolidate (table: TextTableType) =
        let folder = (fun (acc: BufferType) (piece: PieceType) -> 
            let text = (Buffer.substring piece.Span.Start piece.Span.Length table.Buffer)
            Buffer.append text acc
         )
        let buffer = PieceTree.fold folder Buffer.empty table.Pieces
        let piece = Piece.create 0 buffer.Length
        let tree = PieceTree.insert 0 piece PieceTree.empty
        { Pieces = tree; Buffer = buffer; Length = buffer.Length }

    /// Returns a new table with the string inserted.
    let inline insert index (str: string) (table: TextTableType) =
        let buffer = Buffer.append str table.Buffer
        let piece = Piece.create (buffer.Length - str.Length) str.Length
        let pieces = PieceTree.insert index piece table.Pieces

        { Buffer = buffer; Pieces = pieces; Length = table.Length + str.Length }

    let inline delete (startIndex: int) (length: int) (table: TextTableType) =
        let span: SpanType = Span.createWithLength startIndex length
        let newPieces: AaTree = PieceTree.delete startIndex length table.Pieces
        { Pieces = newPieces; Buffer = table.Buffer; Length = table.Length - length }

    let inline substring startIndex length table = 
        PieceTree.substring startIndex length table

    (* Alternative OOP API. *)
    type TextTableType with
        member inline this.Insert(index, str) = insert index str this
        member inline this.Text() = text this
        member inline this.Substring(start, length) = substring start length this
        member inline this.Delete(start: int, length: int) = delete start length this
