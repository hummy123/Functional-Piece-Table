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

    /// Retrieve the string contained within a TextTableType. Building this string takes O(n).
    /// Note that .NET has a size limit of 2 GB on objects and thus you cannot retrieve a string longer than that.
    let text (table: TextTableType) =
        let pieces = ListZipper.ofList table.Pieces

        let rec buildText listPos (acc: string) =
            let piece = pieces[listPos]

            if listPos = pieces.Length - 1 then
                match piece.IsOriginal with
                (* Get text from original buffer. *)
                | true -> acc + table.OriginalBuffer.Substring(piece.Span.Start, piece.Span.Length)
                (* Get text from add buffer. *)
                | false -> acc + table.AddBuffer.Substring(piece.Span.Start, piece.Span.Length)

            else
                match piece.IsOriginal with
                (* Get text from original buffer. *)
                | true ->
                    acc + table.OriginalBuffer.Substring(piece.Span.Start, piece.Span.Length)
                    |> buildText (listPos + 1)
                (* Get text from add buffer. *)
                | false ->
                    acc + table.AddBuffer.Substring(piece.Span.Start, piece.Span.Length)
                    |> buildText (listPos + 1)

        buildText 0 ""

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
