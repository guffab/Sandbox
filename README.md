## SyntaxScanner

### What is it?

**SyntaxScanner** is a high-performance .NET library for syntax-aware splitting and slicing based on arbitrary string input. The available API lets you freely define the syntax rules of your language and how it interacts with one another.

### How does it perform?

While a previous implementation was based on `string`, the most recent implementation uses `ReadOnlySpan<char>` on the API level, as well as internally.
Additionally, most usages of ${\textsf{\color{navyblue}class}}$ objects are now substituted with ${\textsf{\color{navyblue}ref struct}}$ objects to avoid heap allocations.

### Sounds cool, but how do I use it?

Take a look at the example implementation [here](/Examples/Example.SyntaxScanner/MyLanguageParser.cs).


## BidirectionalIterator

### What is it?

Do you love enumerables, foreach loops and their strong integration with LINQ? Did you ever feel the need to go back a few elements or switch direction mid-way of enumeration? <br>
Then this is for you: an iterator that moves forward, backward, jumps over how many elements you like, and itegrates seamlessly with LINQ.

### What is it built on?

Since their interface extends both the `IEnumerator<T>` and `IEnumerable<T>` interface, all bidirectional iterators automatically support foreach blocks and LINQ (which are forward-only by definition).
