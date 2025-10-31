using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Represents a single type of an action that can be freely mutated.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
[DebuggerTypeProxy(typeof(ActionDebugView))]
public class MutableAction(ActionTypeNode actionNode, MutableParameter? parent) : IAction
{
    private readonly ActionTypeNode BackingNode = actionNode;

    /// <inheritdoc/>
    public string Id => $"{ActionName}_@_{TypeName}";

    /// <inheritdoc/>
    public string ActionName { get => BackingNode.Parent.Id; set => BackingNode.Parent.Id = value; }

    /// <inheritdoc/>
    public string TypeName { get => BackingNode.Id; set => BackingNode.Id = value; }

    /// <inheritdoc cref="IAction.Parameters"/>
    public List<MutableParameter> Parameters => BackingNode.Parent.Parameters.Select(x => new MutableParameter(x, this)).ToList();

    /// <inheritdoc cref="IAction.ParentParameter"/>
    public MutableParameter? ParentParameter { get; init; } = parent;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IParameter> IAction.Parameters => [.. Parameters];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IParameter? IAction.ParentParameter => ParentParameter;

    /// <inheritdoc cref="AddParameter(string, Unit, string)"/>
    public void AddParameter(string id, string value = "")
    {
        AddParameter(id, default, value);
    }

    /// <inheritdoc cref="AddParameter(string, Unit, string)"/>
    public void AddParameter(string id, bool value)
    {
        AddParameter(id, Unit.Bool, value ? "1" : "0");
    }

    /// <inheritdoc cref="AddParameter(string, Unit, string)"/>
    public void AddParameter(string id, Unit unit, double value)
    {
        AddParameter(id, unit, value.ToString());
    }

    /// <summary>
    /// Adds a parameter with the given <paramref name="id"/>.
    /// </summary>
    /// <param name="id">A unique id.</param>
    /// <param name="unit">The data type this parameter holds. Keep in mind that this can't be changed later!</param>
    /// <param name="value">The initial value to assign (applies to this ActionType only).</param>
    /// <exception cref="InvalidOperationException"/>
    public void AddParameter(string id, Unit unit, string value)
    {
        BackingNode.Parent.AddParameter(id, unit);
        BackingNode[id] = value ?? "";
    }

    /// <summary>
    /// Removes the parameter with the given <paramref name="id"/> from all types of this action.
    /// </summary>
    public void RemoveParameter(string id)
    {
        BackingNode.Parent.RemoveParameter(id);
    }

    /// <inheritdoc cref="IAction.TryGetParameter(string, out IParameter)"/>
    public bool TryGetParameter(string parameterName, [NotNullWhen(true)] out MutableParameter? parameter)
    {
        foreach (var parameters in Parameters)
        {
            if (parameters.Id == parameterName)
            {
                parameter = parameters;
                return true;
            }
        }

        parameter = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetParameterValue<TEnum>(string parameterName, out TEnum value) where TEnum : struct, Enum
    {
        if (TryGetParameter(parameterName, out var parameter))
            return parameter.TryGetValue(out value);

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetParameterValue(string parameterName, out bool value)
    {
        if (TryGetParameter(parameterName, out var parameter))
            return parameter.TryGetValue(out value);

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetParameterValue(string parameterName, out double value)
    {
        if (TryGetParameter(parameterName, out var parameter))
            return parameter.TryGetValue(out value);

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetParameterValue(string parameterName, out int value)
    {
        if (TryGetParameter(parameterName, out var parameter))
            return parameter.TryGetValue(out value);

        value = default;
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetParameterValue(string parameterName, out string value)
    {
        if (TryGetParameter(parameterName, out var parameter))
            return parameter.TryGetValue(out value);

        value = "";
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetParameterValue(string parameterName, out IList<double> value)
    {
        if (TryGetParameter(parameterName, out var parameter))
            return parameter.TryGetValue(out value);

        value = [];
        return false;
    }

    /// <inheritdoc/>
    public bool TryGetParameterValue(string parameterName, out IList<string> value)
    {
        if (TryGetParameter(parameterName, out var parameter))
            return parameter.TryGetValue(out value);

        value = [];
        return false;
    }

    bool IAction.TryGetParameter(string parameterName, [NotNullWhen(true)] out IParameter? parameter)
    {
        var result = TryGetParameter(parameterName, out var mutableParameter);
        parameter = mutableParameter;
        return result;
    }

    /// <summary>
    /// Allows to retrieve or set any parameter on this <see cref="IAction"/>.
    /// </summary>
    /// <returns>
    /// The parameter if found; otherwise <see langword="null"/>.
    /// </returns>
    public MutableParameter? this[string parameterName]
    {
        get
        {
            TryGetParameter(parameterName, out var parameter);
            return parameter;
        }
        set
        {
            BackingNode.Parent[BackingNode.Id, parameterName] = value?.Value;
        }
    }

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IParameter? IAction.this[string parameterName] => this[parameterName];
}
