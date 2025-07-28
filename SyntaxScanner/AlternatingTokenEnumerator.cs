namespace SyntaxScanner;

/// <summary>
/// Enables enumerating each split within a <see cref="ReadOnlySpan{T}"/> that consists of alternating tokens.
/// </summary>
/// <remarks>
/// Input may look like: 'Text' other 'Text' more 'Text' etc. <br/>
/// with `'` being both the opening and closing (alternating) token.
/// </remarks>
public ref struct AlternatingTokenEnumerator
{
    private readonly ReadOnlySpan<char> _span;
    private SyntaxSplitBlocker _splitBlocker;
    private readonly SyntaxPair _alternatingTokens;

    private int _startCurrent;
    private int _endCurrent;
    private bool _isToken;

    internal AlternatingTokenEnumerator(ReadOnlySpan<char> span, SyntaxPair[] syntaxPairs, SyntaxPair alternatingTokens, Span<SyntaxPair> initialBuffer)
    {
        _span = span;
        _alternatingTokens = alternatingTokens;
        _splitBlocker = new SyntaxSplitBlocker(syntaxPairs, initialBuffer);
    }

    /// <summary>Gets an enumerator that allows for iteration over the split span.</summary>
    public AlternatingTokenEnumerator GetEnumerator() => this; //do not rename (duck typing)

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
        bool startFound = false;

        for (int i = startNext; i < _span.Length; i++)
        {
            var currentChar = _span[i];

            //note any blocks
            _splitBlocker.Process(currentChar);
            if (_splitBlocker.IsBlocking())
                continue;

#warning take SliceInBetween as starting point

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
}
