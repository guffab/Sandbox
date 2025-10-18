using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Represents an <see cref="IAction"/> that can be freely mutated.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
internal class MutableAction(ActionNode actionNode, MutableParameter parent) : IAction
{
    private readonly ActionNode BackingNode = actionNode;

    /// <inheritdoc/>
    public string Id { get => BackingNode.Id; set => BackingNode.Id = value; }

    /// <inheritdoc cref="IAction.Parameters"/>
    public List<MutableParameter> Parameters
    {
        #warning I have to repair this at some point
        get => [];// BackingNode.Parameters.Select(x => new MutableParameter(x, this)).ToList();
        // set => BackingNode.Parameters = value.Select(x => x.BackingNode).ToList();
    }

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
            if (value is null)
                BackingNode.RemoveParameter(parameterName);
            else
            {
                #warning unclear how to do just yet
                // //try replace existing version first
                // for (int i = 0; i < Parameters.Count; i++)
                // {
                //     if (Parameters[i].Id == parameterName)
                //     {
                //         BackingNode.ReplaceParameter(BackingNode.Id, Parameters[i].BackingNode, value.BackingNode);
                //         return;
                //     }
                // }

                // //not found
                // BackingNode.AddParamter(BackingNode.Id, value.BackingNode);
            }
        }
    }

    /// <inheritdoc/>
    IParameter? IAction.this[string parameterName] => this[parameterName];
}
