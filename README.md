## SyntaxScanner

### What is it? :star2:

**SyntaxScanner** is a high-performance .NET library for syntax-aware splitting and slicing based on arbitrary string input. The available API lets you freely define the syntax rules of your language and how it interacts with one another.

### How does it perform? :rocket:

While a previous implementation was based on `string`, the most recent implementation uses `ReadOnlySpan<char>` on the API level, as well as internally. Additionally, most usages of ${\textsf{\color{navyblue}class}}$ objects are now substituted with ${\textsf{\color{navyblue}ref struct}}$ objects to avoid heap allocations.

### Sounds cool, but how do I use it? :eyes:

Take a look at the example implementation [here](/Examples/Example.SyntaxParser/).
