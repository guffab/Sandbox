using System.Diagnostics;
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
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
public class ActionNode
{
    string _id;
    readonly List<ParameterTemplateNode> _parameters;
    readonly List<ActionTypeNode> _types;

    [JsonConstructor]
    private ActionNode(string id, IReadOnlyList<ParameterTemplateNode> parameters, IReadOnlyList<ActionTypeNode> types)
    {
        _id = id;
        _parameters = parameters.ToList();
        _types = types.ToList();

        //the classic chicken egg problem when deserializing: you can't instantiate the children without providing a parent, which in turn needs its children to be constructed first.
        //as a compromise, the children can be instantiated without their parent, but this should only be used during deserialization!
        foreach (var p in _parameters)
            p.Parent = this;

        foreach (var t in _types)
        {
            t.Parent = this;

            //if they don't match up, we can hardly tell which parameter is missing
            if (t.ParameterValues.Count != _parameters.Count)
                throw new InvalidDataException();
        }
    }

    public ActionNode(string id) : this(id, [], [])
    {
    }

    [JsonProperty("I")]
    public string Id
    {
        get => _id;
        set
        {
            foreach (var type in Types)
                ActionNodePool.Instance.UpdateAllReferences(_id, type.Id, value, type.Id);

            _id = value;
        }
    }

    [JsonProperty("P")]
    public IReadOnlyList<ParameterTemplateNode> Parameters => _parameters;

    [JsonProperty("T")]
    public IReadOnlyList<ActionTypeNode> Types => _types;

    [NotNull]
    public string? this[string type, string parameter]
    {
        get
        {
            if (!TryGetType(type, out var typeNode))
                throw new InvalidOperationException("type does not exist");

            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Id == parameter)
                    return typeNode.ParameterValues[i];
            }

            throw new InvalidOperationException("parameter does not exist");
        }
        set
        {
            if (value is null)
            {
                RemoveParameter(parameter);
                return;
            }

            if (!TryGetType(type, out var typeNode))
                throw new InvalidOperationException("type does not exist");

            SET_PARAMETER:
            for (int i = 0; i < _parameters.Count; i++)
            {
                if (_parameters[i].Id == parameter)
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
        if (_parameters.Any(x => x.Id == id))
            throw new InvalidOperationException();

        _parameters.Add(new ParameterTemplateNode(id, unit, this));

        foreach (var type in _types)
            type.ParameterValues = [.. type.ParameterValues, ""];
    }

    public ActionTypeNode AddType(string id)
    {
        if (_types.Any(x => x.Id == id))
            throw new InvalidOperationException();

        var typeNode = new ActionTypeNode(id, Enumerable.Repeat("", _parameters.Count).ToList(), this);
        _types.Add(typeNode);

        return typeNode;
    }

    public bool TryGetType(string id, [NotNullWhen(true)] out ActionTypeNode? typeNode)
    {
        foreach (var type in _types)
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
        foreach (var parameter in _parameters)
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

    public void RemoveParameter(string id)
    {
        var match = _parameters.FirstOrDefault(x => x.Id == id);
        if (match is null)
            return;

        //remove template
        int i = _parameters.IndexOf(match);
        _parameters.RemoveAt(i);

        //remove implementations
        foreach (var type in _types)
            type.ParameterValues = [.. type.ParameterValues.Take(i), .. type.ParameterValues.Skip(i + 1)];
    }
}

/// <summary>
/// Represents a single parameter template of an <see cref="ActionNode"/>, meaning it does not provide a value.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
public class ParameterTemplateNode(string id, Unit unit = Unit.TextOrAction, ActionNode parent = null!)
{
    internal ActionNode Parent = parent;

    [JsonProperty("I")]
    public string Id { get; set; } = id;

    //having a reliable logic for automatic conversions between incompatible units is damn near impossible. Just delete the parameter and recreate it if you must!
    [JsonProperty("U")]
    public Unit Unit { get; } = unit;
}

/// <summary>
/// Represents a type of an action, meaning it provides a value for every parameter.
/// </summary>
/// <remarks>
/// This object is entirely mutable (sometimes through specialized methods only)  so that any changes are immediately visible to all objects referencing it+.
/// </remarks>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
public class ActionTypeNode(string id, IReadOnlyList<string> parameterValues, ActionNode parent = null!)
{
    internal ActionNode Parent = parent;
    private string _id = id;

    [JsonProperty("I")]
    public string Id
    {
        get => _id;
        set
        {
            ActionNodePool.Instance.UpdateAllReferences(Parent.Id, _id, Parent.Id, value);
            _id = value;
        }
    }

    [JsonProperty("V")]
    public IReadOnlyList<string> ParameterValues { get; set; } = parameterValues;

    [NotNull]
    public string? this[string parameter]
    {
        get => Parent[Id, parameter];
        set => Parent[Id, parameter] = value;
    }
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
