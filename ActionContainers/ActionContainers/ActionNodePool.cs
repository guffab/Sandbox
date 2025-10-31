using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace ActionContainers;

public class ActionNodePool
{
    List<ActionNode> _nodes = [];

    //this object is a Singleton, so use the static instance property.
    private ActionNodePool()
    {
    }

    public static ActionNodePool Instance { get; } = new();

    /// <returns>The input <paramref name="node"/>.</returns>
    public ActionNode Add(ActionNode node)
    {
        //there may never be duplicated actions
        if (_nodes.Any(x => x.Id == node.Id))
            throw new InvalidOperationException();

        _nodes.Add(node);
        return node;
    }

    public void Remove(string id)
    {
        _nodes.RemoveAll(x => x.Id == id);
    }

    public void Reset()
    {
        _nodes = [];
    }

    public void Initialize(List<ActionNode> nodes)
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

    /// <summary>
    /// When changing the id of an action, this updates all parameters referencing it to use the new id.
    /// </summary>
    internal void UpdateAllReferences(string oldActionName, string oldTypeName, string newActionName, string newTypeName)
    {
        string oldId = $"{oldActionName}_@_{oldTypeName}";
        string newId = $"{newActionName}_@_{newTypeName}";

        foreach (var node in _nodes)
        {
            foreach (var type in node.Types)
            {
                var pv = type.ParameterValues.ToList();
                for (int i = 0; i < pv.Count; i++)
                    pv[i] = pv[i].Replace(oldId, newId);

                type.ParameterValues = pv;
            }
        }
    }
}
