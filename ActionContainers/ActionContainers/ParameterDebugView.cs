using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

/// <summary>
/// Allows a simpler presentation in the debugger.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
[ExcludeFromCodeCoverage]
internal class ParameterDebugView(IParameter parameter)
{
    public string Id => parameter.Id;

    public Unit Unit => parameter is MutableParameter mp ? mp.Unit : default;

    public string Value => parameter.Value;

    public IAction? ParentAction => parameter.ParentAction;

    public IAction? SubAction => parameter.SubAction;
}
