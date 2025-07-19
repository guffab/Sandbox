namespace SyntaxScanner;

/// <summary>
/// Enables enumerating each split within a <see cref="ReadOnlySpan{T}"/> that has been divided by various different tokens.
/// </summary>
/// <remarks>
/// This may return partial tokens if the input is insufficient.
/// </remarks>
public ref struct SyntaxTokenSplitEnumerator
{
    private readonly ReadOnlySpan<char> _span;
    private SyntaxSplitBlocker _splitBlocker;
    private readonly string[] _supportedTokens;
    private HashSet<char> _tokenStarts;

    private int _startCurrent = -1;
    private int _endCurrent = -1;
    private bool _isToken = false;

    internal SyntaxTokenSplitEnumerator(ReadOnlySpan<char> span, SyntaxPair[] syntaxPairs, string[] supportedTokens, Span<SyntaxPair> initialBuffer)
    {
        _span = span;
        _supportedTokens = supportedTokens;
        _splitBlocker = new SyntaxSplitBlocker(syntaxPairs, initialBuffer);
        _tokenStarts = [.. supportedTokens.Select(x => x.FirstOrDefault())];
    }

    /// <summary>Gets an enumerator that allows for iteration over the split span.</summary>
    public SyntaxTokenSplitEnumerator GetEnumerator() => this; //do not rename (duck typing)

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

            //extract literal
            //find longest token match
            //extract token (size is minimum 1)
        }

        //last item
        _isToken = false;
        _endCurrent = _span.Length;

        return _startCurrent < _endCurrent;
    }
    
    private static int FindLongestTokenMatch(ReadOnlySpan<char> path, int cutpoint, string[] candidates)
    {
        int tokenLength = 1; //we already identified the precence of a token

        for (int j = 1; j < path.Length - cutpoint; j++)
        {
            bool anyfound = false;

            foreach (var candidate in candidates)
            {
                if (candidate.Length > j && candidate[j] == path[cutpoint + j])
                {
                    anyfound = true;
                    tokenLength++;
                    break;
                }
            }

            //there wont be any future match, so we can return early
            if (!anyfound)
                break;
        }
        return tokenLength;
    }
}
