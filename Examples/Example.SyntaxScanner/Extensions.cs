using SyntaxScanner;

public static class SyntaxExtensions
{
    /// <inheritdoc cref="SyntaxView.IndexOf(string, SyntaxPair[], char)"/>
    public static int IndexOf(this string input, char value, SyntaxPair[] syntaxPairs)
        => SyntaxView.IndexOf(input, syntaxPairs, value);

    /// <inheritdoc cref="SyntaxView.IndexOf(ReadOnlySpan{char}, SyntaxPair[], char)"/>
    public static int IndexOf(this ReadOnlySpan<char> input, char value, SyntaxPair[] syntaxPairs)
        => SyntaxView.IndexOf(input, syntaxPairs, value);

    /// <inheritdoc cref="SyntaxView.Split(string, SyntaxPair[], char, Span{SyntaxPair})"/>
    public static SyntaxEnumerator Split(this string input, char separator, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.Split(input, syntaxPairs, separator, initialBuffer);

    /// <inheritdoc cref="SyntaxView.Split(ReadOnlySpan{char}, SyntaxPair[], char, Span{SyntaxPair})"/>
    public static SyntaxEnumerator Split(this ReadOnlySpan<char> input, char separator, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.Split(input, syntaxPairs, separator, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitByTokens(string, SyntaxPair[], string[], Span{SyntaxPair})"/>
    public static SyntaxTokenSplitEnumerator SplitByTokens(this string input, string[] supportedTokens, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitByTokens(input, syntaxPairs, supportedTokens, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitByTokens(ReadOnlySpan{char}, SyntaxPair[], string[], Span{SyntaxPair})"/>
    public static SyntaxTokenSplitEnumerator SplitTokenized(this ReadOnlySpan<char> input, string[] supportedTokens, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitByTokens(input, syntaxPairs, supportedTokens, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitByTokenPair(string, SyntaxPair[], SyntaxPair, Span{SyntaxPair})"/>1
    public static TokenPairSplitEnumerator SplitTokenized(this string input, SyntaxPair tokenPair, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitByTokenPair(input, syntaxPairs, tokenPair, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SplitByTokenPair(ReadOnlySpan{char}, SyntaxPair[], SyntaxPair, Span{SyntaxPair})"/>1
    public static TokenPairSplitEnumerator SplitTokenized(this ReadOnlySpan<char> input, SyntaxPair tokenPair, SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
        => SyntaxView.SplitByTokenPair(input, syntaxPairs, tokenPair, initialBuffer);

    /// <inheritdoc cref="SyntaxView.SliceInBetween(string, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceInBetween(this string input, char start, char end, SyntaxPair[] syntaxPairs, out ReadOnlySpan<char> remainder)
        => SyntaxView.SliceInBetween(input, syntaxPairs, start, end, out remainder);

    /// <inheritdoc cref="SyntaxView.SliceInBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(this ReadOnlySpan<char> input, char start, char end, SyntaxPair[] syntaxPairs, out ReadOnlySpan<char> remainder)
        => SyntaxView.SliceInBetween(input, syntaxPairs, start, end, out remainder);

    /// <inheritdoc cref="SyntaxView.SliceInBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(this ReadOnlySpan<char> input, char start, char end, SyntaxPair[] syntaxPairs)
        => SyntaxView.SliceInBetween(input, syntaxPairs, start, end, out _);
}