namespace SyntaxScanner.Tests;

internal class TokenSplitTests
{
    private SyntaxPair[] syntax;
    private string[] supportedTokens;

    [SetUp]
    public void Setup()
    {
        syntax = [new SyntaxPair('(', ')', 0)];
        supportedTokens = ["==", "!=", "<", "<=", ">", ">=", "+", "-", "*", "/"];
    }

    [Test]
    public void TokenEnumerator_EmptyInput_ReturnsFalse()
    {
        RunSplitTest("", Array.Empty<string>());
        RunSplitTest("", Array.Empty<bool>());
    }

    [Test]
    public void TokenEnumerator_UnsupportedTokenLength_Throws()
    {
        Assert.Throws<ArgumentException>(() => SyntaxView.SplitTokenized("", syntax, ["??="]));
    }

    [TestCase(" ")]
    [TestCase("()")]
    [TestCase(" ( ) ")]
    public void TokenEnumerator_NoTokens_ReturnsInput(string input)
    {
        RunSplitTest(input, [input]);
        RunSplitTest(input, [false]);
    }

    [TestCase("a + b", "a ", "+", " b")]
    [TestCase(" a+b", " a", "+", "b")]
    [TestCase("-2", "-", "2")]
    [TestCase("c++", "c", "+", "+")]
    public void TokenEnumerator_WithTokens_SplitCorrectly(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("a + b", false, true, false)]
    [TestCase(" a+b", false, true, false)]
    [TestCase("-2", true, false)]
    [TestCase("c++", false, true, true)]
    public void TokenEnumerator_WithTokens_ReturnCorrectContext(string input, params bool[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("a < b", "a ", "<", " b")]
    [TestCase("a <= b", "a ", "<=", " b")]
    public void TokenEnumerator_TwoCharTokens_IdentifyCorrectly(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("a = b", "a ", "=", " b")] //note how '=' is not a token, but '==' is
    public void TokenEnumerator_IncompleteTokenMatch_ReturnAnyway(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("(a + b)", "(a + b)")]
    public void TokenEnumerator_BlockingSyntax_NoSplit(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    private void RunSplitTest(string input, IList<bool> expectedResult)
    {
        // Act
        var result = new List<bool>();

        foreach (var (start, end, isToken) in input.SplitTokenized(supportedTokens, syntax, stackalloc SyntaxPair[64]))
            result.Add(isToken);

        // Assert
        Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
    }

    private void RunSplitTest(string input, IList<string> expectedResult)
    {
        // Act
        var result = new List<string>();

        foreach (var (start, end, isToken) in input.SplitTokenized(supportedTokens, syntax, stackalloc SyntaxPair[64]))
            result.Add(input.Substring(start, end - start));

        // Assert
        Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
    }
}
