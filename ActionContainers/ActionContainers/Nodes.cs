using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ActionContainers;

/// <summary>
/// Represents an action, meaning it encapsulates implementing types and their shared parameters.
/// </summary>
/// <remarks>
/// This object is entirely mutable (sometimes through specialized methods only) so that any changes are immediately visible to all objects referencing it.
/// </remarks>
public class ActionNode
{
    public ActionNode(string id, List<ParameterTemplateNode> parameters, List<ActionTypeNode> types)
    {
        Id = id;
        Parameters = parameters;
        Types = types;

        //the classic chicken egg problem when deserializing: you can't instantiate the children without providing a parent, which needs it children to be constructed.
        //as a compromise, the children can be instantiated without their parent, but this should only be used during deserialization!
        foreach (var p in Parameters)
            p.Parent = this;

        foreach (var t in Types)
            t.Parent = this;
    }

    [JsonProperty("I")] public string Id { get; set; }

    [JsonProperty("P")] public IReadOnlyList<ParameterTemplateNode> Parameters { get; set; }

    [JsonProperty("T")] public IReadOnlyList<ActionTypeNode> Types { get; set; }

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
        ((List<ParameterTemplateNode>)Parameters).Add(new ParameterTemplateNode(id, unit, this));

        foreach (var type in Types)
            type.ParameterValues = [.. type.ParameterValues, ""];
    }

    public void AddType(string id)
    {
        Types = [.. Types, new ActionTypeNode(id, Enumerable.Repeat("", Parameters.Count).ToArray(), this)];
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
public class ParameterTemplateNode(string id, Unit unit = Unit.TextOrAction, ActionNode parent = null!)
{
    internal ActionNode Parent = parent;

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
public class ActionTypeNode(string id, IReadOnlyList<string> parameterValues, ActionNode parent = null!)
{
    internal ActionNode Parent = parent;

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
