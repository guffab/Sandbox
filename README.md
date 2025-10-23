# SyntaxScanner

## What is it?

**SyntaxScanner**  is a high-performance .NET library for syntax-aware splitting and slicing based on arbitrary text input. The available API lets you freely define the syntax rules of your language and how they interact with each another. It is heavily optimized to favor stack allocations and `ReadOnlySpan<char>` for maximum performance.

## Examples vs. Native

This section is intended as a showcase for the library, thus comparisons to existing `string` methods are made.

* `IndexOf with syntax awareness`

```csharp
var input = "a = ';' ;";
var syntax = [ new SyntaxPair('\'', '\'') ];

var index = input.IndexOf(';', syntax);
var standardIndex = input.IndexOf(';');

//index: 8
//standardIndex: 5
```

<br/>

* `Slicing with syntax awareness`

```csharp
var input = "path('some text) '), trailing text";
var syntax = [ new SyntaxPair('\'', '\'') ];

var slice = input.SliceBetween('(', ')', syntax, out var remainder);

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
foreach (var slice in input.Split(',', syntax, stackalloc SyntaxPair[64])) //optionally reserve some space on the stack for maximum performance
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

foreach (var (start, end, isToken) in input.SplitTokenized(separators, syntax, stackalloc SyntaxPair[64])) //optionally reserve some space on the stack for maximum performance
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

# IBidirectionalIterator

## What is it?

**IBidirectionalIterator** is an interface supporting forwards, backwards and mixed enumeration, all while maintaining full compatibility with LINQ and foreach loops.

## Usage

* `Creating the iterator`

```csharp
var list = Enumerable.Range(0, 10).ToList();
var iterator = list.GetBidirectionalIterator(); //implemented as an extension method
```

<br/>

* `Using foreach/LINQ`

```csharp
foreach (var item in iterator) { }

var count = iterator.Select(x => x).Count();
```

<br/>

* `Backwards enumeration`

```csharp
while (iterator.MovePrevious())
{
    var current = iterator.Current;
}
```

<br/>

* `Jumping over items`

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

# ActionContainers

## What is it?

**ActionContainers** are a simple and save way to represent complex hierarchies of data refererrencing each other. The goal is to have any updates (like renaming a tpye, changing a parameter value) be reflected across the entire structure, **without** performing recursive search-and-replace.
As a secondary goal, data redunancy is reduced to an absolute minimum to allow for even smaller file sizes when serialized.

## Usage

* `Initializing the pool`

```csharp
//assuming you already have some data available
var serializedJson = "[]"
ActionNodePool.Instance.Reset(serializedJson);
```

<br/>

* `Using mutable abstractions`

```csharp
var action1 = new MutableAction(ActionNodePool.Instance["PG_Production Unit", "Filigree Slab"]);
var action2 = new MutableAction(ActionNodePool.Instance["PG_Production Unit", "Double Wall"]);

action1.ActionName = "Production Unit"; //updates the representations in the pool directly, therefore both action1 and action2 now refer to the new name.

var parameter1 = action1["PF_Layer 1"];
parameter1.Id = "Layer 1"; //again, since this updates the pool directly, all consumers of the "Production Unit" see the updated parameter name.
```

<br/>

* `PLACEHODLER`

