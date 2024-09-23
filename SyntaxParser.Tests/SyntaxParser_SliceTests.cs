namespace SyntaxParser.Tests
{
    public class SyntaxParser_SliceTests
    {
        private const char openingParentheses = '(';
        private const char closingParentheses = ')';
        SyntaxPair[] fullSyntax;
        SyntaxPair[] stringSyntax;

        [SetUp]
        public void Setup()
        {
            fullSyntax = [new SyntaxPair('\"', '\"', int.MaxValue), new SyntaxPair('(', ')', 0)];
            stringSyntax = [new SyntaxPair('\"', '\"', int.MaxValue)];
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

        private void RunSliceTest(string input, string expectedResult, char start = openingParentheses, char end = closingParentheses)
        {
            // Act
            var result = SyntaxParser.SliceInBetween(input, stringSyntax, start, end, out var remainder).ToString();

            // Assert
            Assert.That(result == expectedResult, $"Expected '{expectedResult}' but got '{result}' for slicing '{input}'");
        }

        private void RunSliceRemainderTest(string input, string expectedRemainder)
        {
            // Act
            _ = SyntaxParser.SliceInBetween(input, stringSyntax, openingParentheses, closingParentheses, out var spanRemainder);
            string remainder = spanRemainder.ToString();

            // Assert
            Assert.That(remainder == expectedRemainder, $"Expected '{expectedRemainder}' but got '{remainder}' as remainder for slicing '{input}'");
        }
    }
}