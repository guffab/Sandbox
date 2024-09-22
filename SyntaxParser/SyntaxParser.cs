namespace SyntaxParser;

/// <summary>
/// Handles parsing and interpreting a generic <see cref="string"/> as language syntax.<br/>
/// The syntax identifiers itself are highly configurable by the caller.
/// </summary>
internal static class SyntaxParser
{
#if NETFRAMEWORK
    /// <inheritdoc cref="Split(ReadOnlySpan{char}, ICollection{SyntaxPair}, char)"/>
    public static SyntaxEnumerator Split(string span, ICollection<SyntaxPair> syntaxPairs, char separator)
        => Split(span.AsSpan(), syntaxPairs, separator);
#endif
    /// <summary>
    /// Performs syntax-aware splitting on a span of characters.
    /// </summary>
    /// <param name="span">The characters to split.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="separator">The separator to split on.</param>
    public static SyntaxEnumerator Split(ReadOnlySpan<char> span, ICollection<SyntaxPair> syntaxPairs, char separator)
        => new SyntaxEnumerator(span, syntaxPairs, separator);

    /// <summary>
    /// Performs syntax-aware slicing between <paramref name="start"/> and <paramref name="end"/>.
    /// </summary>
    /// <param name="span">The characters to slice.</param>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    /// <param name="start">The start of the slice.</param>
    /// <param name="end">The end of the slice.</param>
    /// <remarks>
    /// The slice will only be performed on the outer-most <paramref name="start"/> and <paramref name="end"/>.<br/>
    /// Slicing may therefore fail if the amount of both does not add up.
    /// </remarks>
    public static ReadOnlySpan<char> SliceInBetween(ReadOnlySpan<char> span, ICollection<SyntaxPair> syntaxPairs, char start, char end)
    {
        var splitBlocker = new SyntaxSplitBlocker(syntaxPairs);

        //find start
        int splitStart = span.Length;
        for (int i = 0; i < span.Length; i++)
        {
            var currentChar = span[i];

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

        //find end
        for (int i = splitStart; i < span.Length; i++)
        {
            var currentChar = span[i];

            //note any blocks
            splitBlocker.Process(currentChar);
            if (splitBlocker.IsBlocking())
                continue;

            //find additional openers (this will prevent splitting until they are also closed)
            if (currentChar == start)
            {
                surplusOpeners++;
                continue;
            }

            if (currentChar == end)
            {
                //close all openers first
                if (surplusOpeners is 0)
                    return span[splitStart..i];
                else
                    surplusOpeners--;
            }
        }
        return "";
    }

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
        public ReadOnlySpan<char> Current => _span[new Range(_startCurrent, _endCurrent)]; //do not rename (duck typing)

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

    /// <summary>
    /// An object used to look for syntax identifiers and possibly prevent splitting a string/span at the current position.
    /// </summary>
    /// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
    public readonly ref struct SyntaxSplitBlocker(ICollection<SyntaxPair> syntaxPairs)
    {
        private readonly ICollection<SyntaxPair> syntaxPairs = syntaxPairs;
        private readonly Stack<SyntaxPair> existingSyntax = new();

        /// <summary>
        /// The amount of blockers currently present.
        /// </summary>
        public readonly int Count => existingSyntax.Count;

        /// <summary>
        /// Identifies wether this object is currently preventing a split operation.
        /// </summary>
        public readonly bool IsBlocking()
        {
            return existingSyntax.Count is not 0;
        }

        /// <summary>
        /// Processes the next char. This may add or remove a lock.
        /// </summary>
        public readonly void Process(in char c)
        {
            //complete existing syntax first (this conveniently also handles syntax where [start == end])
            foreach (var syntax in syntaxPairs)
            {
                if (c == syntax.End)
                {
                    if (TryFinish(syntax))
                        return;

                    break;
                }
            }

            //start new syntax
            foreach (var syntax in syntaxPairs)
            {
                if (c == syntax.Start)
                {
                    existingSyntax.Push(syntax);
                    return;
                }
            }
        }

        /// <summary>
        /// Finishes the last occurence of this <paramref name="syntax"/> if applicable.
        /// </summary>
        private readonly bool TryFinish(in SyntaxPair syntax)
        {
            int depth = 0;

            foreach (var incompleteSyntax in existingSyntax)
            {
                if (syntax.Priority < incompleteSyntax.Priority)
                    return false;

                depth++;

                //complete first occurence of current syntax
                if (incompleteSyntax.End == syntax.End)
                {
                    for (int i = 0; i < depth; i++)
                        existingSyntax.Pop();

                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// The character pair enclosing a new kind of syntax.
    /// </summary>
    /// <param name="Start">The start of a new syntax.</param>
    /// <param name="End">The end of a new syntax.</param>
    /// <param name="Priority">The priority of this syntax over another in case of a conflict.</param>
    public readonly record struct SyntaxPair(char Start, char End, int Priority);
}
