namespace PieceTable

open Types
open Piece
open System.Text

module TextTable =
    let create str =
        { OriginalBuffer = str
          AddBuffer = StringBuilder()
          DocumentLength = str.Length
          Pieces = if str = "" then [] else [ Piece.create true 0 str.Length ] }

    let text (table: TextTableType) =
        let rec buildText index (acc: StringBuilder) =
            let piece = table.Pieces[index]

            if index = table.Pieces.Length - 1 then
                match piece.IsOriginal with
                (* Get text from original buffer. *)
                | true -> acc.Append(table.OriginalBuffer.Substring(piece.Span.Start, piece.Span.Length))
                (* Get text from add buffer. *)
                | false -> acc.Append(table.AddBuffer.ToString(piece.Span.Start, piece.Span.Length))

            else
                match piece.IsOriginal with
                (* Get text from original buffer. *)
                | true ->
                    acc.Append(table.OriginalBuffer.Substring(piece.Span.Start, piece.Span.Length))
                    |> buildText (index + 1)
                (* Get text from add buffer. *)
                | false ->
                    acc.Append(table.AddBuffer.ToString(piece.Span.Start, piece.Span.Length))
                    |> buildText (index + 1)

        let sb = buildText 0 (StringBuilder())
        sb.ToString()

    let insert index (str: string) (table: TextTableType) =
        let rec insertAtMiddle (curIndex: int) (listPos: int) (insPiece: PieceType) (accList: List<PieceType>) =
            if listPos = table.Pieces.Length then
                accList
            else
                let curPiece = table.Pieces[listPos]
                let pieceEnd = curIndex + curPiece.Span.Length

                if index = curIndex then
                    accList @ [ insPiece ] @ table.Pieces[listPos..]
                elif index = pieceEnd then
                    accList @ [ curPiece ] @ [ insPiece ] @ table.Pieces[listPos + 1 ..]
                (* In range: split piece. *)
                elif index >= curIndex && index <= pieceEnd then
                    let difference = index - curIndex
                    let (p1, p2, p3) = Piece.split curPiece insPiece difference

                    if listPos = table.Pieces.Length - 1 then
                        accList @ [ p1; p2; p3 ]
                    else
                        accList @ [ p1; p2; p3 ] @ table.Pieces[listPos + 1 ..]
                else
                    insertAtMiddle (curIndex + curPiece.Span.Length) (listPos + 1) insPiece (accList @ [ curPiece ])

        let newLength = table.DocumentLength + str.Length
        let appendBuffer = table.AddBuffer.Append(str)
        let piece = Piece.create false (appendBuffer.Length - str.Length) str.Length

        if index = table.DocumentLength then
            let pieceList = table.Pieces @ [ piece ]

            { table with
                DocumentLength = newLength
                AddBuffer = appendBuffer
                Pieces = pieceList }

        elif index = 0 then
            let pieceList = piece :: table.Pieces

            { table with
                DocumentLength = newLength
                AddBuffer = appendBuffer
                Pieces = pieceList }
        else
            let pieceList = insertAtMiddle 0 0 piece []

            { table with
                DocumentLength = newLength
                AddBuffer = appendBuffer
                Pieces = pieceList }

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

    (* Alternative OOP API. *)
    type TextTableType with

        member this.Insert(index, str) = insert index str this
        member this.Text() = text this

        member this.Delete(start, length) =
            let span = Span.createWithLength start length
            delete span this
