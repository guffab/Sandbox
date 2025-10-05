using SyntaxScanner;

namespace Example.SyntaxScanner;

public class PathItemParser
{
    static readonly SyntaxPair[] syntax = [new SyntaxPair('(', ')', 0), new SyntaxPair('[', ']', 0)];

    /// <returns>
    /// The parsed <see cref="PathItem"/> if the input consists of more than whitespace; Otherwise <see langword="null"/>.
    /// </returns> 
    public static PathItem? Parse(ReadOnlySpan<char> input)
    {
        var parser = new PathItemParser();
        return parser.Parse(input, 0);
    }

    /// <summary>
    /// The high-level recurcsive component that all internal methods should use.
    /// </summary>
    private PathItem? Parse(ReadOnlySpan<char> input, int index)
    {
        CleanInput(ref input, ref index);
        if (input.IsEmpty)
            return null;

        //here it's okay to use non-syntax-respecting search because we're literally searching for any syntax at all
        var indexOfSyntax = input.IndexOfAny('.', '(', '[');
        if (indexOfSyntax is -1)
            return new PropertyItem(input, index, null);

        if (input[indexOfSyntax] is '.')
        {
            var next = Parse(input.Slice(indexOfSyntax), index + (indexOfSyntax));
            return new PropertyItem(input.Slice(0, indexOfSyntax), index, next);
        }

        if (input[indexOfSyntax] is '(')
        {
            var closingIndex = input.Slice(indexOfSyntax).IndexOf(')', syntax);
            if (closingIndex is -1)
                return new PropertyItem(input.Slice(0, indexOfSyntax), index, new InvalidItem(index + indexOfSyntax + 1, input.Length - indexOfSyntax));

            return new PropertyItem(input.Slice(0, indexOfSyntax), index, new CheckItem(input.Slice(indexOfSyntax + 1, closingIndex - 1), index + indexOfSyntax, index + indexOfSyntax + closingIndex, index + indexOfSyntax + 1));
        }

        if (input[indexOfSyntax] is '[')
        {
            var closingIndex = input.Slice(indexOfSyntax).IndexOf(']', syntax);
            if (closingIndex is -1)
                return new PropertyItem(input.Slice(0, indexOfSyntax), index, new InvalidItem(index + indexOfSyntax + 1, input.Length - indexOfSyntax));

            var subItem = ParseOrInvalid(input.Slice(indexOfSyntax + 1, closingIndex - 1), index + (indexOfSyntax + 1));
            var next = Parse(input.Slice(indexOfSyntax + 1 + closingIndex), index + (indexOfSyntax + closingIndex + 1));

            return new PropertyItem(input.Slice(0, indexOfSyntax), index, new ListItem(subItem, index + indexOfSyntax, index + indexOfSyntax + closingIndex, next));
        }

        return new InvalidItem(index, input.Length);
    }

    /// <summary>
    /// Parses the input as usual, but guarantees a non-null return value.
    /// </summary>
    private PathItem ParseOrInvalid(ReadOnlySpan<char> input, int index)
    {
        CleanInput(ref input, ref index);

        var result = Parse(input, index);
        if (result is not null)
            return result;

        return new InvalidItem(index, input.Length);
    }

    private static void CleanInput(ref ReadOnlySpan<char> input, ref int index)
    {
        index += CountLeadingWhiteSpace(input);
        input = input.Trim();
        index += CountLeadingChar(input, '.');
        input = input.TrimStart('.'); //not actually needed to indicate a property
    }

    internal static int CountLeadingWhiteSpace(ReadOnlySpan<char> input)
    {
        int whiteSpace = 0;

        for (int i = 0; i < input.Length; i++)
        {
            if (char.IsWhiteSpace(input[i]))
                whiteSpace++;
            else
                return whiteSpace;
        }
        return whiteSpace;
    }

    private static int CountLeadingChar(ReadOnlySpan<char> input, char c)
    {
        int whiteSpace = 0;

        for (int i = 0; i < input.Length; i++)
        {
            if (c == input[i])
                whiteSpace++;
            else
                return whiteSpace;
        }
        return whiteSpace;
    }
}
