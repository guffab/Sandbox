namespace Example.SyntaxScanner;

/// <summary>
/// Represents a user-defined object path to traverse at run-time.
/// </summary>
public interface IPathItem
{
    /// <summary>
    /// Defines the start of the content of this <see cref="IFormula"/> in the input string.
    /// </summary>
    int Start { get; }

    /// <summary>
    /// Defines the length of the content of this <see cref="IFormula"/> in the input string.
    /// </summary>
    int Length { get; }
}

public abstract record PathItem(PathItem? Next, int Start, int Length) : IPathItem
{
    /// <summary>
    /// Indicates if this item (or any of its children) contains invalid code. An interpreter may save itself a lot of work by checking this at the root level.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if this item (and all of its children) are valid; otherwise <see langword="false"/>.
    /// </returns>
    public virtual bool IsValid() => Next?.IsValid() ?? true;

    /// <summary>
    /// Creates a flat representation of the syntax tree with its elements sorted by their position in the input string.
    /// </summary>
    public List<PathItem> Flatten()
    {
        var result = new List<PathItem>();
        FillRec(result, this);
        return result.OrderBy(x => x.Start).ThenBy(x => x.Length).ToList();

        static void FillRec(List<PathItem> result, PathItem? pathItem)
        {
            if (pathItem is null)
                return;

            result.Add(pathItem);
            FillRec(result, pathItem.Next);

            if (pathItem is ListItem li)
                FillRec(result, li.SubItem);
        }
    }
}

public record ListItem(PathItem SubItem, PathItem? Next) : PathItem(Next, 0, 0)
{
    public override bool IsValid()
    {
        var last = SubItem;
        while (last.Next is not null)
            last = last.Next;

        return base.IsValid() && SubItem.IsValid() && (SubItem is PropertyItem && last is CheckItem); //without a check on a property, we wouldn't know what item to select
    }
}

public record PropertyItem(string Name, int Start, PathItem? Next) : PathItem(Next, Start, Name.Length)
{
    //enables a more readable parser file
    public PropertyItem(ReadOnlySpan<char> Name, int Start, PathItem? Next) : this(Name.Trim().ToString(), Start  + ObjectPathParser.CountLeadingWhiteSpace(Name), Next) { }

    public bool IsNameValid()
    {
        return Length > 0 && !Name.Any(char.IsWhiteSpace);
    }

    public override bool IsValid()
    {
        return IsNameValid() && base.IsValid();
    }
}

public record CheckItem(string Value, int Start) : PathItem(null, Start, Value.Length)
{
    //enables a more readable parser file
    public CheckItem(ReadOnlySpan<char> Value, int Start) : this(Value.ToString(), Start) { }

    public override bool IsValid()
    {
        return !string.IsNullOrEmpty(Value);
    }
}

public record InvalidItem(int Start, int Length) : PathItem(null, Start, Length)
{
    public override bool IsValid() => false;
}
