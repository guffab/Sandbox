namespace SyntaxScanner;

/// <summary>
/// Enables enumerating each split within <see cref="ReadOnlySpan{T}"/> that consists of a token pair.
/// </summary>
/// <remarks>
/// Input may look like: 'Text' other 'Text' more 'Text' etc. <br/>
/// with `'` being both the opening and closing token.
/// </remarks>
public ref struct TokenPairSplitEnumerator
{
    private readonly ReadOnlySpan<char> _span;
    private SyntaxSplitBlocker _splitBlocker;
    private readonly SyntaxPair _tokenPair;

    private int _startCurrent;
    private int _endCurrent;
    private bool _isToken;

    internal TokenPairSplitEnumerator(ReadOnlySpan<char> span, SyntaxPair[] syntaxPairs, SyntaxPair tokenPair, Span<SyntaxPair> initialBuffer)
    {
        _span = span;
        _tokenPair = tokenPair;
        _splitBlocker = new SyntaxSplitBlocker(syntaxPairs, initialBuffer);
    }

    /// <summary>Gets an enumerator that allows for iteration over the split span.</summary>
    public TokenPairSplitEnumerator GetEnumerator() => this; //do not rename (duck typing)

    /// <summary>The current element of the enumeration. This may only be called after a successful call to <see cref="MoveNext"/></summary>
    public (int start, int end, bool isToken) Current => (!_isToken ? _startCurrent : _startCurrent + 1, _endCurrent, _isToken); //do not rename (duck typing)

    /// <summary>
    /// Advances the enumerator to the next element of the enumeration.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the enumeration.</returns>
    public bool MoveNext() //do not rename (duck typing)
    {
        int startNext = _endCurrent;
        if (_isToken) startNext++;
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

            //find start
            if (currentChar != _tokenPair.Start)
                continue;

            //extract literal (if any)
            if (startNext != i)
            {
                _endCurrent = i;
                _isToken = false;
                return true;
            }

            for (int j = i + 1; j < _span.Length; j++)
            {
                currentChar = _span[j];

                //note any blocks
                _splitBlocker.Process(currentChar);
                if (_splitBlocker.IsBlocking())
                    continue;

                //find end
                if (currentChar != _tokenPair.End)
                    continue;

                //extract inside of token pair
                _endCurrent = j;
                _isToken = true;
                return true;
            }
        }

        //last item
        _isToken = false;
        _endCurrent = _span.Length;

        return _startCurrent < _endCurrent;
    }
}
