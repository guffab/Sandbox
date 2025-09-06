namespace SyntaxScanner.Tests;

internal class TokenPairSplitTests
{
    private SyntaxPair[] syntax;
    private SyntaxPair tokenPair;

    [SetUp]
    public void Setup()
    {
        syntax = [new SyntaxPair('(', ')', 0)];
        tokenPair = new SyntaxPair('\'', '\'', 0);
    }

    [Test]
    public void TokenPairEnumerator_EmptyInput_ReturnsFalse()
    {
        RunSplitTest("", Array.Empty<string>());
        RunSplitTest("", Array.Empty<bool>());
    }

    [TestCase(" ")]
    [TestCase("()")]
    [TestCase(" ( ) ")]
    public void TokenPairEnumerator_NoTokens_ReturnsInput(string input)
    {
        RunSplitTest(input, [input]);
        RunSplitTest(input, [false]);
    }

    [TestCase("'a'", "a")]
    [TestCase("a 'b'", "a ", "b")]
    [TestCase("'a' b", "a", " b")]
    public void TokenPairEnumerator_WithTokens_SplitCorrectly(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("'a'", true)]
    [TestCase("a 'b'", false, true)] //other way around actually, just for testing the test
    [TestCase("'a' b", true, false)]
    public void TokenPairEnumerator_WithTokens_ReturnCorrectContext(string input, params bool[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    [TestCase("a ' b", "a ", "' b")]
    public void TokenPairEnumerator_IncompleteTokenMatch_ReturnsAsText(string input, params string[] expectedResult)
    {
        RunSplitTest(input, expectedResult);
    }

    private void RunSplitTest(string input, IList<bool> expectedResult)
    {
        // Act
        var result = new List<bool>();

        foreach (var (start, end, isToken) in SyntaxView.SplitByTokenPair(input, syntax, tokenPair, stackalloc SyntaxPair[64]))
            result.Add(isToken);

        // Assert
        Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
    }

    private void RunSplitTest(string input, IList<string> expectedResult)
    {
        // Act
        var result = new List<string>();

        foreach (var (start, end, isToken) in SyntaxView.SplitByTokenPair(input, syntax, tokenPair, stackalloc SyntaxPair[64]))
            result.Add(input.Substring(start, end - start));

        // Assert
        Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
    }
}
