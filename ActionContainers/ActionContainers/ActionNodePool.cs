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

    public void Reset(string serializedJson)
    {
        var deserialized = JsonConvert.DeserializeObject<List<ActionNode>>(serializedJson);
        deserialized ??= [];
        Reset(deserialized);
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

    public List<ActionTypeNode> GetTypeNodes(string[] ids)
    {
        var typeNodes = new List<ActionTypeNode>();

        foreach (var id in ids)
        {
            const string delimiter = "_@_";

            var delimiterIndex = id.IndexOf(delimiter);
            if (delimiterIndex is -1)
                continue;

            var actionName = id[..delimiterIndex];
            var typeName = id[(delimiterIndex + delimiter.Length)..];

            var matches = _nodes.Where(x => x.Id == actionName).SelectMany(x => x.Types).Where(x => x.Id == typeName);
            typeNodes.AddRange(matches);
        }

        return typeNodes;
    }

    public ActionNode? this[string actionName]
    {
        get
        {
            TryGetNode(actionName, out var actionNode);
            return actionNode;
        }
    }

    public ActionTypeNode? this[string actionName, string typeName]
    {
        get
        {
            if (TryGetNode(actionName, out var actionNode) && actionNode.TryGetType(typeName, out var typeNode))
                return typeNode;

            return null;
        }
    }

    public string Serialize()
    {
        var settings = new JsonSerializerSettings() { DefaultValueHandling = DefaultValueHandling.Ignore };
        return JsonConvert.SerializeObject(_nodes, settings);
    }
}
