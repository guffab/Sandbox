namespace SyntaxParser;

/// <summary>
/// Handles parsing and interpreting a generic <see cref="string"/> as language syntax.<br/>
/// The syntax identifiers itself are highly configurable by the caller.
/// </summary>
/// <param name="syntax">The full syntax to take into account.</param>
/// <param name="separator">The separator to split on.</param>
/// <param name="start">The start of a slice.</param>
/// <param name="end">The end of a slice.</param>
public partial class SyntaxParser(SyntaxPair[] syntax, char separator, char start, char end)
{
    private readonly SyntaxPair[] fullSyntax = syntax;
    private readonly SyntaxPair[] subsetOfSyntax = syntax.Where(x => x.Start != start).ToArray();
    private readonly char separator = separator;
    private readonly char start = start;
    private readonly char end = end;

#if NETFRAMEWORK
    /// <inheritdoc cref="SyntaxParser.Split(string, SyntaxPair[], char, Span{SyntaxPair})"/>
    public SyntaxEnumerator Split(string input, Span<SyntaxPair> initialBuffer = default)
        => SyntaxParser.Split(input, fullSyntax, separator, initialBuffer);
#endif

    /// <inheritdoc cref="SyntaxParser.Split(ReadOnlySpan{char}, SyntaxPair[], char, Span{SyntaxPair})"/>
    public SyntaxEnumerator Split(ReadOnlySpan<char> input, Span<SyntaxPair> initialBuffer = default)
        => SyntaxParser.Split(input, fullSyntax, separator, initialBuffer);

#if NETFRAMEWORK
    /// <inheritdoc cref="SyntaxParser.SliceInBetween(string, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public ReadOnlySpan<char> SliceInBetween(string input, out ReadOnlySpan<char> remainder)
        => SyntaxParser.SliceInBetween(input, subsetOfSyntax, start, end, out remainder);
#endif

    /// <inheritdoc cref="SyntaxParser.SliceInBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public ReadOnlySpan<char> SliceInBetween(ReadOnlySpan<char> input, out ReadOnlySpan<char> remainder)
        => SyntaxParser.SliceInBetween(input, subsetOfSyntax, start, end, out remainder);
}