using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ActionContainers;

public class ActionNode(string id, IReadOnlyList<ParameterTemplateNode> parameters, IReadOnlyList<ActionTypeNode> types)
{
    [JsonProperty("I")] public string Id { get; set; } = id;

    [JsonProperty("P")] public IReadOnlyList<ParameterTemplateNode> Parameters { get; private set; } = parameters;

    [JsonProperty("T")] public IReadOnlyList<ActionTypeNode> Types { get; private set; } = types;
    
    public string this[string type, string parameter]
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
            if (!TryGetType(type, out var typeNode))
                throw new Exception("type does not exist");
            
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Id == parameter)
                {
                    typeNode.UpdateValues([.. typeNode.ParameterValues.Take(i), value, .. typeNode.ParameterValues.Skip(i + 1)]);
                    return;
                }
            }

            throw new Exception("parameter does not exist");
        }
    }

    public void AddParameter(string id, Unit unit = Unit.TextOrAction)
    {
        Parameters = [.. Parameters, new ParameterTemplateNode(id, unit)];

        foreach (var type in Types)
            type.UpdateValues([.. type.ParameterValues, ""]);
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
            type.UpdateValues([.. type.ParameterValues.Take(i), .. type.ParameterValues.Skip(i + 1)]);
    }
}

public class ParameterTemplateNode(string id, Unit unit = Unit.TextOrAction)
{
    [JsonProperty("I")] public string Id { get; set; } = id;

    //having a reliable logic for automatic conversions between incompatible units is damn near impossible. Just delete the parameter and recreate it if you must!
    [JsonProperty("U")] public Unit Unit { get; } = unit;
}

public class ActionTypeNode(string id, IReadOnlyList<string> parameterValues)
{
    #warning renaming the Id should probably update all values
    [JsonProperty("I")] public string Id { get; set; } = id;

    [JsonProperty("V")] public IReadOnlyList<string> ParameterValues { get; private set; } = parameterValues;

    /// <summary>
    /// DO NOT CALL THIS DIRECTLY!
    /// </summary>
    internal void UpdateValues(IReadOnlyList<string> newValues)
    {
        ParameterValues = newValues;
    }
}

/// <summary>
/// Represents a single action and it's paramets without any sub-actions. zzzzzzzzzz6zh7ugj           
/// </summary>
/// <remarks>
/// This object is entirely mutable so that any changes are immediately visible to all objects referencing it+.
/// </remarsk>
public class old_ActionNode(string id, List<old_ParameterNode> parameters)
{
    public string Id { get; set; } = id;

    public List<old_ParameterNode> Parameters { get; set; } = parameters;
}

/// <summary>
/// Represents a single parameter of an <see cref="old_ActionNode"/> without a sub-action.
/// </summary>
/// <remarks>
/// This object is entirely mutable so that any changes are immediately visible to all objects referencing it+.
/// </remarsk>
public class old_ParameterNode(string id, string value = "", Unit unit = Unit.TextOrAction)
{
    public string Id { get; set; } = id;
    public string Value { get; set; } = value;
    public Unit Unit { get; set; } = unit;
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
