namespace SyntaxScanner;

/// <summary>
/// Enables enumerating each split within a <see cref="ReadOnlySpan{T}"/> that has been divided by various different tokens.
/// </summary>
/// <remarks>
/// This may return partial tokens if the input is insufficient.
/// </remarks>
public ref struct TokenListSplitEnumerator
{
    private readonly ReadOnlySpan<char> _span;
    private SyntaxSplitBlocker _splitBlocker;
    private readonly string[] _supportedTokens;
    private HashSet<char> _tokenStarts;

    private int _startCurrent;
    private int _endCurrent;
    private bool _isToken;

    internal TokenListSplitEnumerator(ReadOnlySpan<char> span, SyntaxPair[] syntaxPairs, string[] supportedTokens, Span<SyntaxPair> initialBuffer)
    {
        _span = span;
        _supportedTokens = supportedTokens;
        _splitBlocker = new SyntaxSplitBlocker(syntaxPairs, initialBuffer);
        _tokenStarts = [.. supportedTokens.Select(x => x.FirstOrDefault())];

        if (supportedTokens.Any(x => x.Length > 2))
            throw new ArgumentException($"Tokens longer than two characters are currently not supported. Adapt method {nameof(FindLongestTokenMatch)} if needed.", nameof(supportedTokens));
    }

    /// <summary>Gets an enumerator that allows for iteration over the split span.</summary>
    public TokenListSplitEnumerator GetEnumerator() => this; //do not rename (duck typing)

    /// <summary>The current element of the enumeration. This may only be called after a successful call to <see cref="MoveNext"/></summary>
    public (int start, int end, bool isToken) Current => (_startCurrent, _endCurrent, _isToken); //do not rename (duck typing)

    /// <summary>
    /// Advances the enumerator to the next element of the enumeration.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the enumeration.</returns>
    public bool MoveNext() //do not rename (duck typing)
    {
        int startNext = _endCurrent;
        if (startNext >= _span.Length)
            return false;

        _startCurrent = startNext;

        for (int i = startNext; i < _span.Length; i++)
        {
            var currentChar = _span[i];

            //note any blocks
            _splitBlocker.Process(currentChar);
            if (_splitBlocker.IsBlocking())
                continue;

            //find expected tokens (even if they are incomplete)
            if (!_tokenStarts.Contains(currentChar))
                continue;

            //extract literal (if any)
            if (startNext != i)
            {
                _endCurrent = i;
                _isToken = false;
                return true;
            }

            //extract longest token match (minimum 1 char)
            int tokenLength = FindLongestTokenMatch(_span.Slice(i), _supportedTokens);
            _endCurrent = i + tokenLength;
            _isToken = true;
            return true;
        }

        //last item
        _isToken = false;
        _endCurrent = _span.Length;

        return _startCurrent < _endCurrent;
    }

    private static int FindLongestTokenMatch(ReadOnlySpan<char> span, string[] supportedTokens)
    {
        //check if any candidate matches both chars
        foreach (var candidate in supportedTokens)
        {
            if (candidate[0] != span[0])
                continue;

            if (candidate.Length > 1 && candidate[1] == span[1])
                return 2;
        }
        return 1; //default to one char token
    }
}
