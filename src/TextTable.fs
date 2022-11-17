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

    (*    /// Returns a table without the text in the specified span.
    let delete span (table: TextTableType) =
        let rec deleteWithinRange curIndex listPos accList : List<PieceType> =
            match listPos = table.Pieces.Length with
            | true -> accList
            | false ->
                let curPiece = table.Pieces[listPos]
                let curSpan = curPiece.Span
                let curSpanStop = Span.stop curSpan

                if curIndex >= curSpan.Start && curIndex <= curSpanStop then
                    (* Deletion range includes piece.. *)
                    let nextLength = curIndex + curPiece.Span.Length
                    let nextPos = listPos + 1

                    let deleteData = Piece.delete span curPiece

                    match deleteData with
                    | Empty -> deleteWithinRange nextLength nextPos accList
                    | CutOne p -> deleteWithinRange nextLength nextPos (accList @ [ p ])
                    | CutTwo (p1, p2) -> deleteWithinRange nextLength nextPos (accList @ [ p1; p2 ])
                elif curIndex > curSpanStop then
                    (* Deletion range is after piece. *)
                    accList @ table.Pieces[listPos..]
                else
                    (* Deletion range is before piece. *)
                    deleteWithinRange (curIndex + curPiece.Span.Length) (listPos + 1) (accList @ [ curPiece ])

        match Span.isEmpty span with
        | true -> table
        | false ->
            let pieceList = deleteWithinRange 0 0 []

            { table with
                DocumentLength = table.DocumentLength - span.Length
                Pieces = pieceList }
            *)

    (* Alternative OOP API. *)
    type TextTableType with

        member this.Insert(index, str) = insert index str this
        member this.Text() = text this
(*
        member this.Delete(start, length) =
            let span = Span.createWithLength start length
            delete span this
            *)
