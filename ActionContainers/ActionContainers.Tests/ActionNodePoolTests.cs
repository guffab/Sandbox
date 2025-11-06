using Newtonsoft.Json;

namespace ActionContainers.Tests;

public class ActionNodePoolTests
{
    readonly ActionNodePool pool = ActionNodePool.Instance;

    [SetUp]
    public void Setup()
    {
        pool.Reset();

        //an action with two types and a single parameter
        var pu = pool.Add(new ActionNode("PG_Production Unit"));
        pu.AddType("Filigree Slab");
        pu.AddType("Double Wall");
        pu.AddParameter("PF_Layer 1");

        //an action with a single type and a parameter with a special unit
        var layer = pool.Add(new ActionNode("PG_Layer"));
        layer.AddType("Flipped Layer");
        layer.AddParameter("PF_Flip 1", Unit.Bool);
    }

    [Test]
    public void ActionNode_DeserializeWithMissingParamters_Throws()
    {
        //Arrange
        var serialized = """
        {
            "I": "action",
            "P": [
                {
                    "I": "A",
                },
                {
                    "I": "B",
                },
                {
                    "I": "C",
                }
            ],
            "T": [
                {
                    "I": "tpye",
                    "V": [
                        "1",
                        "3",
                    ]
                },
            ]
        }
        """;
 
        //Assert
        Assert.Throws<InvalidDataException>(() => JsonConvert.DeserializeObject<ActionNode>(serialized));
    }

