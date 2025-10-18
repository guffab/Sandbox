using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ActionContainers;

/// <summary>
/// Represents an action, meaning it encapsulates implementing types and their shared parameters.
/// </summary>
/// <remarks>
/// This object is entirely mutable (sometimes through specialized methods only) so that any changes are immediately visible to all objects referencing it.
/// </remarks>
public class ActionNode(string id, IReadOnlyList<ParameterTemplateNode> parameters, IReadOnlyList<ActionTypeNode> types)
{
    [JsonProperty("I")] public string Id { get; set; } = id;

    [JsonProperty("P")] public IReadOnlyList<ParameterTemplateNode> Parameters { get; private set; } = parameters;

    [JsonProperty("T")] public IReadOnlyList<ActionTypeNode> Types { get; private set; } = types;

    [NotNull]
    public string? this[string type, string parameter]
    {
        get
        {
            if (!TryGetType(type, out var typeNode))
                throw new Exception("type does not exist");

            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Id == parameter)
                    return typeNode.ParameterValues[i];
            }

            throw new Exception("parameter does not exist");
        }
        set
        {
            if (value is null)
            {
                RemoveParameter(parameter);
                return;
            }

            if (!TryGetType(type, out var typeNode))
                throw new Exception("type does not exist");

            SET_PARAMETER:
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Id == parameter)
                {
                    typeNode.ParameterValues = [.. typeNode.ParameterValues.Take(i), value, .. typeNode.ParameterValues.Skip(i + 1)];
                    return;
                }
            }

            //not found, create it then
            AddParameter(parameter);
            goto SET_PARAMETER;
        }
    }

    public void AddParameter(string id, Unit unit = Unit.TextOrAction)
    {
        Parameters = [.. Parameters, new ParameterTemplateNode(id, unit)];

        foreach (var type in Types)
            type.ParameterValues = [.. type.ParameterValues, ""];
    }

    public bool TryGetType(string id, [NotNullWhen(true)] out ActionTypeNode? typeNode)
    {
        foreach (var type in Types)
        {
            if (type.Id == id)
            {
                typeNode = type;
                return true;
            }
        }

        typeNode = null;
        return false;
    }

    public bool TryGetParameter(string id, [NotNullWhen(true)] out ParameterTemplateNode? templateNode)
    {
        foreach (var parameter in Parameters)
        {
            if (parameter.Id == id)
            {
                templateNode = parameter;
                return true;
            }
        }

        templateNode = null;
        return false;
    }

    public void RemoveParameter(string parameterName)
    {
        var match = Parameters.FirstOrDefault(x => x.Id == parameterName);
        if (match is null)
            return;

        var parameters = Parameters.ToList();
        int i = parameters.IndexOf(match);

        //remove template
        parameters.RemoveAt(i);
        Parameters = [.. parameters];

        //remove implementations
        foreach (var type in Types)
            type.ParameterValues = [.. type.ParameterValues.Take(i), .. type.ParameterValues.Skip(i + 1)];
    }
}

/// <summary>
/// Represents a single parameter template of an <see cref="ActionNode"/>, meaning it does not provide a value.
/// </summary>
public class ParameterTemplateNode(string id, Unit unit = Unit.TextOrAction)
{
    internal ActionNode Parent;

    [JsonProperty("I")] public string Id { get; set; } = id;

    //having a reliable logic for automatic conversions between incompatible units is damn near impossible. Just delete the parameter and recreate it if you must!
    [JsonProperty("U")] public Unit Unit { get; } = unit;
}

/// <summary>
/// Represents a type of an action, meaning it provides a value for every parameter.
/// </summary>
/// <remarks>
/// This object is entirely mutable (sometimes through specialized methods only)  so that any changes are immediately visible to all objects referencing it+.
/// </remarks>
public class ActionTypeNode(string id, IReadOnlyList<string> parameterValues)
{
    internal ActionNode Parent;

#warning renaming the Id should probably update all values
    [JsonProperty("I")] public string Id { get; set; } = id;

    [JsonProperty("V")] public IReadOnlyList<string> ParameterValues { get; set; } = parameterValues;
}

/// <summary>
/// Defines the unit of a value. This can be used to display the value in a convenient way.
/// </summary>
[JsonConverter(typeof(StringEnumConverter))]
public enum Unit
{
    /// <summary>
    /// Dimension-less numerical value that can be used to represent counts, ratios, or similar.
    /// </summary>
    Number = -1,

    /// <summary>
    /// Either holds some arbitrary text or links to another action by specifying its ID.
    /// </summary>
    TextOrAction = 0,

    Bool = 1,

    Length = 2,

    Weight = 3,
}
