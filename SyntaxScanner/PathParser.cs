using System;

namespace SyntaxScanner;

public static  class PathParser
{
    public static SyntaxPair[] Syntax = [new SyntaxPair('(', ')', 0), new SyntaxPair('[', ']', 0)];


    public static void Run()
    {
        var example1 = "Disassemble[Parameters[Definition.Name(PF_Number)].AsElementId.Name(^Ass)].Name";
        var aa = Parse(example1.AsSpan());
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
        if (input.IsEmpty)
            return null;

        //here it's okay to use non-syntax-respecting search because we're literally searching for syntax
        var indexOfSyntax = input.IndexOfAny('.', '(', '[');
        if (indexOfSyntax is -1)
            return new PropertyToken("input", null);

        if (input[indexOfSyntax] is '.')
            return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), Parse(input.Slice(indexOfSyntax + 1)));

        if (input[indexOfSyntax] is '(')
        {
            var closingIndex = input.Slice(indexOfSyntax + 1).IndexOf(')', Syntax);
            if (closingIndex is -1)
                return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), new InvalidToken());

            return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), new CheckToken(input.Slice(indexOfSyntax + 1, closingIndex).ToString()));
        }

        if (input[indexOfSyntax] is '[')
        {
            var closingIndex = input.Slice(indexOfSyntax + 1).IndexOf(']', Syntax);
            if (closingIndex is -1)
                return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), new InvalidToken());

            var listNext = closingIndex + 1 >= input.Length ? null : Parse(input.Slice(closingIndex + 1));

            return new PropertyToken(input.Slice(0, indexOfSyntax).ToString(), new ListToken(Parse(input.Slice(indexOfSyntax + 1, closingIndex)) ?? new InvalidToken(), listNext));
        }

        else
            return new InvalidToken();
    }
}
