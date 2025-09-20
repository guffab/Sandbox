namespace ActionContainers;

public static class ActionNodePool
{
    static List<ActionNode> _nodes = [];

    public static void AddNode(ActionNode node)
    {
        _nodes.Add(node);
    }

    public static void DeleteNode(string id)
    {
        _nodes.RemoveAll(x => x.Id == id);
    }

    public static bool TryGetNode(string id, out ActionNode? actionNode)
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
