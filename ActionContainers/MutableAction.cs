using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Represents an <see cref="IAction"/> that can be freely mutated.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
internal class MutableAction(ActionNode actionNode, MutableParameter parent) : IAction
{
    /// <inheritdoc/>
    public string Id { get => actionNode.Id; set => actionNode.Id = value; }

    /// <inheritdoc cref="IAction.Parameters"/>
    public List<MutableParameter> Parameters { get; set; }

    /// <inheritdoc cref="IAction.ParentParameter"/>
    public MutableParameter ParentParameter { get; init; } = parent;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IReadOnlyList<IParameter> IAction.Parameters => [.. Parameters];

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    IParameter IAction.ParentParameter => ParentParameter;

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
    bool IAction.TryGetParameter(string parameterName, [NotNullWhen(true)] out IParameter parameter)
    {
        var result = TryGetParameter(parameterName, out var mutableParameter);
        parameter = mutableParameter;
        return result;
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

    /// <summary>
    /// Checks wether the <paramref name="id"/> is referring to this action or one of its parents.
    /// </summary>
    /// <param name="id">The id to look.</param>
    /// <remarks>
    /// This can be used to determine wether it is safe to put a sub-action with this Id below this action.
    /// </remarks>
    public bool IsSelfReferencing(string id)
    {
        if (Id == id)
            return true;

        //this is a hidden null check
        return ParentParameter?.ParentAction is MutableAction parent && parent.IsSelfReferencing(id);
    }

    /// <summary>
    /// Allows to retrieve or set any parameter on this <see cref="IAction"/>.
    /// </summary>
    /// <returns>
    /// The parameter if found; otherwise <see langword="null"/>.
    /// </returns>
    public MutableParameter this[string parameterName]
    {
        get
        {
            TryGetParameter(parameterName, out var parameter);
            return parameter;
        }
        set
        {
            //try replace existing version first
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Id == parameterName)
                {
                    Parameters[i] = value;
                    return;
                }
            }
#warning this needs to be adapted
            //not found
            Parameters.Add(value);
        }
    }

    /// <inheritdoc/>
    IParameter IAction.this[string parameterName] => this[parameterName];
}
