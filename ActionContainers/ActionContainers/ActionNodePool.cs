using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace ActionContainers;

public class ActionNodePool
{
    List<ActionNode> _nodes = [];

    public static ActionNodePool Instance { get; } = new();

    public void Add(ActionNode node)
    {
        //there may never be duplicated actions (wether intentional or not!)
        if (_nodes.Any(x => x.Id == node.Id))
            throw new InvalidOperationException();

        _nodes.Add(node);
    }

    public void Remove(string id)
    {
        _nodes.RemoveAll(x => x.Id == id);
    }

    public void Reset(List<ActionNode> nodes)
    {
        _nodes = nodes ?? [];
    }

    public bool TryGetNode(string id, [NotNullWhen(true)] out ActionNode? actionNode)
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

    public List<ActionNode> GetNodes(string[] ids)
    {
        return _nodes.Where(x => ids.Contains(x.Id)).ToList();
    }

    public ActionNode? this[string id]
    {
        get
        {
            TryGetNode(id, out var actionNode);
            return actionNode;
        }
    }

    public string Serialize()
    {
        var settings = new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore };
        return JsonConvert.SerializeObject(_nodes, settings);
    }
}
