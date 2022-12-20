# Purely Functional Piece Table in F#

## Introduction
This is a purely functional Piece Table written in F#. A Piece Table is a data structure that has often been used in text editors such as Visual Studio/Code and AbiWord. 

For more information about the Piece Table data structure, [this repository](https://github.com/veler/Csharp-Piece-Table-Implementation) provides some helpful introductory links.

This particular implementation contains a Piece Table with the Pieces stored in a linked list with a zipper which, like a splay tree, move us to the node where the last operation was performed. This lets us reach nodes local to the previous one and perform operations on them more efficiently.

## Credits

Third party libraries used in this implementation include:

- [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) for performance tests.
- [NStack](https://github.com/gui-cs/NStack) for enabling Unicode CodePoint support.

The associated licenses can be found in the third-party-licenses directory within this repository's root.