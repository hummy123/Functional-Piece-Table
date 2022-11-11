namespace PieceTable

open Types

module Span =
    let createWithLength start length = { Start = start; Length = length }

    let createWithStop start stop =
        { Start = start; Length = stop - start }

    /// <summary>
    /// Returns the span's end index. Called "stop" because "end" is a reserved keyword in F#.
    /// </summary>
    let stop span = span.Start + span.Length

    let isEmpty span = span.Length = 0

    let containsPosition span position =
        position >= span.Start && position < stop span

    /// <summary>
    /// Returns true if the first span fully contains the second.
    /// </summary>
    let containsSpan a b = a.Start >= b.Start && stop a <= stop b

    let maxStartMinStop a b =
        let start = if a.Start > b.Start then a.Start else b.Start
        let stopA = stop a
        let stopB = stop b
        let stop = if stopA > stopB then stopB else stopA
        (start, stop)

    let areOverlapping a b =
        let (start, stop) = maxStartMinStop a b
        start < stop

    let overlap a b =
        let (start, stop) = maxStartMinStop a b

        match start < stop with
        | true -> Some(createWithStop start stop)
        | false -> None

    let areIntersecting a b = b.Start < stop a && stop b >= a.Start

    let intersection a b =
        let (start, stop) = maxStartMinStop a b

        match start <= stop with
        | true -> Some(createWithStop start stop)
        | false -> None

    let union a b =
        let start = if a.Start < b.Start then a.Start else b.Start
        let stopA = stop a
        let stopB = stop b
        let stop = if stopA < stopB then stopA else stopB

        createWithStop start stop

    type SpanType with

        member this.Stop = stop this
