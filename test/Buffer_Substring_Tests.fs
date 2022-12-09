module BufferSubstringTests

open System
open System.Collections
open Xunit
open PieceTable
open PieceTable.Types

[<Fact>]
let ``Can get different indices of a buffer with five characters`` () =
    let buffer = Buffer.createWithString "12345"
    Assert.Equal("", buffer.Substring(0, 0))
    Assert.Equal("1", buffer.Substring(0, 1))
    Assert.Equal("12", buffer.Substring(0, 2))
    Assert.Equal("123", buffer.Substring(0, 3))
    Assert.Equal("1234", buffer.Substring(0, 4))
    Assert.Equal("12345", buffer.Substring(0, 5))
    Assert.Equal("12345", buffer.Substring(0, 6)) (* Specifying length higher than what we have in the buffer should return max. *)

    Assert.Equal("2345", buffer.Substring(1, 4))
    Assert.Equal("345", buffer.Substring(2, 3))
    Assert.Equal("45", buffer.Substring(3, 2))
    Assert.Equal("5", buffer.Substring(4, 1))
    Assert.Equal("", buffer.Substring(5, 5)) (* Length number doesn't matter in this case. *)

    Assert.Equal("2", buffer.Substring(1, 1))
    Assert.Equal("23", buffer.Substring(1, 2))
    Assert.Equal("234", buffer.Substring(1, 3))