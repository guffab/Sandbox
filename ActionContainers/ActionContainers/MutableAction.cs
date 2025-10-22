using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Represents an <see cref="IAction"/> that can be freely mutated.
/// </summary>
[DebuggerDisplay($"{{{nameof(ActionName)}_@_{nameof(TypeName)},nq}}")]
public class MutableAction(ActionTypeNode actionNode, MutableParameter? parent) : IAction
{
    private readonly ActionTypeNode BackingNode = actionNode;

    public string Id => $"{ActionName}_@_{TypeName}";

    /// <inheritdoc/>
    public string ActionName { get => BackingNode.Parent.Id; set => BackingNode.Parent.Id = value; }

    /// <inheritdoc/>
    public string TypeName { get => BackingNode.Id; set => BackingNode.Id = value; }

#warning unknowns: how to create or delete a parameter
    /// <inheritdoc cref="IAction.Parameters"/>
    public List<MutableParameter> Parameters => BackingNode.Parent.Parameters.Select(x => new MutableParameter(x, this)).ToList();

    /// <inheritdoc cref="IAction.ParentParameter"/>
    public MutableParameter? ParentParameter { get; init; } = parent;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IParameter> IAction.Parameters => [.. Parameters];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IParameter? IAction.ParentParameter => ParentParameter;

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

    /// <inheritdoc/>
    IParameter? IAction.this[string parameterName] => this[parameterName];
}
