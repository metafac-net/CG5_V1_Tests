using MetaFac.CG5.Parsing;
using System.Collections.Generic;

namespace MetaFac.CG5.Expressions
{
    public record SymbolConstantNode : ConstantNode
    {
        public static SymbolConstantNode Create(Token<CG5Token> value) => new SymbolConstantNode() { Value = value };
        public Token<CG5Token> Value { get; init; }
        public override string ToString() => Value.ToDisplayString();
    }
    public static class CG5TokenExtensions
    {
        public static bool IsCodeToken(this Token<CG5Token> token)
        {
            return (int)token.Kind >= 0x10;
        }

        public static string ToDisplayString(this Token<CG5Token> token)
        {
            return token.Kind switch
            {
                CG5Token.Var => $"[{new string(token.Source.Span)}]",
                CG5Token.Spc => $"Spc[{new string(token.Source.Span)}]",
                _ => new string(token.Source.Span)
            };
        }

        public static IEnumerable<string> ToDisplayStrings(this IEnumerable<Token<CG5Token>> tokens)
        {
            foreach (var token in tokens)
            {
                yield return token.ToDisplayString();
            }
        }

        public static IEnumerable<Token<CG5Token>> SelectCodeTokens(this IEnumerable<Token<CG5Token>> tokens)
        {
            foreach (var token in tokens)
            {
                if (token.IsCodeToken())
                    yield return token;
            }
        }
    }
}
