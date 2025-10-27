using ActionContainers;

var pool = ActionNodePool.Instance;

var node = pool.Add(new ActionNode("Production Unit"));
node.AddParameter("Layer 1");
node.AddParameter("Layer 2");
node.AddParameter("Flip Layer 1", Unit.Bool);

var fs = node.AddType("Filigree Slab");
fs["Layer 1"] = "Layer_@_Filigree";

var dw = node.AddType("Double Wall");
dw["Layer 2"] = "Layer_@_Wall 2";
dw["Flip Layer 1"] = "1";

node["Filigree Slab", "Layer 1"] = "Layer_@_Something Else";
if (node.TryGetParameter("Layer 1", out var templateNode))
    templateNode.Id = "PF_Layer 1";

_ = /*lang=json*/ """
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
