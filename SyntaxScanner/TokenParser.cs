using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxScanner
{
    internal class TokenParser
    {
        public static string[] Tokens = ["+", "-", "*", "/", "=", "=="];

        bool IsTerm(ReadOnlySpan<char> rawInput)
        {
            var indices = Formalize(rawInput, Tokens);

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

        /// <summary>
        /// Tests: 
        /// 123 + 47 * 7  -1/3 == 12
        /// b = 12 == false
        /// </summary>
        /// <param name="rawInput"></param>
        /// <param name="tokenLookupTable"></param>
        /// <returns></returns>
        internal List<(ParseType Type, Range Range)> Formalize(ReadOnlySpan<char> rawInput, string[] tokenLookupTable)
        {
            var indices = new List<(ParseType Type, Range Range)>();
            for (int i = 0; i < rawInput.Length; i++)
            {
                if (FindBestMatch(rawInput, tokenLookupTable, i, out Range range))
                {
                    //add all the stuff preceding the token

                    //add current token
                    indices.Add((ParseType.Token, range));
                    i += (range.End.Value - range.Start.Value) - 1;//offset by multi-length
                }
            }

            //include stuff after last token
            var lastToken = indices.LastOrDefault();
            if (lastToken.Type is ParseType.Token && lastToken.Range.End.Value < rawInput.Length)
                indices.Add((ParseType.Other, new Range(lastToken.Range.End, rawInput.Length)));

            return indices;
        }

        private bool FindBestMatch(ReadOnlySpan<char> rawInput, string[] tokenLookupTable, int i, out Range range)
        {
            string candidate;

            foreach (var token in tokenLookupTable)
            {
                if (token[i] == rawInput[i])
                {
                    candidate = token;
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
        Other = 0,
        
        Token = 1,
    }

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
    }
#endif
}
