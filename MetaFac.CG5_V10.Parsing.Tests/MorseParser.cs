using System;
using System.Collections.Generic;

namespace MetaFac.CG5.Parsing.Tests
{
    public class MorseParser : Parser<MorseToken, Node>
    {
        public MorseParser() : base(new MorseLexer()) { }
        protected override IEnumerable<Token<MorseToken>> OnFilterTokens(IEnumerable<Token<MorseToken>> tokens) => tokens;
        protected override Node OnMakeErrorNode(string message) => new Node(message);

        private static ParserResult<Node> Match1Token(ParserInputs<MorseToken> input, char result, MorseToken token1)
        {
            var tokens = input.Source.Span;
            if (tokens.Length < 1) return default;
            if (tokens[0].Kind != token1) return default;
            return new ParserResult<Node>(1, new Node(result));
        }

        private static ParserResult<Node> Match2Tokens(ParserInputs<MorseToken> input, char result, MorseToken token1, MorseToken token2)
        {
            var tokens = input.Source.Span;
            if (tokens.Length < 2) return default;
            if (tokens[0].Kind != token1) return default;
            if (tokens[1].Kind != token2) return default;
            return new ParserResult<Node>(2, new Node(result));
        }

        private static ParserResult<Node> Match3Tokens(ParserInputs<MorseToken> input, char result, MorseToken token1, MorseToken token2, MorseToken token3)
        {
            var tokens = input.Source.Span;
            if (tokens.Length < 3) return default;
            if (tokens[0].Kind != token1) return default;
            if (tokens[1].Kind != token2) return default;
            if (tokens[2].Kind != token3) return default;
            return new ParserResult<Node>(3, new Node(result));
        }

        private static ParserResult<Node> CharGap(ParserInputs<MorseToken> input) => Match1Token(input, ' ', MorseToken.Gap);
        private static ParserResult<Node> WordGap(ParserInputs<MorseToken> input) => Match2Tokens(input, ' ', MorseToken.Gap, MorseToken.Gap);
        private static ParserResult<Node> LongGap(ParserInputs<MorseToken> input) => Match3Tokens(input, ' ', MorseToken.Gap, MorseToken.Gap, MorseToken.Gap);

        // todo longest symbols 1st
        private static ParserResult<Node> LetterO(ParserInputs<MorseToken> input) => Match3Tokens(input, 'O', MorseToken.Dah, MorseToken.Dah, MorseToken.Dah);
        private static ParserResult<Node> LetterS(ParserInputs<MorseToken> input) => Match3Tokens(input, 'S', MorseToken.Dit, MorseToken.Dit, MorseToken.Dit);
        private static ParserResult<Node> LetterA(ParserInputs<MorseToken> input) => Match2Tokens(input, 'A', MorseToken.Dit, MorseToken.Dah);

        // todo all symbols
        private static ParserResult<Node> AnyChar(ParserInputs<MorseToken> input) => input.FirstOf(LetterS, LetterO, LetterA);
        private static ParserResult<Node> AnyWord(ParserInputs<MorseToken> input) => Combinators.OneOrMore<MorseToken, Node>(input, AnyChar, CharGap, (n1, op, n2) => Node.Join(n1, n2));
        private static ParserResult<Node> Sentence(ParserInputs<MorseToken> input) => Combinators.OneOrMore<MorseToken, Node>(input, AnyWord, WordGap, (n1, op, n2) => Node.Join(n1, n2, " "));
        private static ParserResult<Node> Paragraph(ParserInputs<MorseToken> input) => Combinators.OneOrMore<MorseToken, Node>(input, Sentence, LongGap, (n1, op, n2) => Node.Join(n1, n2, ". "));

        // todo words

        protected override bool OnTryMatch(ReadOnlyMemory<Token<MorseToken>> tokens, out int consumed, out Node? result)
        {
            ParserInputs<MorseToken> inputs = new ParserInputs<MorseToken>(tokens);
            var parsed = Paragraph(inputs);
            consumed = parsed.Consumed;
            result = parsed.Result;
            return parsed.Matched;
        }
    }
}
