namespace PieceTable

open System.Text
open Types

module InputBuffer =
    /// <summary>
    /// Create a new input buffer at the specified offset.
    /// </summary>
    let create offset =
        { Content = StringBuilder()
          Offset = offset }

    /// <summary>
    /// Get the buffer's end position.
    /// </summary>
    let private getEnd buffer = buffer.Offset + buffer.Content.Length

    /// <summary>
    /// Get the buffer's length.
    /// </summary>
    let length buffer = buffer.Content.Length

    /// <summary>
    /// Append a new character to the buffer's end.
    /// </summary>
    let private append buffer (chr: char) =
        buffer.Content.Append chr |> ignore
        buffer

    /// <summary>
    /// Add a new character to the buffer in the middle or start.
    /// </summary>
    let private insertMidOrStart buffer cursorPos (chr: char) =
        let bufferInsertPos = cursorPos - buffer.Offset
        buffer.Content.Insert(int bufferInsertPos, chr) |> ignore
        buffer

    /// <summary>
    /// Add a character to the buffer.
    /// </summary>
    let insert cursorPos (chr: char) buffer =
        match cursorPos, buffer.Offset, (getEnd buffer) with
        // insert at end of buffer
        | cursor, _, bufEnd when cursor = bufEnd -> append buffer chr
        // insert at or after start start and before end
        | cursor, bufStart, bufEnd when cursor >= bufStart && cursor < bufEnd -> insertMidOrStart buffer cursorPos chr

        // These cases should be avoided by the caller which
        // inserts the current buffer contents into the tree and clears the string builder
        | cursor, bufStart, _ when cursor < bufStart -> failwith "trying to insert before buffer"
        | cursor, _, bufEnd when cursor > bufEnd -> failwith "trying to insert after buffer"
        | _ -> failwith "unknown insert case"

    /// <summary>
    /// Remove one or more character from the buffer.
    /// </summary>
    let remove (remStart: int) (remLength: int) buffer =
        match buffer.Offset, (getEnd buffer), remStart, remLength with
        | bufStart, bufEnd, remStart, remLength when remStart >= bufStart && (remStart + remLength) <= bufEnd ->
            let startIndex = if remStart <= 0 then 0 else int remStart - 1
            buffer.Content.Remove(startIndex, int remLength) |> ignore
            buffer

        // Failure cases should be handled by the caller
        | bufStart, _, remStart, _ when remStart < bufStart -> failwith "trying to remove before buffer"
        | _, bufEnd, remStart, remLen when (remStart + remLen) > bufEnd -> failwith "trying to remove after buffer"
        | _ -> failwith "unknown removal case"

    /// <summary>
    /// Gets an input buffer's text.
    /// Alternatively, one can call buffer.Content.ToString() which this is a wrapper for.
    /// </summary>
    let getText () buffer = buffer.Content.ToString()

    // Alternative API that lets one call functions on a record, similar to OOP if client wishes
    type InputBuffer with

        member this.Insert(cursor, chr) = insert cursor chr this
        member this.Remove remStart remLength = remove remStart remLength this
        member this.GetText = getText () this
