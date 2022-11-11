namespace PieceTable

open Types
open System.Text

module TextTable =
    let create str =
        { OriginalBuffer = str
          AddBuffer = StringBuilder()
          DocumentLength = str.Length
          Pieces = if str = "" then [] else [ Piece.create true 0 str.Length ] }

    let text (table: TextTableType) =
        let rec buildText index (acc: StringBuilder) =
            if index = acc.Length then
                acc
            else
                let piece = table.Pieces[index]

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

    let insert (str: string) index (table: TextTableType) =
        let rec insertAtMiddle (curIndex: int) (listPos: int) (insPiece: PieceType) (accList: List<PieceType>) =
            if listPos = table.Pieces.Length then
                accList
            else
                let curPiece = table.Pieces[listPos]

                if curIndex = curPiece.Span.Start then
                    accList @ [ insPiece ] @ table.Pieces[listPos..]
                elif curIndex = Span.stop curPiece.Span then
                    accList @ [ curPiece ] @ [ insPiece ] @ table.Pieces[listPos + 1 ..]
                (* In range: split piece. *)
                elif curIndex >= curPiece.Span.Start && curIndex <= Span.stop curPiece.Span then
                    let splitData = Piece.split curPiece insPiece

                    let pieces =
                        match splitData with
                        | Piece.Merge p -> [ p ]
                        | Piece.Split (f, s, t) -> [ f; s; t ]

                    if listPos = table.Pieces.Length - 1 then
                        accList @ pieces
                    else
                        accList @ pieces @ table.Pieces[listPos + 1 ..]
                else
                    insertAtMiddle (curIndex + curPiece.Span.Length) (listPos + 1) insPiece (accList @ [ curPiece ])

        let newLength = table.DocumentLength + str.Length
        let appendBuffer = table.AddBuffer.Append(str)
        let piece = Piece.create false index (str.Length - 1)

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
