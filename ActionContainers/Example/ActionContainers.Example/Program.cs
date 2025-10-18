// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
using ActionContainers;
using ActionContainers.Example;
using Newtonsoft.Json;

Console.WriteLine("Hello, World!");

List<TemplateAction> a = [new TemplateAction("Production Unit", [new("Layer 1"), new("Layer 2"), new("Flip Layer 1", "bool")],
[new("Filigree Slab", ["Layer_@_Filigree", "", "0"]), new("Double Wall", ["", "Layer_@_Wall 2", "0"])])];
var serialized = JsonConvert.SerializeObject(a, Formatting.Indented);
Console.WriteLine(serialized);

var deserialized = JsonConvert.DeserializeObject<List<TemplateAction>>(serialized);

var example1 = /*lang=json*/ """
[
  {
    "Id": "Production Unit",
    "Parameters": [
      {
        "Id": "Layer 1",
      },
      {
        "Id": "Layer 2",
      },
      {
        "Id": "Flip Layer 1",
        "Unit": "bool"
      }
    ],
    "Types": [
      {
        "Id": "Filigree Slab",
        "ParameterValues": [
          "Layer_@_Filigree",
          "",
          "0"
        ]
      },
      {
        "Id": "Double Wall",
        
        //Represents the value of each parameter connected by its index => saves a lot of space (no key duplication), although each parameter needs a value assigned even if it's default!
        //Rename: O(1) => no types involved
        //Create: (t + 1)*O(1) = O(1) => instant by appending, all types have to append as well
        //Read/Update: O(1)
        //Delete: O(n)
        "ParameterValues": [
          "",
          "Layer_@_Wall 2",
          "0"
        ]
      }
    ]
  }
]
""";
