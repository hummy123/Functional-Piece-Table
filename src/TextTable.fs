namespace PieceTable

open Types

module TextTable =
    /// Create a TextTableType given a string,
    let create str =
        { OriginalBuffer = str
          AddBuffer = ""
          Pieces =
            if str = "" then
                ListZipper.empty
            else
                ListZipper.createWithPiece (Piece.create true 0 str.Length) }

    let text table = ListZipper.text table

    /// Returns a new table with the string inserted.
    let insert index (str: string) (table: TextTableType) =
        let addBuffer = table.AddBuffer + str
        let piece = Piece.create false (addBuffer.Length - str.Length) str.Length
        let pieces = ListZipper.insert index piece table.Pieces

        { table with
            Pieces = pieces
            AddBuffer = addBuffer }

    (* Alternative OOP API. *)
    type TextTableType with

        member this.Insert(index, str) = insert index str this
        member this.Text() = text this

        member this.Delete(start, length) =
            let span = Span.createWithLength start length
            ListZipper.delete span this
