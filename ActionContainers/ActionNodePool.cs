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
}
