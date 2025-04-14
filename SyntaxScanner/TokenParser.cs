using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxScanner
{
    public class TokenParser
    {
        private static string[] Tokens = ["+", "-", "*", "/", "=", "=="];

        public static bool IsOperation(string rawInput, out Operation? operation) => IsOperation(rawInput.AsSpan(), out operation);

        public static bool IsOperation(ReadOnlySpan<char> rawInput, out Operation? operation)
        {
            var indices = FindTokens(rawInput, Tokens);
            operation = null;

            if (indices.Count <= 1)// && indices.FirstOrDefault().Type is not ParseType.Token)
                return false;

            //find first highest priority and create it (prince)
            //enter loop (recursion)
            //bool is Left; //indicates wether the other operand still needs to be parsed
            //find highest priority on left side and compare to highest on right
            //if equal or lower, include (prince) in this one. Else same for other (switched sides).
            // => new (prince) included on other side
            //when direct neighbor is not highest priority -> enter recursion
            
            return true;
        }

        private static List<(ParseType Type, Range Range)> FindTokens(ReadOnlySpan<char> rawInput, string[] tokenLookupTable)
        {
            var indices = new List<(ParseType Type, Range Range)>();
            int lastMatch = 0;

            for (int i = 0; i < rawInput.Length; i++)
            {
                if (FindBestMatch(rawInput, tokenLookupTable, i, out Range range))
                {
                    //preceding literals
                    if (lastMatch < i - 1)
                        indices.Add((ParseType.Literal, new Range(lastMatch, i)));

                    indices.Add((ParseType.Token, range));

                    //adjust indices
                    int tokenLength = range.End.Value - range.Start.Value;
                    lastMatch = i + tokenLength;
                    i += tokenLength - 1;//loop will add 1 by itself
                }
            }

            //include stuff after last token
            var lastToken = indices.LastOrDefault();
            if (lastToken.Type is ParseType.Token && lastToken.Range.End.Value < rawInput.Length)
                indices.Add((ParseType.Literal, new Range(lastToken.Range.End, rawInput.Length)));

            return indices;
        }

        private static bool FindBestMatch(ReadOnlySpan<char> rawInput, string[] tokenLookupTable, int i, out Range range)
        {
            foreach (var token in tokenLookupTable)
            {
                if (token[0] == rawInput[i])
                {
                    //find other candidates; if none, return current. else check next char
                    range = new Range(i, i + 1);
                    return true;
                }
            }

            range = new Range();
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

#if NETFRAMEWORK

    /// <summary>
    /// Simplified backport from .NET Core.
    /// </summary>
    /// <param name="start">Represent the inclusive start index of the range.</param>
    /// <param name="end">Represent the exclusive end index of the range.</param>
    public readonly struct Range(Index start, Index end) : IEquatable<Range>
    {
        /// <summary>Represent the inclusive start index of the Range.</summary>
        public Index Start { get; } = start;

        /// <summary>Represent the exclusive end index of the Range.</summary>
        public Index End { get; } = end;

        /// <summary>Indicates whether the current Range object is equal to another Range object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(Range other) => other.Start.Equals(Start) && other.End.Equals(End);

        public override string ToString()
        {
            return Start.ToString() + ".." + End.ToString();
        }
    }

    /// <summary>
    /// Simplified backport from .NET Core.
    /// </summary>
    public readonly struct Index(int value) : IEquatable<Index>
    {
        /// <summary>Returns the index value.</summary>
        public readonly int Value = value;

        /// <summary>Indicates whether the current Index object is equal to another Index object.</summary>
        /// <param name="other">An object to compare with this object</param>
        public bool Equals(Index other) => Value == other.Value;

        /// <summary>Returns the hash code for this instance.</summary>
        public override int GetHashCode() => Value;

        /// <summary>Converts integer number to an Index.</summary>
        public static implicit operator Index(int value) => new Index(value);

        public override string ToString()
        {
            return Value.ToString();
        }
    }
#endif
}
