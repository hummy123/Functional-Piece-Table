namespace PieceTable

open Types

module TextTable =
    /// Creates an empty TextTableType.
    let empty = {Buffer = Buffer.empty; Pieces = ListZipper.empty}

    /// Create a TextTableType given a string.
    let create str =
        if str = ""
        then empty
        else
            let buffer = Buffer.createWithString str
            let pieces = ListZipper.createWithPiece (Piece.create true 0 str.Length)
            {Buffer = buffer; Pieces = pieces}

    let text table = ListZipper.text table

    /// Consolidates a table into a buffer with only used characters and a single piece.
    /// Recommended to call this in another thread: do not use it synchronously.
    let consolidate table =
        let folder acc piece = 
            Buffer.append (Piece.text piece table) acc
        let buffer = List.fold folder Buffer.empty table.Pieces.Path
        let buffer = List.fold folder buffer table.Pieces.Focus
        let piece = Piece.createWithSpan (Span.createWithLength 0 buffer.Length)
        let zipper = {Focus = [piece]; Path = []; Index = 0 }
        {Pieces = zipper; Buffer = buffer;}

    /// Returns a new table with the string inserted.
    let insert index (str: string) (table: TextTableType) =
        let buffer = Buffer.append str table.Buffer
        let piece = Piece.create false (buffer.Length - str.Length) str.Length
        let pieces = ListZipper.insert index piece table.Pieces

        { table with
            Pieces = pieces
            Buffer = buffer }

    (* Alternative OOP API. *)
    type TextTableType with

        member this.Insert(index, str) = insert index str this
        member this.Text() = text this

        member this.Delete(start, length) =
            let span = Span.createWithLength start length
            ListZipper.delete span this

        member this.Substring(startIndex, length) =
            let span = Span.createWithLength startIndex length
            ListZipper.textSlice span this
