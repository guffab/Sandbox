namespace SyntaxScanner.Tests;

public class SplitTests
{
    const char separator = ',';
    private SyntaxPair[] syntax;

    [SetUp]
    public void Setup()
    {
        var quotesSynxtax = new SyntaxPair('\"', '\"', int.MaxValue);
        var parenthesesSyntax = new SyntaxPair('(', ')', 0);

        syntax = [quotesSynxtax, parenthesesSyntax];
    }

    [TestCase("")]
    [TestCase(",")]
    [TestCase("a,b,cd")]
    public void Split_NoSyntax_PerformRegularSplit(string input)
    {
        RunSplitTest(input, input.Split(','));
    }

    [TestCase("Path(a,b), 100, 500", "Path(a,b)", " 100", " 500")]
    [TestCase("if (,,), Path(,), 2.5", "if (,,)", " Path(,)", " 2.5")]
    public void Split_SyntaxIsFinished_ReturnsExpectedSpans(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("Formula((),),3", "Formula((),)", "3")]
    public void Split_SyntaxInterferes_ReturnsExpectedSpans(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("Path(\"),a, b\")", "Path(\"),a, b\")")]
    [TestCase("Path(\"),a, b\"), c,d", "Path(\"),a, b\")", " c", "d")]
    public void Split_SyntaxInsideString_ReturnsExpectedSpans(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("if (a, b , c", "if (a, b , c")]
    [TestCase("\"a, b , c", "\"a, b , c")]
    public void Split_IncompleteSyntax_ReturnsInput(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("\"2,5\"", "\"2,5\"")]
    [TestCase(" Path(,) ", " Path(,) ")]
    [TestCase("if(a, b, c) ", "if(a, b, c) ")]
    public void Split_EnclosingSyntax_ReturnsInput(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    private void RunSplitTest(string input, IList<string> expectedResult)
    {
        // Act
        var result = new List<string>();

        foreach (var subSpan in input.Split(separator, syntax, stackalloc SyntaxPair[64]))
            result.Add(subSpan.ToString());

        // Assert
        Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
    }
}
