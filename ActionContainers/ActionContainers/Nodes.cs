using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ActionContainers;

public class TemplateAction(string id, IReadOnlyList<TemplateParameter> parameters, IReadOnlyList<TypeAction> types)
{
    [JsonProperty("I")] public string Id { get; set; } = id;

    [JsonProperty("P")] public IReadOnlyList<TemplateParameter> Parameters { get; private set; } = parameters;

    [JsonProperty("T")] public IReadOnlyList<TypeAction> Types { get; private set; } = types;
    
    public string this[string type, string parameter]
    {
        get
        {
            var t = Types.FirstOrDefault(x => x.Id == type) ?? throw new Exception("type does not exist");

            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Id == parameter)
                    return t.ParameterValues[i];
            }

            throw new Exception("parameter does not exist");
        }
        set
        {
            var t = Types.FirstOrDefault(x => x.Id == type) ?? throw new Exception("type does not exist");
            
            for (int i = 0; i < Parameters.Count; i++)
            {
                if (Parameters[i].Id == parameter)
                {
                    t.UpdateValues([.. t.ParameterValues.Take(i), value, .. t.ParameterValues.Skip(i + 1)]);
                    return;
                }
            }

            throw new Exception("parameter does not exist");
        }
    }

    public void AddParameter(string id, Unit unit)
    {
        Parameters = [..Parameters, new TemplateParameter(id, unit)];

        foreach (var type in Types)
            type.UpdateValues([.. type.ParameterValues, ""]);
    }
}

public class TemplateParameter(string id,  Unit unit = Unit.TextOrAction)
{
    [JsonProperty("I")] public string Id { get; set; } = id;

    [JsonProperty("U")] public Unit Unit { get; set; } = unit;
}

public class TypeAction(string id, IReadOnlyList<string> parameterValues)
{
    #warning renaming the Id should probably update all values
    [JsonProperty("I")] public string Id { get; set; } = id;

    [JsonProperty("V")] public IReadOnlyList<string> ParameterValues { get; private set; } = parameterValues;

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
public class ActionNode(string id, List<ParameterNode> parameters)
{
    public string Id { get; set; } = id;

    public List<ParameterNode> Parameters { get; set; } = parameters;
}

/// <summary>
/// Represents a single parameter of an <see cref="ActionNode"/> without a sub-action.
/// </summary>
/// <remarks>
/// This object is entirely mutable so that any changes are immediately visible to all objects referencing it+.
/// </remarsk>
public class ParameterNode(string id, string value = "", Unit unit = Unit.TextOrAction)
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
