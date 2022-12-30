namespace PieceTable

open Types
open System
open System.IO

module TextTable =
<<<<<<< HEAD
    let empty = { Buffer = Buffer.empty; Pieces = PieceTree.empty; Length = 0 }

    /// Create a TextTableType given a string,
    let create str =
        if str = ""
        then 
            empty
        else 
            let buffer = Buffer.createWithString str
            let piece = Piece.create true 0 str.Length
            let pieces = PieceTree.insert 0 piece PieceTree.empty
            { Buffer=buffer; Pieces=pieces; Length=str.Length }

    let text table = PieceTree.text table

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
        { Pieces = tree; Buffer = buffer; Length = buffer.Length }
=======
    (* TextTable creation functions. *)
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

    (* TextTable IO functions. *)
    /// Save a TextTableType using the specified StreamWriter.
    let saveFile table (writer: StreamWriter) =
        ListZipper.write table writer

    (* TextTable consolidation functions. *)
    /// Checks if a TextTableType can be consolidated.
    let inline private canBeConsolidated table =
        table.Pieces.Focus.Length + table.Pieces.Path.Length > 1

    /// Returns a consolidated table with only used characters and a single piece
    /// or returns the original table if it cannot be consolidated any further.
    /// This provides better performance for table operations as the table's memory may
    /// degrade over time. 
    /// The author's use case was in a text editor where this method can be called 
    /// asynchronously or in another thread (it is thread safe) 
    /// after an appreciable delay after typing to ensure continued 
    /// best-case performance for core operations like insert and delete.
    /// If the user types during that time, the result can be discarded 
    /// in the multithreaded version or the operation can be cancelled 
    /// in the async version.
    /// Note that the caller must wrap around this method to implement it
    /// in either a multithreaded or async manner.
    let consolidate table =
        if canBeConsolidated table
        then
            let folder acc piece = 
                let text = Piece.text piece table
                Buffer.append text acc
            let buffer = List.fold folder Buffer.empty (List.rev table.Pieces.Path)
            let buffer = List.fold folder buffer table.Pieces.Focus
            let piece = Piece.createWithSpan (Span.createWithLength 0 buffer.Length)
            let zipper = {Focus = [piece]; Path = []; Index = 0 }
            {Pieces = zipper; Buffer = buffer;}
        else
            table
>>>>>>> main

    (* TextTable "modification" functions. *)
    /// Returns a new TextTableType with the string inserted.
    let insert index (str: string) (table: TextTableType) =
        let buffer = Buffer.append str table.Buffer
        let piece = Piece.create false (buffer.Length - str.Length) str.Length
<<<<<<< HEAD
        let pieces = PieceTree.insert index piece table.Pieces
=======
        let pieces = ListZipper.insert index piece table.Pieces
>>>>>>> main

        { Buffer = buffer; Pieces = pieces; Length = table.Length + str.Length }

    let delete startIndex length (table: TextTableType) =
        let span = Span.createWithLength startIndex length
        let newPieces = PieceTree.delete span table.Pieces
        { Pieces = newPieces; Buffer = table.Buffer; Length = table.Length - length }

    let substring startIndex length table = 
        let span = Span.createWithLength startIndex length
        PieceTree.substring span table

    /// Returns a new TextTableType with the text in the given range removed.
    let delete startIndex length table =
        let span = Span.createWithLength startIndex length
        ListZipper.delete span table

    (* TextTable read functions. *)
    /// Retrieves all of the text from a table as a string.
    /// It is recommended to use the substring method for 
    /// text retrieval in most cases for performance reasons.
    let text table = ListZipper.text table

    /// Returns a substring from the table.
    let substring startIndex length table = 
        let span = Span.createWithLength startIndex length
        ListZipper.textSlice span table

    /// Find the first occurrence of a string in the table returns an int representing the index.
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
            if curPos <= 0
            then 
                let searchSpan = Span.createWithLength 0 1
                let searchText = ListZipper.textSlice searchSpan table 
                searchText.LastIndexOf(str, StringComparison.OrdinalIgnoreCase)
            else
                let searchStartPos = 
                    if curPos >= Buffer.MaxBufferLength
                    then curPos - Buffer.MaxBufferLength
                    else 0
                let searchSpan = Span.createWithLength searchStartPos Buffer.MaxBufferLength
                let searchText = ListZipper.textSlice searchSpan table
                let isFound = searchText.LastIndexOf(str, StringComparison.OrdinalIgnoreCase)
                if isFound >= 0
                then isFound + searchStartPos
                else 
                    let firstPosOfLastChar = searchText.IndexOf(str[str.Length - 1], StringComparison.OrdinalIgnoreCase)
                    if firstPosOfLastChar > -1
                    then loop firstPosOfLastChar
                    else loop searchStartPos

        if table.Buffer.Length <= Buffer.MaxBufferLength
        then (text table).LastIndexOf(str)
        else loop (table.Buffer.Length)

    /// Finds all occurrences of a string in the table and returns a list containing each index in order.
    /// Returns an empty list if the given string was not found.
    let allIndexesOf (str: string) (table: TextTableType) =
        let rec loop curStart curLength (acc: int list) =
            if curStart = 0 
            then
                let searchSpan = Span.createWithLength 0 curLength
                let searchText = ListZipper.textSlice searchSpan table 
                let isFound = searchText.LastIndexOf(str, StringComparison.OrdinalIgnoreCase)
                if isFound = -1
                then acc
                else 
                    loop curStart (curLength - 1) (isFound::acc)
            elif curStart <= Buffer.MaxBufferLength * -1
            then
                acc
            elif curStart < 0
            then
                let searchSpan = Span.createWithLength 0 (curLength + curStart)
                let searchText = ListZipper.textSlice searchSpan table 
                let isFound = searchText.LastIndexOf(str, StringComparison.OrdinalIgnoreCase)
                if isFound = -1
                then acc
                else 
                    loop 0 (searchSpan.Length - 1) (isFound::acc)
            else
                let searchSpan = Span.createWithLength curStart curLength
                let searchText = ListZipper.textSlice searchSpan table 
                let isFound = searchText.LastIndexOf(str, StringComparison.OrdinalIgnoreCase)
                if isFound = -1
                then loop (curStart - 1) Buffer.MaxBufferLength acc
                else
                    let findPos = isFound + curStart
                    loop (curStart - 1) Buffer.MaxBufferLength (findPos::acc)

        if table.Buffer.Length <= Buffer.MaxBufferLength
        then loop 0 table.Buffer.Length []
        else loop (table.Buffer.Length - Buffer.MaxBufferLength) Buffer.MaxBufferLength []

    (* Alternative OOP API. *)
    type TextTableType with

        member this.Insert(index, str) = insert index str this
        member this.Text() = text this
<<<<<<< HEAD
        member this.Substring(start, length) = substring start length this
        member this.Delete(start, length) = delete start length this
=======

        member this.Delete(start, length) =
            let span = Span.createWithLength start length
            ListZipper.delete span this

        member this.Substring(startIndex, length) =
            let span = Span.createWithLength startIndex length
            ListZipper.textSlice span this

        member this.IndexOf(str) = indexOf str this
        member this.LastIndexOf(str) = lastIndexOf str this
        member this.AllIndexesOf(str) = allIndexesOf str this
>>>>>>> main
