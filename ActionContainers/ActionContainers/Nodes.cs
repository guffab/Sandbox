namespace ActionContainers;

/// <summary>
/// Represents a single action and it's paramets without any sub-actions.
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

    Boolean = 1,

    Length = 2,

    Weight = 3,
}
