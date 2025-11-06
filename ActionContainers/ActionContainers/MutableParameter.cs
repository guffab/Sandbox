using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Represents an <see cref="IParameter"/> that can be freely mutated.
/// </summary>
/// <remarks>
/// The data structure is re-built on every call to have all downstream changes bubble up immediately.
/// </remarks>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
[DebuggerTypeProxy(typeof(ParameterDebugView))]
public class MutableParameter(ParameterTemplateNode parameterNode, MutableAction parent) : IParameter
{
    const char separatorChar = ';';

    private readonly ParameterTemplateNode BackingNode = parameterNode;

    /// <inheritdoc/>
    public string Id { get => BackingNode.Id; set => BackingNode.Id = value; }

    /// <inheritdoc/>
    public StringBoolOrDouble Value { get => BackingNode.Parent[ParentAction.TypeName, BackingNode.Id]; set => BackingNode.Parent[ParentAction.TypeName, BackingNode.Id] = value; }

    /// <inheritdoc cref="ActionContainers.Unit"/>
    public Unit Unit => BackingNode.Unit;

    /// <inheritdoc cref="IParameter.ParentAction"/>
    public MutableAction ParentAction { get; } = parent;

    /// <inheritdoc cref="IParameter.SubAction"/>
    public MutableAction? SubAction
    {
        get => SubActions.FirstOrDefault();
        set => SubActions = value is null ? [] : [value];
    }

    /// <inheritdoc cref="IParameter.SubActions"/>
    public List<MutableAction> SubActions
    {
        get => ActionNodePool.Instance.GetTypeNodes(((string)Value).Split(separatorChar)).Select(x => new MutableAction(x, this)).ToList();
        set => Value = string.Join(separatorChar, value.Select(x => x.Id));
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    IAction? IParameter.ParentAction => ParentAction;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    IAction? IParameter.SubAction => SubAction;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    List<IAction> IParameter.SubActions => [.. SubActions];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    [ExcludeFromCodeCoverage]
    string IParameter.Value => Value;

    /// <inheritdoc/>
    public bool TryGetValue<TEnum>(out TEnum value) where TEnum : struct, Enum
    {
        return Enum.TryParse(Value, out value);
    }

    /// <inheritdoc/>
    public bool TryGetValue(out bool value)
    {
        //null-safe value parsing
        if (TryGetValue(out int intValue))
        {
            value = intValue is 1;
            return true;
        }

        //not found
        value = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetValue(out double value)
    {
        //null-safe value parsing
        if (TryGetValue(out string strValue))
            return double.TryParse(strValue, out value);

        //not found
        value = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetValue(out int value)
    {
        //null-safe value parsing
        if (TryGetValue(out string strValue))
            return int.TryParse(strValue, out value);

        //not found
        value = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetValue(out string value)
    {
        value = Value;
        return true;
    }

    /// <inheritdoc/>
    public bool TryGetValue(out IList<double> value)
    {
        //null-safe value parsing
        if (TryGetValue(out string strValue))
        {
            var stringValues = strValue.Split(separatorChar);

            //parse strings to target type
            value = new List<double>();
            foreach (var stringValue in stringValues)
            {
                if (double.TryParse(stringValue, out var doubleValue))
                    value.Add(doubleValue);
            }
            return true;
        }

        //not found
        value = Array.Empty<double>();
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetValue(out IList<string> value)
    {
        //null-safe value parsing
        if (TryGetValue(out string strValue))
        {
            value = strValue.Split(separatorChar);
            return true;
        }

        //not found
        value = Array.Empty<string>();
        return false;
    }

    /// <summary>
    /// Represents a lightweight union of (string, bool, double).
    /// </summary>
    // C# will gain support for actual unions somewhere in the next 5 years, so this is a quick compromise.
    public readonly struct StringBoolOrDouble(string value)
    {
        private readonly string value = value ?? "";

        public static implicit operator string(StringBoolOrDouble sbd) => sbd.value;
        public static implicit operator bool(StringBoolOrDouble sbd) => sbd.value is "1" ? true : false;
        public static implicit operator double(StringBoolOrDouble sbd) => double.TryParse(sbd.value, out double result) ? result : 0;

        public static implicit operator StringBoolOrDouble(string s) => new(s);
        public static implicit operator StringBoolOrDouble(bool b) => new(b ? "1" : "0");
        public static implicit operator StringBoolOrDouble(double d) => new(d.ToString());

        public static bool operator ==(StringBoolOrDouble a, StringBoolOrDouble b) => a.value == b.value;
        public static bool operator !=(StringBoolOrDouble a, StringBoolOrDouble b) => a.value != b.value;

        public override bool Equals(object? obj) => obj is StringBoolOrDouble sbd && sbd == this;
        public override int GetHashCode() => value.GetHashCode();
    }
}
