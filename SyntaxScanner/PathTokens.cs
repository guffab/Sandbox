namespace SyntaxScanner;

public abstract record PathToken(PathToken? Next, TokenType TokenType)
{
    public virtual bool IsValid() => Next?.IsValid() ?? true;
}

public record ListToken(PathToken SubToken, PathToken? Next) : PathToken(Next, TokenType.List)
{
    public override bool IsValid()
    {
        return base.IsValid() && SubToken.IsValid();
    }
}

public record PropertyToken(string Name, PathToken? Next) : PathToken(Next, TokenType.Property);

public record CheckToken(string Value) : PathToken(null, TokenType.Check);

public record InvalidToken() : PathToken(null, TokenType.Invalid)
{
    public override bool IsValid() => false;
}

public enum TokenType
{
    Invalid = -1,

    Property = 0,

    List = 1,

    Check = 2,
}