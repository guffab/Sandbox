namespace SyntaxParser;

public partial class SyntaxParser
{
#if NETFRAMEWORK
    /// <inheritdoc cref="Split(ReadOnlySpan{char}, SyntaxPair[], char, Span{SyntaxPair})"/>
    public static SyntaxEnumerator Split(string input, SyntaxPair[] syntaxPairs, char separator, Span<SyntaxPair> initialBuffer = default)
        => Split(input.AsSpan(), syntaxPairs, separator, initialBuffer);
#endif

    /// <summary>
    /// Performs syntax-aware splitting on a span of characters.
    /// </summary>
    /// <param name="input">The characters to split.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="separator">The separator to split on.</param>
    /// <param name="initialBuffer">A buffer that will be used as internal storage.</param>
    public static SyntaxEnumerator Split(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, char separator, Span<SyntaxPair> initialBuffer = default)
        => new SyntaxEnumerator(input, syntaxPairs, separator, initialBuffer);

#if NETFRAMEWORK
    /// <inheritdoc cref="SliceInBetween(ReadOnlySpan{char}, SyntaxPair[], char, char, out ReadOnlySpan{char})"/>
    public static ReadOnlySpan<char> SliceInBetween(string input, SyntaxPair[] syntaxPairs, char start, char end, out ReadOnlySpan<char> remainder)
        => SliceInBetween(input.AsSpan(), syntaxPairs, start, end, out remainder);
#endif

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
    public static ReadOnlySpan<char> SliceInBetween(ReadOnlySpan<char> input, SyntaxPair[] syntaxPairs, char start, char end, out ReadOnlySpan<char> remainder)
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
