namespace SyntaxParser;

/// <summary>
/// An object used to look for syntax identifiers and possibly prevent splitting a string/span at the current position.
/// </summary>
/// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
public readonly ref struct SyntaxSplitBlocker(SyntaxPair[] syntaxPairs)
{
    private readonly SyntaxPair[] syntaxPairs = syntaxPairs;
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
