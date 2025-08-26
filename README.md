# SyntaxScanner

## What is it?

**SyntaxScanner**  is a high-performance .NET library for syntax-aware splitting and slicing based on arbitrary text input. The available API lets you freely define the syntax rules of your language and how it interacts with one another. It is heavily optimized to favor stack allocations and `ReadOnlySpan<char>` for maximum performance.

## Usage and comparison with native methods

* `IndexOf with syntax awareness`

```csharp
var input = "a = \";\" ;";
var syntax = [ new SyntaxPair('\"', '\"') ];

var length = input.Length; //length: 9

var index = SyntaxView.IndexOf(input, syntax, value);
var standardIndex = input.IndexOf(value);

//index: 8
//standardIndex: 5
```

<br/>

* `Slicing with syntax awareness`

```csharp
var input = "path(\"some text) \"), trailing text";
var syntax = [ new SyntaxPair('\"', '\"') ];

var slice = SyntaxView.SliceInBetween(input, syntax, '(', ')', out var remainder);

var start = input.IndexOf('(');
var end = input.IndexOf(')', start);
var standardSlice = input.SubString(start, end - start);
var standardRemainder = input.SubString(end);

//slice: "\"some text) \""
//remainder: ", trailing text"

//standardSlice: "\"some text"
//standardRemainder: " \"), trailing text"
``` 

<br/>

* `Split with syntax awareness`

```csharp
var input = "path(a,b), \"b,c,d\", e";
var syntax = [ new SyntaxPair('\"', '\"'), new SyntaxPair('(', ')') ];

//streams in the resulting slices to avoid allocating an array
foreach (var slice in SyntaxView.Split(input, syntax, ',', stackalloc SyntaxPair[64])) //optionally reserve some space on the stack for maximum performance
{
    //do something with it
}

var standardSplit = input.Split(',');

//results (each slice): ["path(a,b)", " \"b,c,d\"", " e"]
//                       └─────────┘  └──────────┘  └──┘

//standardSplit: ["path(a", "b)", " \"b", "c", "d\"", " e"]
//                └──────┘  └──┘  └────┘  └─┘  └───┘  └──┘
```

<br/>

* `Split per token (mainly used for arimethic operators)`

```csharp

var input = "a + b";
var supportedTokens = ["==", "!=", "<", "<=", ">", ">=", "+", "-", "*", "/"];

foreach (var (start, end, isToken) in SyntaxView.SplitByTokens(input, [], supportedTokens, stackalloc SyntaxPair[64])) //optionally reserve some space on the stack for maximum performance
{
    var slice = input.Slice(start, end);
    var tokenOrNot = isToken;
}

//results: 'a ' '+' ' b'
``` 

<br/>


# BidirectionalIterator

## What is it?

Do you love enumerables, foreach loops and their strong integration with LINQ? Did you ever feel the need to go back a few elements or switch direction mid-way of enumeration? <br>
Then this is for you: an iterator that moves forward, backward, jumps over how many elements you like, and itegrates seamlessly with LINQ.

## What is it built on?

Since their interface extends both the `IEnumerator<T>` and `IEnumerable<T>` interface, all bidirectional iterators automatically support foreach blocks and LINQ (which are forward-only by definition).
