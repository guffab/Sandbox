using System;
using System.Diagnostics;

namespace ActionContainers;

/// <summary>
/// Allows a simpler presentation in the debugger.
/// </summary>
[DebuggerDisplay($"{{{nameof(Id)},nq}}")]
internal class ParameterDebugView(IParameter parameter)
{
    public string Id => parameter.Id;

    public Unit Unit => parameter is MutableParameter mp ? mp.Unit : default;

    public string Value => parameter.Value;

    public IAction? ParentAction => parameter.ParentAction;

    public IAction? SubAction => parameter.SubAction;
}
