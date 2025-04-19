
namespace SyntaxScanner
{
    public class TokenParser
    {
        private static string[] Tokens = ["+", "-", "*", "/", "=", "=="];

#if NETFRAMEWORK
        public static bool IsOperation(string rawInput, out Operation? operation) => IsOperation(rawInput.AsSpan(), out operation);
#endif

        public static bool IsOperation(ReadOnlySpan<char> rawInput, out Operation? operation)
        {
            var indices = FindTokens(rawInput, Tokens);
            operation = null;

            if (indices.Count <= 1)// && indices.FirstOrDefault().Type is not ParseType.Token)
                return false;

            //currently no valid usage (except for negative numbers?)
            if (indices.First().Type is ParseType.Token)
                return false;
            
            //simplest case
            if (indices.Count == 3)
            {

                operation = new Operation(new Literal(indices[0].Value), Parse(indices[1].Value), new Literal(indices[2].Value));
            }

            //preparing step
            var parsed = new List<IFormula>(indices.Count);

            //find first highest priority and create it (prince)
            var x = int.MinValue;
            var start = 0;

            for (int i = 0; i < indices.Count; i++)
            {
                var (type, value) = indices[0];
                if (type is not ParseType.Token)
                    continue;
                
                //find top-performer
                var priority = GetPriority(Parse(value));
                if (priority > x)
                    start = i;
            }

            //temporary fix
            if (indices[start - 1].Type is ParseType.Literal && indices[start + 1].Type is ParseType.Literal)
            {
                var prince = new Operation(new Literal(indices[start-1].Value), Parse(indices[start].Value), new Literal(indices[start+1].Value));
            }
            else
                return false;
            
            //enter loop (recursion)
            //bool is Left; //indicates wether the other operand still needs to be parsed
            //find highest priority on left side and compare to highest on right
            //if equal or lower, include (prince) in this one. Else same for other (switched sides).
            // => new (prince) included on other side
            //when direct neighbor is not highest priority -> enter recursion

            return true;
        }

        private static OperatorKind Parse(string value)
        {
            return value switch
            {
                "+" => OperatorKind.Add,
                "-" => OperatorKind.Subtract,
                "*" => OperatorKind.Multiply,
                "/" => OperatorKind.Divide,
                "=" => OperatorKind.Assignment,
                "==" => OperatorKind.Equals,
                _ => throw new NotImplementedException(),
            };
        }

        private static int GetPriority(OperatorKind kind)
        {
            return kind switch
            {
                OperatorKind.Assignment => -2,
                OperatorKind.Equals => -1,
                //OperatorKind.LessOrEquals => 0,
                OperatorKind.Add or OperatorKind.Subtract => 1,
                OperatorKind.Multiply or OperatorKind.Divide => 2,
                _ => throw new NotImplementedException(),
            };
        }
        
        private static List<(ParseType Type, string Value)> FindTokens(ReadOnlySpan<char> rawInput, string[] tokenLookupTable)
        {
            var indices = new List<(ParseType Type, string Value)>();
            int lastMatchEnd = 0;

            for (int i = 0; i < rawInput.Length; i++)
            {
                if (FindBestMatch(rawInput, tokenLookupTable, i, out string match))
                {
                    //preceding literals
                    if (lastMatchEnd < i - 1)
                        indices.Add((ParseType.Literal, rawInput.Slice(lastMatchEnd, i - lastMatchEnd).Trim().ToString()));

                    indices.Add((ParseType.Token, match));

                    //adjust indices
                    lastMatchEnd = i + match.Length;
                    i += match.Length - 1;//loop will add 1 by itself
                }
            }

            //include stuff after last token
            var lastToken = indices.LastOrDefault();
            if (lastToken.Type is ParseType.Token && lastMatchEnd < rawInput.Length - 1)
                indices.Add((ParseType.Literal, rawInput.Slice(lastMatchEnd).Trim().ToString()));

            return indices;
        }

        private static bool FindBestMatch(ReadOnlySpan<char> rawInput, string[] tokenLookupTable, int i, out string match)
        {
            foreach (var token in tokenLookupTable)
            {
                if (token[0] == rawInput[i])
                {
                    //find other candidates; if none, return current. else check next char
                    match = token;
                    return true;
                }
            }

            match = "";
            return false;
        }
    }

    internal enum ParseType
    {
        Literal = 0,

        Token = 1,
    }

    public enum OperatorKind
    {
        /// <summary> + </summary>
        Add,

        /// <summary> - </summary>
        Subtract,

        /// <summary> * </summary>
        Multiply,

        /// <summary> / </summary>
        Divide,

        /// <summary> == </summary>
        Equals,

        /// <summary> = </summary>
        Assignment,
    }

    public record Operation(IFormula Left, OperatorKind OperatorKind, IFormula Right) : IFormula;

    public record Literal(string Value) : IFormula;

    public interface IFormula { };

}
