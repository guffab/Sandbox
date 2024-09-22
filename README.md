## SyntaxParser

### What is it? :star2:

**SyntaxParser** is a .NET library for syntax-aware splitting and slicing based on arbitrary string input. The available API lets you freely define the syntax of your language and how it interacts with one another.

### How does it perform? :rocket:

While a previous implementation was based on `string`, the most recent implementation uses `ReadOnlySpan<char>` on the API level, as well as internally. Additionally, most usages of <span style="color:#569CD6">class</span> objects are now substituted with <span style="color:#569CD6">ref struct</span> objects to avoid heap allocations.

