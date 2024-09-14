using MetaFac.CG5.Parsing.Builtin;
using System.Collections.Generic;

namespace MetaFac.CG5.Parsing.Tests
{
    public sealed class MorseLexer : Lexer<MorseToken>
    {
        private static readonly List<ITokenMatcher<MorseToken>> _matchers =
        [
            // single char symbol matchers
            new CharMatcher<MorseToken>(
                new Dictionary<char, MorseToken>()
                {
                    [' '] = MorseToken.Gap,
                    ['.'] = MorseToken.Dit,
                    ['-'] = MorseToken.Dah,
                }),
        ];

        public MorseLexer() : base(_matchers) { }
    }
}
