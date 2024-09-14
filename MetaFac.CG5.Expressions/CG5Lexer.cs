using MetaFac.CG5.Parsing;
using MetaFac.CG5.Parsing.Builtin;
using System.Collections.Generic;

namespace MetaFac.CG5.Expressions
{
    public sealed class CG5Lexer : Lexer<CG5Token>
    {
        private static readonly List<ITokenMatcher<CG5Token>> _matchers =
        [
            // order is important here
            new LineSeparator<CG5Token>(CG5Token.EOL),
            new WhiteSpace<CG5Token>(CG5Token.Spc),
            new DecimalNumber<CG5Token>(CG5Token.Num),
            new CSharpIdentifier<CG5Token>(CG5Token.Var, new Dictionary<string, CG5Token>()
            {
                ["true"] = CG5Token.Bool,
                ["false"] = CG5Token.Bool,
                ["null"] = CG5Token.Null,
            }),
            new DoubleQuotedString<CG5Token>(CG5Token.Str),
            new SingleQuotedChar<CG5Token>(CG5Token.Chr),
            // double char symbol matchers
            new StringMatcher<CG5Token>(2,
                new Dictionary<string, CG5Token>()
                {
                    //["<<"] = MyToken.SHL,
                    //[">>"] = MyToken.SHR,
                    ["=="] = CG5Token.EQU,
                    ["!="] = CG5Token.NEQ,
                    [">="] = CG5Token.GEQ,
                    ["<="] = CG5Token.LEQ,
                    [":="] = CG5Token.Assign,
                    ["**"] = CG5Token.Power,
                    ["&&"] = CG5Token.AND,
                    ["||"] = CG5Token.OR,
                    // +=
                    // -=
                    // *=
                    // /=
                    // %=
                }),
            // single char symbol matchers
            new CharMatcher<CG5Token>(
                new Dictionary<char, CG5Token>()
                {
                    //['='] = CG5Token.Equals,
                    ['+'] = CG5Token.Plus,
                    ['-'] = CG5Token.Dash,
                    ['*'] = CG5Token.Star,
                    ['/'] = CG5Token.Slash,
                    ['%'] = CG5Token.Percent,
                    ['#'] = CG5Token.Hash,
                    ['!'] = CG5Token.Bang,
                    ['^'] = CG5Token.Hat,
                    ['.'] = CG5Token.Dot,
                    ['&'] = CG5Token.Amp,
                    ['@'] = CG5Token.At,
                    ['?'] = CG5Token.Quest,
                    [':'] = CG5Token.Colon,
                    //['"'] = MyToken.Quote, conflicts with literal string
                    //['\''] = MyToken.Tick, conflicts with literal char
                    ['\\'] = CG5Token.Slosh,
                    ['('] = CG5Token.LParen,
                    [')'] = CG5Token.RParen,
                    ['['] = CG5Token.LBrack,
                    [']'] = CG5Token.RBrack,
                    ['{'] = CG5Token.LBrace,
                    ['}'] = CG5Token.RBrace,
                    ['<'] = CG5Token.LAngle,
                    ['>'] = CG5Token.RAngle,
                    ['~'] = CG5Token.Tilde,
                    //['_'] = MyToken.Under, conflicts with CSharpName
                }),
        ];

        public CG5Lexer() : base(_matchers) { }
    }
}
