namespace SyntaxScanner;

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
public class SyntaxView
{
#if NETFRAMEWORK
    /// <inheritdoc cref="Split(ReadOnlySpan{char}, SyntaxPair[], char, Span{SyntaxPair})"/>
    public static SyntaxEnumerator Split(string input, SyntaxPair[] syntaxPairs, char separator, Span<SyntaxPair> initialBuffer = default)
        => Split(input.AsSpan(), syntaxPairs, separator, initialBuffer);

    /// <inheritdoc cref="SplitTokenized(ReadOnlySpan{char}, SyntaxPair[], string[], Span{SyntaxPair})"/>
    public static TokenListSplitEnumerator SplitTokenized(string input, SyntaxPair[] syntaxPairs, string[] supportedTokens, Span<SyntaxPair> initialBuffer = default)
        => SplitTokenized(input.AsSpan(), syntaxPairs, supportedTokens, initialBuffer);

    /// <inheritdoc cref="SplitTokenized(ReadOnlySpan{char}, SyntaxPair[], SyntaxPair, Span{SyntaxPair})"/>1
    public static TokenPairSplitEnumerator SplitTokenized(string input, SyntaxPair[] syntaxPairs, SyntaxPair tokenPair, Span<SyntaxPair> initialBuffer = default)
        => SplitTokenized(input.AsSpan(), syntaxPairs, tokenPair, initialBuffer);

    /// <inheritdoc cref="IndexOf(ReadOnlySpan{char}, SyntaxPair[], char)"/>
    public static int IndexOf(string input, SyntaxPair[] syntaxPairs, char value)
        => IndexOf(input.AsSpan(), syntaxPairs, value);

    /// <inheritdoc cref="SliceBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(string input, SyntaxPair[] syntaxPairs, char start, char end)
        => SliceBetween(input.AsSpan(), syntaxPairs, start, end, out _);

    /// <inheritdoc cref="SliceBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(string input, SyntaxPair[] syntaxPairs, char start, char end, out ReadOnlySpan<char> remainder)
        => SliceBetween(input.AsSpan(), syntaxPairs, start, end, out remainder);
#endif

    /// <summary>
    /// Performs syntax-aware splitting on a span of characters.
    /// </summary>
    /// <param name="input">The characters to split.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="separator">The separator to split on.</param>
    /// <param name="initialBuffer">A buffer that will be used as internal storage. For stack-allocated arrays, this should be kept well below a length of 128 (adding up to ~1 kb)</param>
    public static SyntaxEnumerator Split(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, char separator, Span<SyntaxPair> initialBuffer = default)
        => new(input, syntaxPairs, separator, initialBuffer);

    /// <summary>
    /// Performs syntax-aware token/literal splitting on a span of characters.
    /// </summary>
    /// <param name="input">The characters to split.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="supportedTokens">The tokens to split on and return. Tokens are currently limited to up to two chars.</param>
    /// <param name="initialBuffer">A buffer that will be used as internal storage. For stack-allocated arrays, this should be kept well below a length of 128 (adding up to ~1 kb)</param>
    /// <remarks>
    /// This may return partial tokens if the input is insufficient.
    /// </remarks>
    public static TokenListSplitEnumerator SplitTokenized(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, string[] supportedTokens, Span<SyntaxPair> initialBuffer = default)
        => new(input, syntaxPairs, supportedTokens, initialBuffer);

    /// <summary>
    /// Performs syntax-aware splitting around a token pair.
    /// </summary>
    /// <param name="input">The characters to split.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="tokenPair">The token pair to split on and return.</param>
    /// <param name="initialBuffer">A buffer that will be used as internal storage. For stack-allocated arrays, this should be kept well below a length of 128 (adding up to ~1 kb)</param>
    /// <remarks>
    /// This may return partial tokens if the input is insufficient.
    /// </remarks>1
    public static TokenPairSplitEnumerator SplitTokenized(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, SyntaxPair tokenPair, Span<SyntaxPair> initialBuffer = default)
        => new(input, syntaxPairs, tokenPair, initialBuffer);

    /// <summary>
    /// Performs a syntax-aware search for the <paramref name="value"/> parameter.
    /// </summary>
    /// <param name="input">The characters to search in.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="value">The value to find in the input chars.</param>
    /// <returns>The zero-based index position of the <paramref name="value"/> parameter from the start of the current instance if that char is found, or -1 if it is not.</returns>
    public static int IndexOf(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, char value)
    {
        var blocker = new SyntaxSplitBlocker(syntaxPairs, stackalloc SyntaxPair[64]);

        for (int i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];

            blocker.Process(currentChar);
            if (blocker.IsBlocking())
                continue;

            if (currentChar == value)
                return i;
        }
        return -1;
    }

    /// <inheritdoc cref="SliceBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceBetween(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, char start, char end)
        => SliceBetween(input, syntaxPairs, start, end, out _);

    /// <summary>
    /// Performs syntax-aware slicing between <paramref name="start"/> and <paramref name="end"/>.
    /// </summary>
    /// <param name="input">The characters to slice.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="start">The start of the slice.</param>
    /// <param name="end">The end of the slice.</param>
    /// <param name="remainder">The right-hand side of the slice OR the <paramref name="input"/> if the operation fails.</param>
    /// <remarks>
    /// The slice will only be performed on the outer-most <paramref name="start"/> and <paramref name="end"/>.<br/>
    /// Slicing may therefore fail if the amount of both does not add up.
    /// </remarks>
    public static ReadOnlySpan<char> SliceBetween(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, char start, char end, out ReadOnlySpan<char> remainder)
    {
        var splitBlocker = new SyntaxSplitBlocker(syntaxPairs, stackalloc SyntaxPair[64]);

        //find start
        int splitStart = input.Length;
        for (int i = 0; i < input.Length; i++)
        {
            var currentChar = input[i];

            //note any blocks
            splitBlocker.Process(currentChar);
            if (splitBlocker.IsBlocking())
                continue;

            if (currentChar == start)
            {
                splitStart = i + 1;
                break;
            }
        }

        int surplusOpeners = 0;
        bool startIsEnd = start == end;

        //find end
        for (int i = splitStart; i < input.Length; i++)
        {
            var currentChar = input[i];

            //note any blocks
            splitBlocker.Process(currentChar);
            if (splitBlocker.IsBlocking())
                continue;

            //find additional openers (this will prevent splitting until they are also closed)
            if (!startIsEnd && currentChar == start)
            {
                surplusOpeners++;
                continue;
            }

            if (currentChar == end)
            {
                //close all openers first
                if (surplusOpeners is not 0)
                    surplusOpeners--;
                else
                {
                    remainder = input.Slice(i + 1);
                    return input.Slice(splitStart, i - splitStart);
                }
            }
        }

        remainder = input;
        return [];
    }
}
