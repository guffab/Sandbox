namespace SyntaxParser;

/// <summary>
/// Enables enumerating each split within a <see cref="ReadOnlySpan{T}"/> that has been divided using a single separator.
/// </summary>
public ref struct SyntaxEnumerator
{
    private readonly ReadOnlySpan<char> _span;
    private readonly SyntaxSplitBlocker _splitBlocker;
    private readonly char _separator;

    private int _startCurrent = -1;
    private int _endCurrent = -1;

    const int separatorLength = 1;

    internal SyntaxEnumerator(ReadOnlySpan<char> span, ICollection<SyntaxPair> syntaxPairs, char separator)
    {
        _span = span;
        _splitBlocker = new SyntaxSplitBlocker(syntaxPairs);
        _separator = separator;
    }

    /// <summary>Gets an enumerator that allows for iteration over the split span.</summary>
    public SyntaxEnumerator GetEnumerator() => this; //do not rename (duck typing)

    /// <summary>The current element of the enumeration. This may only be called after a successful call to <see cref="MoveNext"/></summary>
    public ReadOnlySpan<char> Current => _span.Slice(_startCurrent, _endCurrent - _startCurrent); //do not rename (duck typing)

    /// <summary>
    /// Advances the enumerator to the next element of the enumeration.
    /// </summary>
    /// <returns><see langword="true"/> if the enumerator was successfully advanced to the next element; <see langword="false"/> if the enumerator has passed the end of the enumeration.</returns>
    public bool MoveNext() //do not rename (duck typing)
    {
        int startNext = _endCurrent + separatorLength;
        if (startNext > _span.Length)
            return false;

        _startCurrent = startNext;

        for (int i = startNext; i < _span.Length; i++)
        {
            var currentChar = _span[i];

            //note any blocks
            _splitBlocker.Process(currentChar);
            if (_splitBlocker.IsBlocking())
                continue;

            if (currentChar == _separator)
            {
                _endCurrent = i;
                return true;
            }
        }

        //last item (without separator)
        _endCurrent = _span.Length;
        return true;
    }
}
