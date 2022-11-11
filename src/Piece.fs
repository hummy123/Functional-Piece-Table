namespace PieceTable

open Types

module Piece =
    let create isOriginal start length =
        { IsOriginal = isOriginal
          Span = Span.createWithLength start length }

    let createWithSpan isOriginal span =
        { IsOriginal = isOriginal; Span = span }
