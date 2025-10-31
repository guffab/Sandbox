using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Allows a simpler presentation in the debugger.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
[ExcludeFromCodeCoverage]
internal class ActionDebugView(IAction action)
{
    public string Id => action.Id;

    public IParameter? ParentParameter { get; init; } = action.ParentParameter;

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public List<IParameter> Z_Parameters => action.Parameters.ToList();
}
