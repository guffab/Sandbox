# SyntaxScanner

## What is it?

**SyntaxScanner**  is a high-performance .NET library for syntax-aware splitting and slicing based on arbitrary text input. The available API lets you freely define the syntax rules of your language and how it interacts with one another. It is heavily optimized to favor stack allocations and `ReadOnlySpan<char>` for maximum performance.

## Usage

* `IndexOf with syntax awareness`

```csharp
var input = "a = \";\" ;";
var syntax = [ new SyntaxPair('\"', '\"') ];

var index = SyntaxView.IndexOf(input, syntax, value);
var length = input.Length;

//index: 8
//length: 9
```

<br/>

* `Slicing with syntax awareness`

```csharp
var input = "path(\"notice this closing quote ->)<- \"), trailing text";
var syntax = [ new SyntaxPair('\"', '\"') ];

var slice = SyntaxView.SliceInBetween(input, syntax, '(', ')', out var remainder);

//slice: '"notice this closing quote ->)<- "'
//remainder: ', trailing text'
``` 

<br/>

* `Split with syntax awareness`

```csharp
var input = "path(a,b), \"b,c,d\", e";
var syntax = [ new SyntaxPair('\"', '\"'), new SyntaxPair('(', ')') ];

foreach (var slice in SyntaxView.Split(input, syntax, ',', stackalloc SyntaxPair[64])) //optionally reserve some space on the stack for maximum performance
{
    //do something with it
}

//results: 'path(a,b)' ' \"b,c,d\"' ' e'
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
