module BufferSubstringTests

open System
open System.Collections
open Xunit
open PieceTable
open PieceTable.Buffer

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

[<Fact>]
let ``Can get different substrings there are three nodes in buffer`` () =
    let str = (String.replicate 65535 "a") + (String.replicate 65535 "b") + (String.replicate 65535 "c")
    let buffer = Buffer.createWithString str
    
    (* Can get substring including end of first node and start of second. *)
    Assert.Equal("ab", buffer.Substring(65534, 2)) 

    (* Can get substring including end of first node, whole of second, and start of third. *)
    let expected = "a" + (String.replicate 65535 "b") + "c"
    Assert.Equal(expected, buffer.Substring(65534, 65537))

    (* Can get substring including all nodes. *)
    Assert.Equal(str, buffer.Substring(0, (65535 * 3)))

[<Fact>]
let ``Buffer returns a code point instead of splitting when we try to substring`` () =
    let str = "123😊567"
    let buffer = Buffer.createWithString str
    
    let t0 = buffer.Substring(0, 3)
    let t1 = buffer.Substring(0, 4)
    let t2 = buffer.Substring(2, 3)
    let t3 = buffer.Substring(4, 2)

    Assert.Equal("123", t0) (* Can get before emoji. *)
    Assert.Equal("123😊", t1) (* Can get emoji. *)
    Assert.Equal("3😊5", t2) (* Can get around emoji. *)
    Assert.Equal("56", t3) (* Can get after emoji. *)