using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxScanner.Tests
{
    internal class TokenParserTests
    {
        private SyntaxPair[] syntax;
        private string[] supportedTokens;

        [SetUp]
        public void Setup()
        {
            syntax = [new SyntaxPair('\"', '\"', int.MaxValue), new SyntaxPair('(', ')', 0)];
            supportedTokens = ["==", "!=", "<", "<=", ">", ">=", "+", "-", "*", "/"];
        }

        [Test]
        public void TokenParser_EmptyInput_ReturnsFalse()
        {
            RunSplitTest("", Array.Empty<string>());
            RunSplitTest("", Array.Empty<bool>());
        }

        [TestCase(" ")]
        [TestCase("()")]
        [TestCase(" ( ) ")]
        public void TokenParser_NoTokens_ReturnsInput(string input)
        {
            RunSplitTest(input, [input]);
            RunSplitTest(input, [false]);
        }

        [TestCase("a + b", "a ", "+", " b")]
        [TestCase(" a+b", " a", "+", "b ")]
        [TestCase("-2", "-", "2")]
        [TestCase("c++", "c", "+", "+")]
        public void TokenParser_WithTokens_SplitCorrectly(string input, params string[] expectedResult)
        {
            RunSplitTest(input, expectedResult);
        }

        [TestCase("a + b", false, true, false)]
        [TestCase(" a+b", false, true, false)]
        [TestCase("-2", true, false)]
        [TestCase("c++", false, true, true)]
        public void TokenParser_WithTokens_ReturnCorrectContext(string input, params bool[] expectedResult)
        {
            RunSplitTest(input, expectedResult);
        }

        private void RunSplitTest(string input, IList<bool> expectedResult)
        {
            // Act
            var result = new List<bool>();

            foreach (var (start, end, isToken) in SyntaxView.SplitByTokens(input, syntax, supportedTokens, stackalloc SyntaxPair[64]))
                result.Add(isToken);

            // Assert
            Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
        }

        private void RunSplitTest(string input, IList<string> expectedResult)
        {
            // Act
            var result = new List<string>();

            foreach (var (start, end, isToken) in SyntaxView.SplitByTokens(input, syntax, supportedTokens, stackalloc SyntaxPair[64]))
                result.Add(input.Substring(start, end - start));

            // Assert
            Assert.That(result.SequenceEqual(expectedResult), $"Expected '{string.Join(";", expectedResult)}' (Count = {expectedResult.Count}) but got '{string.Join(";", result)}' (Count = {result.Count}) for splitting '{input}'");
        }


        // [Test]
        // public void TokenParser_SingleOperation_ParseFine()
        // {
        //     var expected = new Operation(new Literal("123"), OperatorKind.Add, new Literal("45"));
        //     _ = TokenParser.IsOperation("123 + 45", out Operation? result);

        //     Assert.That(result, Is.EqualTo(expected));
        // }

        // [Test]
        // public void TokenParser_DifferentTokens_RespectPriorities()
        // {
        //     var test1 = "1 + 2 * 3";
        //     var test2 = "1 * 2 + 3";

        //     var expected1 = new Operation(new Literal("1"), OperatorKind.Add, new Operation(new Literal("2"), OperatorKind.Multiply, new Literal("3")));
        //     var expected2 = new Operation(new Operation(new Literal("1"), OperatorKind.Multiply, new Literal("2")), OperatorKind.Add, new Literal("3"));

        //     _ = TokenParser.IsOperation(test1, out var result1);
        //     _ = TokenParser.IsOperation(test2, out var result2);

        //     Assert.That(result1, Is.EqualTo(expected1));
        //     Assert.That(result2, Is.EqualTo(expected2));
        // }

        // [Test]
        // public void TokenParser_MultipleOperations_ParseWithCorrectPriority()
        // {
        //     var a = new Operation(new Literal("47"), OperatorKind.Multiply, new Literal("7"));
        //     var b = new Operation(new Literal("1"), OperatorKind.Divide, new Literal("3"));
        //     var c = new Operation(new Literal("123"), OperatorKind.Add, a);
        //     var d = new Operation(c, OperatorKind.Subtract, b);

        //     var expected = new Operation(d, OperatorKind.Equals, new Literal("12"));
        //     _ = TokenParser.IsOperation("123 + 47 * 7  -1 / 3 == 12", out Operation? result);

        //     Assert.That(result, Is.EqualTo(expected));
        // }

        // [Test]
        // public void TokenParser_SimilarTokens_IdentifyCorrectOne()
        // {
        //     var a = new Operation(new Literal("12"), OperatorKind.Equals, new Literal("false"));

        //     var expected = new Operation(new Literal("b"), OperatorKind.Assignment, a);
        //     _ = TokenParser.IsOperation("b = 12 == false", out Operation? result);

        //     Assert.That(result, Is.EqualTo(expected));
        // }

        // public void dd()
        // {
        //     //123 * ( 4 + 5)
        // }
    }
}
