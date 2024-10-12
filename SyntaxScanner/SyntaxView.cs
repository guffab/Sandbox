namespace SyntaxParser;

/// <summary>
/// Creates syntax-enhanced views over generic <see cref="string"/> input.<br/>
/// </summary>
/// <param name="syntax">The full syntax to take into account.</param>
/// <param name="separator">The separator to split on.</param>
/// <param name="start">The start of a slice.</param>
/// <param name="end">The end of a slice.</param>
/// <remarks>
/// The syntax itself is fully configurable by the caller.
/// </remarks>
public partial class SyntaxView(SyntaxPair[] syntax, char separator, char start, char end)
{
    private readonly SyntaxPair[] fullSyntax = syntax;
    private readonly SyntaxPair[] subsetOfSyntax = syntax.Where(x => x.Start != start).ToArray();
    private readonly char separator = separator;
    private readonly char start = start;
    private readonly char end = end;

#if NETFRAMEWORK
    /// <inheritdoc cref="SyntaxView.Split(string, SyntaxPair[], char, Span{SyntaxPair})"/>
    public SyntaxEnumerator Split(string input, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.Split(input, fullSyntax, separator, initialBuffer);
#endif

    /// <inheritdoc cref="SyntaxView.Split(ReadOnlySpan{char}, SyntaxPair[], char, Span{SyntaxPair})"/>
    public SyntaxEnumerator Split(ReadOnlySpan<char> input, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.Split(input, fullSyntax, separator, initialBuffer);

#if NETFRAMEWORK
    /// <inheritdoc cref="SyntaxView.SliceInBetween(string, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public ReadOnlySpan<char> SliceInBetween(string input, out ReadOnlySpan<char> remainder)
        => SyntaxView.SliceInBetween(input, subsetOfSyntax, start, end, out remainder);
#endif

    /// <inheritdoc cref="SyntaxView.SliceInBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public ReadOnlySpan<char> SliceInBetween(ReadOnlySpan<char> input, out ReadOnlySpan<char> remainder)
        => SyntaxView.SliceInBetween(input, subsetOfSyntax, start, end, out remainder);
}