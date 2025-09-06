# SyntaxScanner

## What is it?

**SyntaxScanner**  is a high-performance .NET library for syntax-aware splitting and slicing based on arbitrary text input. The available API lets you freely define the syntax rules of your language and how it interacts with one another. It is heavily optimized to favor stack allocations and `ReadOnlySpan<char>` for maximum performance.

## Usage and comparison with native methods

* `IndexOf with syntax awareness`

```csharp
var input = "a = ';' ;";
var syntax = [ new SyntaxPair('\'', '\'') ];

var index = SyntaxView.IndexOf(input, syntax, value);
var standardIndex = input.IndexOf(value);

//index: 8
//standardIndex: 5
```

<br/>

* `Slicing with syntax awareness`

```csharp
var input = "path('some text) '), trailing text";
var syntax = [ new SyntaxPair('\'', '\'') ];

var slice = SyntaxView.SliceInBetween(input, syntax, '(', ')', out var remainder);

var start = input.IndexOf('(');
var end = input.IndexOf(')', start);
var standardSlice = input.SubString(start, end - start);
var standardRemainder = input.SubString(end);

//slice: "'some text) '"
//standardSlice: "'some text"

//remainder: ", trailing text"
//standardRemainder: " '), trailing text"
``` 

<br/>

* `Split with syntax awareness`

```csharp
var input = "path(a,b), 'b,c,d', e";
var syntax = [ new SyntaxPair('\'', '\''), new SyntaxPair('(', ')') ];

//streams in the resulting slices to avoid allocating an array
foreach (var slice in SyntaxView.Split(input, syntax, ',', stackalloc SyntaxPair[64])) //optionally reserve some space on the stack for maximum performance
{
    //do something with it
}

var standardSplit = input.Split(',');

//results (each slice): ["path(a,b)", " 'b,c,d'", " e"]
//                       └─────────┘  └────────┘  └──┘

//standardSplit: ["path(a", "b)", " 'b", "c", "d'", " e"]
//                └──────┘  └──┘  └───┘  └─┘  └──┘  └──┘
```

<br/>

* `Split with syntax awareness (include separators)`

```csharp
var input = "(1 + 3) * 2 == 4";
var separators = ["+", "*", "==" ];//extend as needed
var syntax = [ new SyntaxPair('(', ')') ];

foreach (var (start, end, isToken) in SyntaxView.SplitByTokens(input, syntax, separators, stackalloc SyntaxPair[64])) //optionally reserve some space on the stack for maximum performance
{
    var slice = input.Slice(start, end);
}

var standardSplit = input.Split(separators, StringSplitOptions.None);

//results (each slice): ["(1 + 3) ", "*", " 2 ", "==", " 4"]
//                       └────────┘  └─┘  └───┘  └──┘  └──┘

//standardSplit: ["(1 ", " 3) ", " 2 ", " 4"]
//                └───┘  └────┘  └───┘  └──┘
``` 

<br/>


# BidirectionalIterator

## What is it?

Do you love enumerables, foreach loops and their strong integration with LINQ? But, have you also ever missed the option to reverse direction mid-way of enumeration? Then this is for you: an iterator that moves forwards, backwards, jumps over how many elements you like, and itegrates seamlessly with LINQ.

## Usage

* `creating the iterator`

```csharp
var list = Enumerable.Range(0, 10).ToList();
var iterator = list.GetBidirectionalIterator(); //implemented as an extension method
```

<br/>

* `foreach/LINQ usage`

```csharp
foreach (var item in iterator) { }

var count = iterator.Select(x => x).Count();
```

<br/>

* `backwards enumeration`

```csharp
while (iterator.MovePrevious())
{
    var current = iterator.Current;
}
```

<br/>

* `jump over items`

```csharp
if (iterator.Move(4)) //move four indices forwards
{
    var current = iterator.Current;
}

if (iterator.Move(-3)) //move three indices backwards
{
    var current = iterator.Current;
}
```

<br/>
