namespace PieceTable

open Types

module internal Span =
    /// Create a span specifying the start index of the buffer it is on and the number of characters it has.
    let createWithLength start length = { Start = start; Length = length }

    /// Create a span specifying the start and end index of the buffer it is on.
    let createWithStop start stop =
        { Start = start; Length = stop - start }

    /// Returns the span's end index. Called "stop" because "end" is a reserved keyword in F#.
    let stop span = span.Start + span.Length

    /// Returns true if a span has a length of 0; otherwise, retrns false.
    let isEmpty (span: SpanType) = span.Length = 0

    type SpanType with

        member this.Stop = stop this
        member this.IsEmpty = isEmpty this