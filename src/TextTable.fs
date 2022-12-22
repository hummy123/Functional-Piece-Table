namespace PieceTable

open Types
open System

module TextTable =
    /// Creates an empty TextTableType.
    let empty = {Buffer = Buffer.empty; Pieces = ListZipper.empty}

    /// Checks if a TextTableType can be consolidated.
    let isConsolidated table =
        table.Pieces.Focus.Length + table.Pieces.Path.Length > 1

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
        let buffer = List.fold folder Buffer.empty (List.rev table.Pieces.Path)
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

    /// Find the first occurrence of a string in the table  returns an int representing the index.
    /// Returns -1 if the given string was not found.
    let indexOf (str: string) (table: TextTableType) =
        let rec loop curPos =
            if curPos >= table.Buffer.Length then
                -1
            else
                let searchLength = 
                    if curPos + Buffer.MaxBufferLength < table.Buffer.Length
                    then Buffer.MaxBufferLength
                    else table.Buffer.Length - curPos
                let searchSpan = Span.createWithLength curPos searchLength
                let searchText = ListZipper.textSlice searchSpan table
                let isFound = searchText.IndexOf(str, StringComparison.OrdinalIgnoreCase)
                if isFound >= 0
                then isFound + curPos
                else 
                    (* We start oour next search at the last occurrence 
                     * in this string, of the first letter in the search string
                     * if possible; else start search at next node. *)
                    let lastPosOfFirstChar = searchText.LastIndexOf(str[0].ToString(), StringComparison.OrdinalIgnoreCase)
                    if lastPosOfFirstChar > -1
                    then loop lastPosOfFirstChar
                    else loop (curPos + Buffer.MaxBufferLength) 

        if table.Buffer.Length <= Buffer.MaxBufferLength
        then (text table).IndexOf(str, StringComparison.OrdinalIgnoreCase)
        else loop 0

    /// Find the last occurrence of a string from the table and returns an int representing the index.
    /// Returns -1 if the given string was not found.
    let lastIndexOf (str: string) (table: TextTableType) = 
        let rec loop curPos = 
            if curPos < 0 
            then -1
            elif curPos = 0
            then str[..0].LastIndexOf(str)
            else
                let searchStartPos = 
                    if curPos >= Buffer.MaxBufferLength
                    then curPos - Buffer.MaxBufferLength
                    else 0
                let searchSpan = Span.createWithLength searchStartPos Buffer.MaxBufferLength
                let searchText = ListZipper.textSlice searchSpan table
                let isFound = searchText.LastIndexOf(str, StringComparison.OrdinalIgnoreCase)
                if isFound >= 0
                then isFound
                else 
                    let firstPosOfLastChar = searchText.IndexOf(str[str.Length - 1], StringComparison.OrdinalIgnoreCase)
                    if firstPosOfLastChar > -1
                    then loop firstPosOfLastChar
                    else loop searchStartPos

        if table.Buffer.Length <= Buffer.MaxBufferLength
        then (text table).LastIndexOf(str)
        else loop (table.Buffer.Length - 1)

    /// Finds all occurrences of a string in the table and returns a list containing each index in order.
    /// Returns an empty list if the given string was not found.
    let allIndexesOf (str: string) (table: TextTableType) =
        let rec loop curPos acc =
            if curPos < 0
            then acc
            else
                let searchStartPos = 
                    if curPos >= Buffer.MaxBufferLength
                    then curPos - Buffer.MaxBufferLength
                    else 0
                let searchSpan = Span.createWithLength searchStartPos Buffer.MaxBufferLength
                let searchText = ListZipper.textSlice searchSpan table
                let isFound = searchText.LastIndexOf(str, StringComparison.OrdinalIgnoreCase)
                if isFound >= 0
                then loop isFound (isFound :: acc)
                else loop searchStartPos acc

        loop (table.Buffer.Length - 1) []

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

        member this.IndexOf(str) = indexOf str this
        member this.LastIndexOf(str) = lastIndexOf str this
        member this.AllIndexesOf(str) = allIndexesOf str this