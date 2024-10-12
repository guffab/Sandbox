namespace SyntaxScanner.Tests
{
    public class SyntaxView_SliceTests
    {
        const char openingParentheses = '(';
        const char closingParentheses = ')';
        const char separator = ',';
        private SyntaxView syntaxParser;

        [SetUp]
        public void Setup()
        {
            SyntaxPair[] syntax = [new SyntaxPair('\"', '\"', int.MaxValue), new SyntaxPair('(', ')', 0)];
            syntaxParser = new SyntaxView(syntax, separator, openingParentheses, closingParentheses);
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

        private void RunSplitTest(string input, IList<string> expectedResult)
        {
            // Act
            var result = new List<string>();

            foreach (var subSpan in syntaxParser.Split(input, stackalloc SyntaxPair[64]))
                result.Add(subSpan.ToString());

            // Assert
            Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
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
            var result = SyntaxView.SliceInBetween(input, [], start, end, out _).ToString();

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