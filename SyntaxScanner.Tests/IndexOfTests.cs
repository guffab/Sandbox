namespace SyntaxScanner.Tests;

public class IndexOfTests
{
    private SyntaxPair[] syntax;

    [SetUp]
    public void Setup()
    {
        var quotesSynxtax = new SyntaxPair('\"', '\"', int.MaxValue);
        var parenthesesSyntax = new SyntaxPair('(', ')', 0);


        PathParser.Run();
        syntax = [quotesSynxtax, parenthesesSyntax];
    }

    [Test]
    public void IndexOf_EmptyInput_ReturnsMinusOne()
    {
        RunIndexOfTest("", 'a', -1);
    }

    [TestCase("a", 'a', 0)]
    [TestCase("angle", 'g', 2)]
    [TestCase("a++;", ';', 3)]
    public void IndexOf_NoSyntax_ReturnsCorrectIndex(string input, char value, int expectedIndex)
    {
        RunIndexOfTest(input, value, expectedIndex);
    }

    [TestCase("a\";\";", ';', 4)]
    public void IndexOf_WithSyntax_ReturnsCorrectIndex(string input, char value, int expectedIndex)
    {
        RunIndexOfTest(input, value, expectedIndex);
    }

    private void RunIndexOfTest(string input, char value, int expectedIndex)
    {
        // Act
        int index = input.IndexOf(value, syntax);
        
        // Assert
        Assert.That(index, Is.EqualTo(expectedIndex), $"Expected index of '{value}' in '{input}' to be {expectedIndex}, but got {index}.");
    }
}