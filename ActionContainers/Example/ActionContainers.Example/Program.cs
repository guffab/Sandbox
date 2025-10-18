// See https://aka.ms/new-console-template for more information
using ActionContainers;

Console.WriteLine("Hello, World!");


var example1 = /*lang=jsonc,strict*/ """
    [
        {
            "I": "Production Unit",
            "P": [
                {
                    "I": "Layer 1"
                },
                {
                    "I": "Layer 2"
                },
                {
                    "I": "Flip Layer 1",
                    "U": "bool"
                }
            ],
            "T": [
                {
                    "I": "Filigree Slab",
                    "P": [
                        "Layer_@_Filigree",
                        "",
                        "0"
                    ]
                },
                {
                    "I": "Double Wall",
                    //Represents the value of each parameter connected by its index => saves a lot of space (no key duplication), although each parameter needs a value assigned even if it's default!
                    //Rename: O(1) => template/types not involved
                    //Create: O(1) * t*O(1) = O(1) => instant by appending, all types have to append as well
                    //Read/Update: O(1)
                    //Delete: O(n)
                    "P": [
                        "",
                        "Layer_@_Wall 2",
                        "0"
                    ]
                }
            ]
        }
    ]
    """;

Console.WriteLine(example1);