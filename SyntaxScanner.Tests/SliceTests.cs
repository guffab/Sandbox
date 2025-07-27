namespace SyntaxScanner.Tests;

public class SliceTests
{
    const char openingParentheses = '(';
    const char closingParentheses = ')';
    private SyntaxPair[] syntaxSubset;

    [SetUp]
    public void Setup()
    {
        var quotesSynxtax = new SyntaxPair('\"', '\"', int.MaxValue);
        syntaxSubset = [quotesSynxtax];
    }

    [TestCase("(a)", "a")]
    [TestCase(" (if(,,)) ", "if(,,)")]
    public void SliceInBetween_ValidAmountOfBrackets_ReturnsInner(string input, string expectedResult)
    {
        RunSliceTest(input, expectedResult);
    }

    [TestCase("'a'", "a")]
    [TestCase(" 'if(,,)' ", "if(,,)")]
    public void SliceInBetween_ValidAmountOfQuotes_ReturnsInner(string input, string expectedResult)
    {
        RunSliceTest(input, expectedResult, '\'', '\'');
    }

    [TestCase("(a", "")]
    [TestCase("a", "")]
    [TestCase(" (if(,,) ", "")]
    [TestCase(" if,, ", "")]
    public void SliceInBetween_MissingBrackets_ReturnsEmpty(string input, string expectedResult)
    {
        RunSliceTest(input, expectedResult);
    }

    [TestCase("(\"a\")", "\"a\"")]
    [TestCase(" (\"if(,,)\") ", "\"if(,,)\"")]
    public void SliceInBetween_BlockingSyntaxCloses_ReturnsInner(string input, string expectedResult)
    {
        RunSliceTest(input, expectedResult);
    }

    [TestCase("(\"a)", "")]
    [TestCase("\"(a)", "")]
    [TestCase(" (\"if(,,)) ", "")]
    public void SliceInBetween_BlockingSyntaxStays_ReturnsEmpty(string input, string expectedResult)
    {
        RunSliceTest(input, expectedResult);
    }

    [TestCase("(\"a)", "(\"a)")]
    [TestCase(" (\"if(,,)) ", " (\"if(,,)) ")]
    public void SliceInBetweenRemainder_BlockingSyntaxStays_ReturnsInput(string input, string expectedRemainder)
    {
        RunSliceRemainderTest(input, expectedRemainder);
    }

    [TestCase("", "")]
    [TestCase("if(,,)", "")]
    [TestCase("if(,,)))", "))")]
    [TestCase("if(,,)  ", "  ")]
    [TestCase("if(,,) else()", " else()")]
    public void SliceInBetweenRemainder_ValidAmountOfBrackets_ReturnsTrailingRest(string input, string expectedRemainder)
    {
        RunSliceRemainderTest(input, expectedRemainder);
    }

    private void RunSliceTest(string input, string expectedResult)
    {
        // Act
        var result = SyntaxView.SliceInBetween(input, syntaxSubset, openingParentheses, closingParentheses, out _).ToString();

        // Assert
        Assert.That(result == expectedResult, $"Expected '{expectedResult}' but got '{result}' for slicing '{input}'");
    }

    private void RunSliceTest(string input, string expectedResult, char start, char end)
    {
        // Act
        var result = SyntaxView.SliceInBetween(input, [], start, end, out _).ToString();

        // Assert
        Assert.That(result == expectedResult, $"Expected '{expectedResult}' but got '{result}' for slicing '{input}'");
    }

    private void RunSliceRemainderTest(string input, string expectedRemainder)
    {
        // Act
        _ = SyntaxView.SliceInBetween(input, syntaxSubset, openingParentheses, closingParentheses, out var spanRemainder);
        string remainder = spanRemainder.ToString();

        // Assert
        Assert.That(remainder == expectedRemainder, $"Expected '{expectedRemainder}' but got '{remainder}' as remainder for slicing '{input}'");
    }
}
