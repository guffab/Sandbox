using SyntaxScanner;

namespace Example
{
    /// <summary>
    /// This class serves as a rough guidance for building a custom language parser using <see cref="SyntaxView"/>. <br/>
    /// </summary>
    /// <remarks>
    /// It is currently built to parse an excel-formula-like syntax, but you can simply adapt and mix up the syntax as required by <b>your</b> language definition.
    /// </remarks>
    public class MyLanguageParser
    {
        const char openingParentheses = '(';
        const char closingParentheses = ')';
        const char quote = '\"';
        const char separator = ',';

        private SyntaxPair[] fullSyntax;
        private SyntaxPair[] syntaxSubset;
        private string[] supportedTokens = ["+", "-", "*", "/"];
        private SyntaxPair tokenPair = new SyntaxPair('\'', '\'', 0);

        public MyLanguageParser()
        {
            var quotesSynxtax = new SyntaxPair(openingParentheses, closingParentheses, 0); //note how parentheses are here defined as "regular" syntax
            var parenthesesSyntax = new SyntaxPair(quote, quote, int.MaxValue); //while quotes have the highest priority (because they usually serve as string identifiers)

            fullSyntax = [quotesSynxtax, parenthesesSyntax];
            syntaxSubset = [quotesSynxtax];
        }

        //callers may provide a string, but by using spans internally, we can make more efficient computations
        public IParsedLanguage? Parse(string input)
            => Parse(input.AsSpan());

        public IParsedLanguage? Parse(ReadOnlySpan<char> input)
        {
            input = input.Trim(); //allocation-free, simply a different view over the same char array

            if (input.IsEmpty)
                return default;

            //assuming input is enclosed by parentheses 
            if (input[0] is openingParentheses && input[input.Length - 1] is closingParentheses)
            {
                var inner = SyntaxView.SliceInBetween(input, syntaxSubset, openingParentheses, closingParentheses, out var remainder);

                //now we need to define how our language works: is it incorrect to have something after the parentheses or could it even be a requirement?
                if (!remainder.Trim().IsEmpty)
                    throw new NotImplementedException();

                //assuming parentheses are just needed to isolate e.g., mathematical operations
                return Parse(inner);
            }

            //assuming input resembles a string (you might need to deal with escape sequences, double quotes and so on)
            if (input[0] is quote && input[input.Length - 1] is quote)
            {
                return default;
            }

            //split input on every comma separator, unless some other syntax plays into it
            var subParts = new List<IParsedLanguage?>();
            foreach (var subSpan in SyntaxView.Split(input, fullSyntax, separator, stackalloc SyntaxPair[64])) //we provide a small stack-allocated buffer for best performance
            {
                subParts.Add(Parse(subSpan));
            }

            return default;
        }

        private void CopmarisonWithExtensions(ReadOnlySpan<char> input)
        {
            SyntaxView.IndexOf(input, fullSyntax, separator);
            input.IndexOf(separator, fullSyntax);

            SyntaxView.SliceInBetween(input, syntaxSubset, '(', ')', out _);
            input.SliceBetween('(', ')', syntaxSubset);

            foreach (var subSpan in SyntaxView.Split(input, fullSyntax, separator)) ;
            foreach (var subSpan in input.Split(separator, fullSyntax)) ;

            foreach (var (start, end, isToken) in SyntaxView.SplitByTokens(input, fullSyntax, supportedTokens)) ;
            foreach (var (start, end, isToken) in input.SplitTokenized(supportedTokens, fullSyntax)) ;
            
            foreach (var (start, end, isToken) in SyntaxView.SplitByTokenPair(input, fullSyntax, tokenPair)) ;
            foreach (var (start, end, isToken) in input.SplitTokenized(tokenPair, fullSyntax)) ;
        }
    }

    public interface IParsedLanguage;
}
