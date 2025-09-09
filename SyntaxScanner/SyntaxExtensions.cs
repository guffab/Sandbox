namespace SyntaxScanner;

public static class SyntaxExtensions
{
#if NETFRAMEWORK
    /// <inheritdoc cref="SyntaxView.IndexOf(string, SyntaxPair[], char)"/>
    public static int IndexOf(this string input, char value, SyntaxPair[] syntaxPairs)
        => SyntaxView.IndexOf(input, syntaxPairs, value);

    /// <inheritdoc cref="SyntaxView.Split(string, SyntaxPair[], char, Span{SyntaxPair})"/>
    public static SyntaxEnumerator Split(this string input, char separator, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.Split(input, syntaxPairs, separator, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitTokenized(string, SyntaxPair[], string[], Span{SyntaxPair})"/>
    public static TokenListSplitEnumerator SplitTokenized(this string input, string[] supportedTokens, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitTokenized(input, syntaxPairs, supportedTokens, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitTokenized(string, SyntaxPair[], SyntaxPair, Span{SyntaxPair})"/>1
    public static TokenPairSplitEnumerator SplitTokenized(this string input, SyntaxPair tokenPair, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitTokenized(input, syntaxPairs, tokenPair, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SliceBetween(string, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(this string input, char start, char end, SyntaxPair[] syntaxPairs, out ReadOnlySpan<char> remainder)
        => SyntaxView.SliceBetween(input, syntaxPairs, start, end, out remainder);

    /// <inheritdoc cref="SyntaxView.SliceBetween(string, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(this string input, char start, char end, SyntaxPair[] syntaxPairs)
        => SyntaxView.SliceBetween(input, syntaxPairs, start, end);
#endif

    /// <inheritdoc cref="SyntaxView.IndexOf(ReadOnlySpan{char}, SyntaxPair[], char)"/>
    public static int IndexOf(this ReadOnlySpan<char> input, char value, SyntaxPair[] syntaxPairs)
        => SyntaxView.IndexOf(input, syntaxPairs, value);

    /// <inheritdoc cref="SyntaxView.Split(ReadOnlySpan{char}, SyntaxPair[], char, Span{SyntaxPair})"/>
    public static SyntaxEnumerator Split(this ReadOnlySpan<char> input, char separator, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.Split(input, syntaxPairs, separator, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitTokenized(ReadOnlySpan{char}, SyntaxPair[], string[], Span{SyntaxPair})"/>
    public static TokenListSplitEnumerator SplitTokenized(this ReadOnlySpan<char> input, string[] supportedTokens, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitTokenized(input, syntaxPairs, supportedTokens, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitTokenized(ReadOnlySpan{char}, SyntaxPair[], SyntaxPair, Span{SyntaxPair})"/>1
    public static TokenPairSplitEnumerator SplitTokenized(this ReadOnlySpan<char> input, SyntaxPair tokenPair, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitTokenized(input, syntaxPairs, tokenPair, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SliceBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(this ReadOnlySpan<char> input, char start, char end, SyntaxPair[] syntaxPairs, out ReadOnlySpan<char> remainder)
        => SyntaxView.SliceBetween(input, syntaxPairs, start, end, out remainder);

    /// <inheritdoc cref="SyntaxView.SliceBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(this ReadOnlySpan<char> input, char start, char end, SyntaxPair[] syntaxPairs)
        => SyntaxView.SliceBetween(input, syntaxPairs, start, end, out _);
}