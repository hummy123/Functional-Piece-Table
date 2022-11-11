namespace PieceTable

open Types

module Piece =
    let create isOriginal start length =
        { IsOriginal = isOriginal
          Span = Span.createWithLength start length }

    let createWithSpan isOriginal span =
        { IsOriginal = isOriginal; Span = span }


    type Pieces =
        | Merge of PieceType
        | Split of PieceType * PieceType * PieceType

    let split (a: PieceType) (b: PieceType) =
        match a.IsOriginal = b.IsOriginal with
        (* Just merge the two pieces into one. *)
        | true ->
            let p = createWithSpan a.IsOriginal (Span.union a.Span b.Span)
            Merge(p)
        (* Split into three pieces. *)
        | false ->
            let p1 = create a.IsOriginal a.Span.Start (b.Span.Start - a.Span.Start)
            let stopA = Span.stop a.Span
            let stopB = Span.stop b.Span
            let stop3 = if stopA > stopB then stopA else stopB
            let span3 = Span.createWithStop ((Span.stop b.Span) + 1) stop3
            let p3 = createWithSpan a.IsOriginal span3
            Split(p1, b, p3)
