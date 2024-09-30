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
            int count = 0;

            foreach (var a in syntaxParser.Split(" (\"if(,,)\") "))
                count++;

            return count;
        }

        [Benchmark]
        public int UnsplittableV2()
        {
            int count = 0;

            foreach (var a in syntaxParser.Split(" if(Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\"), Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") + 24, Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") - 13"))
                count++;

            return count;
        }

        [Benchmark]
        public int Splittable()
        {
            int count = 0;

            foreach (var a in syntaxParser.Split("Path(((()))), 313, 12"))
                count++;

            return count;
        }

        [Benchmark]
        public int SplittableV2()
        {
            int count = 0;

            foreach (var a in syntaxParser.Split("    Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\"), Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") + 24, Path(\"@Host.Paramters[Definition.BuiltInParameter(DB_COLOR_RVT)].AsValueString\") - 13"))
                count++;

            return count;
        }
    }
}
