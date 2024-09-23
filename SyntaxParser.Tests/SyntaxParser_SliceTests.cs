namespace SyntaxParser.Tests
{
    public class SyntaxParser_SliceTests
    {
        const char openingParentheses = '(';
        const char closingParentheses = ')';
        const char separator = ',';
        private SyntaxParser syntaxParser;

        [SetUp]
        public void Setup()
        {
            SyntaxPair[] syntax = [new SyntaxPair('\"', '\"', int.MaxValue), new SyntaxPair('(', ')', 0)];
            syntaxParser = new SyntaxParser(syntax, separator, openingParentheses, closingParentheses);
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

        private void RunSliceTest(string input, string expectedResult)
        {
            // Act
            var result = syntaxParser.SliceInBetween(input, out _).ToString();

            // Assert
            Assert.That(result == expectedResult, $"Expected '{expectedResult}' but got '{result}' for slicing '{input}'");
        }

        private void RunSliceTest(string input, string expectedResult, char start, char end)
        {
            // Act
            var result = SyntaxParser.SliceInBetween(input, [], start, end, out _).ToString();

            // Assert
            Assert.That(result == expectedResult, $"Expected '{expectedResult}' but got '{result}' for slicing '{input}'");
        }

        private void RunSliceRemainderTest(string input, string expectedRemainder)
        {
            // Act
            _ = syntaxParser.SliceInBetween(input, out var spanRemainder);
            string remainder = spanRemainder.ToString();

            // Assert
            Assert.That(remainder == expectedRemainder, $"Expected '{expectedRemainder}' but got '{remainder}' as remainder for slicing '{input}'");
        }
    }
}