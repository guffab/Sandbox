using System;

namespace SyntaxScanner;

public static  class PathParser
{
    static readonly SyntaxPair[] syntax = [new SyntaxPair('(', ')', 0), new SyntaxPair('[', ']', 0)];

    public static void Run()
    {
        var example1 = "Disassemble[Parameters[Definition.Name((PF_Number)|(Number))].AsElementId.Name(^Ass)].Name";
        var bb = Parse(example1.AsSpan());
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>
    /// May return <see langword="null"/> if the input consists of whitespace only.
    /// </returns> 
    public static PathToken? Parse(ReadOnlySpan<char> input)
    {
        //keeping track of indices and such is ignored for now
        input = input.Trim();
        input = input.TrimStart('.'); //not actually needed to indicate a property
        if (input.IsEmpty)
            return null;

        //here it's okay to use non-syntax-respecting search because we're literally searching for syntax
        var indexOfSyntax = input.IndexOfAny('.', '(', '[');
        if (indexOfSyntax is -1)
            return new PropertyToken(input.ToString(), null);

        if (input[indexOfSyntax] is '.')
            return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), Parse(input.Slice(indexOfSyntax)));

        if (input[indexOfSyntax] is '(')
        {
            var closingIndex = input.Slice(indexOfSyntax).IndexOf(')', syntax);
            if (closingIndex is -1)
                return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), new InvalidToken());

            return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), new CheckToken(input.Slice(indexOfSyntax + 1, closingIndex - 1).ToString()));
        }

        if (input[indexOfSyntax] is '[')
        {
            var closingIndex = input.Slice(indexOfSyntax).IndexOf(']', syntax);
            if (closingIndex is -1)
                return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), new InvalidToken());

            var next = new ListToken(Parse(input.Slice(indexOfSyntax + 1, closingIndex - 1)) ?? new InvalidToken(), Parse(input.Slice(indexOfSyntax + 1 + closingIndex)));
            return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), next);
        }

        return new InvalidToken();
    }
}
