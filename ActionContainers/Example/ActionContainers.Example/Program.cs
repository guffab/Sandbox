using ActionContainers;
using Newtonsoft.Json;

var node = new ActionNode("Production Unit", [new("Layer 1"), new("Layer 2"), new("Flip Layer 1", Unit.Bool)], [new("Filigree Slab", ["Layer_@_Filigree", "", "0"]), new("Double Wall", ["", "Layer_@_Wall 2", "0"])]);
ActionNodePool.Instance.Add(node);

node["Filigree Slab", "Layer 1"] = "Layer_@_Something Else";
if (node.TryGetParameter("Layer 1", out var templateNode))
    templateNode.Id = "PF_Layer 1";

var serialized = ActionNodePool.Instance.Serialize();
Console.WriteLine(serialized);

ActionNodePool.Instance["Production Unit"]!.AddParameter("hate");

var deserialized = JsonConvert.DeserializeObject<List<ActionNode>>(serialized)!;
ActionNodePool.Instance.Reset(deserialized);

var ma = new MutableAction(ActionNodePool.Instance["Production Unit", "Filigree Slab"], null);
ma.ActionName = "Flanders";
ma.TypeName = "Ned";

Console.WriteLine(ma.Id);

var paraL2 = ma["Layer 2"];
paraL2.Id = "EAt dirt";

paraL2.Value = "1e-6";


var example1 = /*lang=json*/ """
[
  {
    "I": "Production Unit",
    "P": [
      {
        "I": "Layer 1",
      },
      {
        "I": "Layer 2",
      },
      {
        "I": "Flip Layer 1",
        "U": "bool"
      }
    ],
    "T": [
      {
        "I": "Filigree Slab",
        "V": [
          "Layer_@_Filigree",
          "",
          "0"
        ]
      },
      {
        "I": "Double Wall",
        
        //Represents the value of each parameter connected by its index => saves a lot of space (no key duplication), although each parameter needs a value assigned even if it's default!
        //Rename: O(1) => no types involved
        //Create: (t + 1)*O(1) = O(1) => instant by appending, all types have to append as well
        //Read/Update: O(1)
        //Delete: O(n)
        "V": [
          "",
          "Layer_@_Wall 2",
          "0"
        ]
      }
    ]
  }
]
""";
