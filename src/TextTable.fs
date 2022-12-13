namespace PieceTable

open Types

module TextTable =
    /// Create a TextTableType given a string.
    let create str =
        let (buffer, pieces) =
            if str = ""
            then Buffer.empty, ListZipper.empty
            else 
                let buffer = Buffer.createWithString str
                let pieces = ListZipper.createWithPiece (Piece.create true 0 str.Length)
                (buffer, pieces)

        { Buffer = buffer
          Pieces = pieces }

    let text table = ListZipper.text table

    /// Consolidates a table into a buffer with only used characters and a single piece.
    /// Recommended to call this in another thread: do not use it synchronously.
    let consolidate table =
        let oldPieces = ListZipper.ofList table.Pieces
        let buffer = List.fold (fun acc piece -> Buffer.append (Piece.text piece table) acc ) Buffer.empty oldPieces
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
