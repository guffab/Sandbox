using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxScanner.Tests
{
    internal class TokenParserTests
    {

        [SetUp]
        public void Setup()
        {
        
        }

        [Test]
        public void TokenParser_SingleOperation_ParseFine()
        {
            var expected = new Operation(new Literal("123"), OperatorKind.Add, new Literal("45"));
            _ = TokenParser.IsOperation("123 + 45", out Operation? result);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TokenParser_DifferentTokens_RespectPriorities()
        {
            var test1 = "1 + 2 * 3";
            var test2 = "1 * 2 + 3";
            
            var expected1 = new Operation(new Literal("1"), OperatorKind.Add, new Operation(new Literal("2"), OperatorKind.Multiply, new Literal("3")));
            var expected2 = new Operation(new Operation(new Literal("1"), OperatorKind.Multiply, new Literal("2")), OperatorKind.Add, new Literal("3"));
            
            _ = TokenParser.IsOperation(test1, out var result1);
            _ = TokenParser.IsOperation(test2, out var result2);
            
            Assert.That(result1, Is.EqualTo(expected1));
            Assert.That(result2, Is.EqualTo(expected2));
        }
        
        [Test]
        public void TokenParser_MultipleOperations_ParseWithCorrectPriority()
        {
            var a = new Operation(new Literal("47"), OperatorKind.Multiply, new Literal("7"));
            var b = new Operation(new Literal("1"), OperatorKind.Divide, new Literal("3"));
            var c = new Operation(new Literal("123"), OperatorKind.Add, a);
            var d = new Operation(c, OperatorKind.Subtract, b);

            var expected = new Operation(d, OperatorKind.Equals, new Literal("12"));
            _ = TokenParser.IsOperation("123 + 47 * 7  -1 / 3 == 12", out Operation? result);

            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void TokenParser_SimilarTokens_IdentifyCorrectOne()
        {
            var a = new Operation(new Literal("12"), OperatorKind.Equals, new Literal("false"));

            var expected = new Operation(new Literal("b"), OperatorKind.Assignment, a);
            _ = TokenParser.IsOperation("b = 12 == false", out Operation? result);

            Assert.That(result, Is.EqualTo(expected));
        }

        public void dd()
        {
            //123 * ( 4 + 5)
        }
    }
}
