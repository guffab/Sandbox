using System.Diagnostics.CodeAnalysis;

namespace ActionContainers;

public static class ActionNodePool
{
    static List<ActionNode> _nodes = [];

    public static void Add(ActionNode node)
    {
        _nodes.Add(node);
    }

    public static void RemoveAll(string id)
    {
        _nodes.RemoveAll(x => x.Id == id);
    }

    public static bool TryGetNode(string id, [NotNullWhen(true)] out ActionNode? actionNode)
    {
        foreach (var node in _nodes)
        {
            if (node.Id == id)
            {
                actionNode = node;
                return true;
            }
        }

        actionNode = default;
        return false;
    }

    public static List<ActionNode> GetNodes(string[] ids)
    {
        return _nodes.Where(x => ids.Contains(x.Id)).ToList();
    }

    public static void AddParamter(string actionId, ParameterNode parameter)
    {
#warning needs a way to sync between all same objects with a different type
        if (TryGetNode(actionId, out var action))
        {
            action.Parameters.Add(parameter);            
        }
    }

    public static void RemoveParameter(string id, string parameterName)
    {
        throw new NotImplementedException();
    }

    public static void ReplaceParameter(string actionId, ParameterNode toReplace, ParameterNode replacement)
    {
        throw new NotImplementedException();
    }
}
