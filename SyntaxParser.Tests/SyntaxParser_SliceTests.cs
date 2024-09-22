namespace SyntaxParser.Tests
{
    public class SyntaxParser_SliceTests
    {
        private const char openingParentheses = '(';
        private const char closingParentheses = ')';
        SyntaxParser.SyntaxPair[] fullSyntax;
        SyntaxParser.SyntaxPair[] stringSyntax;

        [SetUp]
        public void Setup()
        {
            fullSyntax = [new SyntaxParser.SyntaxPair('\"', '\"', int.MaxValue), new SyntaxParser.SyntaxPair('(', ')', 0)];
            stringSyntax = [new SyntaxParser.SyntaxPair('\"', '\"', int.MaxValue)];
        }

        [TestCase("(a)", "a")]
        [TestCase(" (if(,,)) ", "if(,,)")]
        public void SliceInBetween_ValidAmountOfBrackets_ReturnsInner(string input, string expectedResult)
        {
            RunSliceTest(input, expectedResult);
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

        private void RunSliceTest(string input, string expectedResult)
        {
            // Act
            var result = SyntaxParser.SliceInBetween(input, stringSyntax, openingParentheses, closingParentheses).ToString();

            // Assert
            Assert.That(result == expectedResult, $"Expected '{expectedResult}' but got '{result}' for slicing '{input}'");
        }
    }
}