    [Test]
    public void ActionParameter_ChangeId_UpdatesEverywhere()
    {
        //Arrange
        var action1 = new MutableAction(pool["PG_Production Unit", "Filigree Slab"]!, null);
        var action2 = new MutableAction(pool["PG_Production Unit", "Double Wall"]!, null);

        //Act
        var v1 = action1["PF_Layer 1"]!;
        v1.Id = "Layer 1";

        var v2 = action2["Layer 1"]!;
        v2.Id = "Ly 1";

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(v1.Id, Is.EqualTo(v2.Id));
            Assert.That(v2.Id, Is.EqualTo("Ly 1"));
        });
    }

    [Test]
    public void ActionNode_ChangeId_ReferencesDontBreak()
    {
        //Arrange
        var filigreeSlab = new MutableAction(pool["PG_Production Unit", "Filigree Slab"]!, null);
        var layer1Param = filigreeSlab["PF_Layer 1"]!;

        layer1Param.Value = "PG_Layer_@_Flipped Layer";
        var flippedLayer = layer1Param.SubAction!;

        //Act
        flippedLayer.ActionName = "Layer";

        //Assert
        Assert.That(layer1Param.SubAction, Is.Not.EqualTo(null));
        Assert.That(layer1Param.SubAction.ActionName, Is.EqualTo("Layer"));
    }

    [Test]
    public void ActionTypeNode_ChangeId_ReferencesDontBreak()
    {
        //Arrange
        var filigreeSlab = new MutableAction(pool["PG_Production Unit", "Filigree Slab"]!, null);
        var layer1Param = filigreeSlab["PF_Layer 1"]!;

        layer1Param.Value = "PG_Layer_@_Flipped Layer";
        var flippedLayer = layer1Param.SubAction!;

        //Act
        flippedLayer.TypeName = "Regular Layer";

        //Assert
        Assert.That(layer1Param.SubAction, Is.Not.EqualTo(null));
        Assert.That(layer1Param.SubAction.TypeName, Is.EqualTo("Regular Layer"));
    }

    [Test]
    public void MutableAction_AddParameter_ParameterExists()
    {
        //Arrange
        var filigreeSlab = new MutableAction(pool["PG_Production Unit", "Filigree Slab"]!, null);
        var before = filigreeSlab["New Parameter"];

        //Act
        filigreeSlab.AddParameter("New Parameter", Unit.Length, "something");
        var after = filigreeSlab["New Parameter"];

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(before, Is.EqualTo(null));
            Assert.That(after, Is.Not.EqualTo(null));
        });
        Assert.Multiple(() =>
        {
            Assert.That(after.Unit, Is.EqualTo(Unit.Length));
            Assert.That(after.Value, Is.EqualTo("something"));
        });
    }

    [Test]
    public void MutableAction_AddExistingParameter_Throws()
    {
        //Arrange
        var filigreeSlab = new MutableAction(pool["PG_Production Unit", "Filigree Slab"]!, null);
        filigreeSlab.AddParameter("Add Me Twice");

        //Assert
        Assert.Throws<InvalidOperationException>(() => filigreeSlab.AddParameter("Add Me Twice"));
    }

    [Test]
    public void MutableAction_RemoveParameter_IsRemoved()
    {
        //Arrange
        var filigreeSlab = new MutableAction(pool["PG_Production Unit", "Filigree Slab"]!, null);
        
        filigreeSlab.AddParameter("A");
        filigreeSlab.AddParameter("B", true);
        filigreeSlab.AddParameter("C", "Value");
        filigreeSlab.AddParameter("D", Unit.Weight, 68);

        var countBefore = filigreeSlab.Parameters.Count;

        //Act
        filigreeSlab.RemoveParameter("C");
        var deletedParameter = filigreeSlab["C"];
        var countNow = filigreeSlab.Parameters.Count;

        //Assert
        Assert.Multiple(() =>
        {
            Assert.That(deletedParameter, Is.EqualTo(null));
            Assert.That(countNow, Is.EqualTo(countBefore - 1));
        });
    }

    [Test]
    public void Pool_SerializeEditDeserialize_RevertChanges()
    {
        //Arrange
        var serializedJson = pool.Serialize();
        var layer = pool["PG_Layer"]!;
        layer.Id = "Something else";

        layer = pool["PG_Layer"]!;
        var somethingElse = pool["Something else"];

        //Act
        var deserialized = JsonConvert.DeserializeObject<List<ActionNode>>(serializedJson);
        deserialized ??= [];
        pool.Initialize(deserialized);

        //Assert
        Assert.Multiple(() =>
        {
            //Assert
            Assert.That(layer, Is.EqualTo(null));
            Assert.That(somethingElse, Is.Not.EqualTo(null));
            Assert.That(pool["PG_Layer"], Is.Not.EqualTo(null));
        });
    }

    [Test]
    public void MutableParameter_InsertSubAction_Works()
    {
        //Arrange
        var pu = pool["PG_Production Unit"]!;
        pu.AddParameter("Test");

        var pu1 = new MutableAction(pu.AddType("Type 1"), null);
        var pu1Param = pu1["Test"]!;

        var pu2 = new MutableAction(pu.AddType("Type 2"), null);
        var pu2Param = pu2["Test"]!;

        var layer = pool["PG_Layer"]!;
        var ly1 = new MutableAction(layer.AddType("Type 1"), null);
        var ly2 = new MutableAction(layer.AddType("Type 2"), null);

        //Act
        pu1Param.SubAction = ly1;
        pu2Param.SubAction = ly2;

        Assert.Multiple(() =>
        {
            //Assert
            Assert.That(pu1Param.SubAction.Id, Is.EqualTo(ly1.Id));
            Assert.That(pu2Param.SubAction.Id, Is.EqualTo(ly2.Id));
        });
    }

    [Test]
    public void MutableAction_InsertParamters_Works()
    {
        //Arrange
        var pu1 = new MutableAction(pool["PG_Production Unit", "Filigree Slab"]!, null);
        var before1 = pu1["PU Test 1"]?.Id;
        var before2 = pu1["PU Test 2"]?.Id;

        var layer = pool["PG_Layer"]!;
        layer.AddParameter("Test 1");
        layer.AddParameter("Test 2");

        var ly = new MutableAction(layer.AddType("Type"), null);
        
        //Act
        pu1["PU Test 1"] = ly["Test 1"];
        pu1["PU Test 2"] = ly["Test 2"];

        var after1 = pu1["PU Test 1"]?.Id;
        var after2 = pu1["PU Test 2"]?.Id;

        //Assert
        Assert.Multiple(() =>
        {
            //Assert
            Assert.That(before1, Is.EqualTo(null));
            Assert.That(before2, Is.EqualTo(null));
            Assert.That(after1, Is.Not.EqualTo(null));
            Assert.That(after2, Is.Not.EqualTo(null));
        });
    }
}