namespace SyntaxParser;

/// <summary>
/// An object used to look for syntax identifiers and possibly prevent splitting a string/span at the current position.
/// </summary>
/// <param name="syntaxPairs">The supported syntax identifiers to look out for.</param>
/// <param name="initialBuffer">A buffer that will be used as internal storage.</param>
public ref struct SyntaxSplitBlocker(SyntaxPair[] syntaxPairs, Span<SyntaxPair> initialBuffer = default)
{
    private readonly SyntaxPair[] syntaxPairs = syntaxPairs;
    private SyntaxStack syntaxStack = new(initialBuffer);

    /// <summary>
    /// Identifies wether this object is currently preventing a split operation.
    /// </summary>
    public readonly bool IsBlocking()
    {
        return syntaxStack.Length is not 0;
    }

    /// <summary>
    /// Processes the next char. This may add or remove a lock.
    /// </summary>
    public void Process(in char c)
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
                bool isPriorityHighEnough = !syntaxStack.TryPeek(out var previous) //empty stack
                                            || syntax.Priority > previous.Priority; //higher priority than last syntax (closing the previous would close this one either way)

                if (isPriorityHighEnough)
                    syntaxStack.Push(syntax);

                return;
            }
        }
    }

    /// <summary>
    /// Finishes the last occurence of this <paramref name="syntax"/> if applicable.
    /// </summary>
    private bool TryFinish(in SyntaxPair syntax)
    {
        int depth = 0;

        foreach (var incompleteSyntax in syntaxStack)
        {
            if (syntax.Priority < incompleteSyntax.Priority)
                return false;

            depth++;

            //complete first occurence of current syntax
            if (incompleteSyntax.End == syntax.End)
            {
                for (int i = 0; i < depth; i++)
                    syntaxStack.Pop();

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
