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
    public void Pool_SerializeEditDeserialize_RevertChanges()
    {
        //Arrange
        var serializedJson = pool.Serialize();
        var layer = pool["PG_Layer"]!;
        layer.Id = "Something else";

        layer = pool["PG_Layer"]!;
        var somethingElse = pool["Something else"];

        //Act
        pool.Initialize(serializedJson);

        //Assert
        Assert.Multiple(() =>
        {
            //Assert
            Assert.That(layer, Is.EqualTo(null));
            Assert.That(somethingElse, Is.Not.EqualTo(null));
            Assert.That(pool["PG_Layer"], Is.Not.EqualTo(null));
        });
    }
}