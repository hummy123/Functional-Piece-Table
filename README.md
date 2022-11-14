# Purely Functional Piece Table in F#

This is a purely functional Piece Table written in F#. A Piece Table is a data structure that has often been used in text editors such as Visual Studio/Code and AbiWord. 

For more information about the Piece Table data structure, [this repository](https://github.com/veler/Csharp-Piece-Table-Implementation) provides some helpful introductory links.

There are some optimisations that can be made with this functional implementation, such as: 
- Using a StringBuilder instead of a string for the AddBuffer (at the cost of the structure no longer being purely functional), or
- Iterating over the list of pieces backwards if we are trying to insert near the end.
 
However, my goal in creating this was to use it as a starting point for a more efficient data structure that build on this, such as a [Counted B-Tree](https://www.chiark.greenend.org.uk/~sgtatham/algorithms/cbtree.html) or a Piece Tree. So I see no point trying to optimise what is simply a stepping stone when that optimisation will not live on through the next iterations.
