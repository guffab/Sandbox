using BenchmarkDotNet.Attributes;

namespace SyntaxParser.Benchmarks
{
    [MemoryDiagnoser]
    public class SyntaxParserPerformance
    {
        const char openingParentheses = '(';
        const char closingParentheses = ')';
        const char separator = ',';
        private SyntaxParser syntaxParser;

        [GlobalSetup]
        public void Setup()
        {
            SyntaxPair[] syntax = [new SyntaxPair('\"', '\"', int.MaxValue), new SyntaxPair('(', ')', 0)];
            syntaxParser = new SyntaxParser(syntax, separator, openingParentheses, closingParentheses);
        }

        [Benchmark]
        public int Unsplittable()
        {
            ReadOnlySpan<char> input = " (\"if(,,)\") ";
            return RunTest(input);
        }

        [Benchmark]
        public int UnsplittableV2()
        {
            ReadOnlySpan<char> input = " if(Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\"), Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") + 24, Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") - 13";
            return RunTest(input);
        }

        [Benchmark]
        public int Splittable()
        {
            ReadOnlySpan<char> input = "Path(((((()))))), 313, 12";
            return RunTest(input);
        }

        [Benchmark]
        public int SplittableV2()
        {
            ReadOnlySpan<char> input = "    Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\"), Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") + 24, Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") - 13";
            return RunTest(input);
        }

        private int RunTest(ReadOnlySpan<char> input)
        {
            int count = 0;

            foreach (var a in syntaxParser.Split(input))
                count += a.Length;

            return count;
        }
    }
}
