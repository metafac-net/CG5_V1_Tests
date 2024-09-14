using FluentAssertions;
using MetaFac.CG5.Parsing;

namespace MetaFac.CG5.Expressions.Tests
{
    public class LexerTests
    {
        [Theory]
        [InlineData(CG5Token.Bang, '!', 0x21)]
        [InlineData(CG5Token.Quote, '"', 0x22)]
        [InlineData(CG5Token.Hash, '#', 0x23)]
        [InlineData(CG5Token.Percent, '%', 0x25)]
        [InlineData(CG5Token.Amp, '&', 0x26)]
        [InlineData(CG5Token.Tick, '\'', 0x27)]
        [InlineData(CG5Token.LParen, '(', 0x28)]
        [InlineData(CG5Token.RParen, ')', 0x29)]
        [InlineData(CG5Token.Star, '*', 0x2A)]
        [InlineData(CG5Token.Plus, '+', 0x2B)]
        [InlineData(CG5Token.Comma, ',', 0x2C)]
        [InlineData(CG5Token.Dash, '-', 0x2D)]
        [InlineData(CG5Token.Dot, '.', 0x2E)]
        [InlineData(CG5Token.Slash, '/', 0x2F)]
        [InlineData(CG5Token.Colon, ':', 0x3A)]
        [InlineData(CG5Token.Semi, ';', 0x3B)]
        [InlineData(CG5Token.LAngle, '<', 0x3C)]
        //[InlineData(CG5Token.Equals, '=', 0x3D)]
        [InlineData(CG5Token.RAngle, '>', 0x3E)]
        [InlineData(CG5Token.Quest, '?', 0x3F)]
        [InlineData(CG5Token.At, '@', 0x40)]
        [InlineData(CG5Token.LBrack, '[', 0x5B)]
        [InlineData(CG5Token.Slosh, '\\', 0x5C)]
        [InlineData(CG5Token.RBrack, ']', 0x5D)]
        [InlineData(CG5Token.Hat, '^', 0x5E)]
        [InlineData(CG5Token.Under, '_', 0x5F)]
        [InlineData(CG5Token.Grave, '`', 0x60)]
        [InlineData(CG5Token.LBrace, '{', 0x7B)]
        [InlineData(CG5Token.Pipe, '|', 0x7C)]
        [InlineData(CG5Token.RBrace, '}', 0x7D)]
        [InlineData(CG5Token.Tilde, '~', 0x7E)]
        public void CheckChar(CG5Token kind, char ch, int expectedCode)
        {
            char.IsAscii(ch).Should().BeTrue();
            int ord = ch;
            ord.Should().Be(expectedCode, $"hex value of '{ch}' is 0x{ord:X2}");
            ((int)kind).Should().Be(expectedCode);
        }

        [Fact]
        public void Lex01_Whitespace()
        {
            var source = """   """; // 3 spaces
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            tokens.Count.Should().Be(1);
            tokens[0].Kind.Should().Be(CG5Token.Spc);
            tokens[0].Source.Length.Should().Be(3);
            string.Join(' ', tokens.ToDisplayStrings()).Should().Be("Spc[   ]");
        }

        [Fact]
        public void Lex02_WholeNumber()
        {
            var source = """1234567890""";
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            tokens.Count.Should().Be(1);
            tokens[0].Kind.Should().Be(CG5Token.Num);
            tokens[0].Source.Length.Should().Be(10);
            string.Join(' ', tokens.ToDisplayStrings()).Should().Be("1234567890");
        }

        [Fact]
        public void Lex03_QuotedString()
        {
            var source =
                """
                "a string"
                """;
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            tokens.Count.Should().Be(1);
            tokens[0].Kind.Should().Be(CG5Token.Str);
            tokens[0].Source.Length.Should().Be(10);
            string.Join(' ', tokens.ToDisplayStrings()).Should().Be("\"a string\"");
        }

        [Fact]
        public void Lex04_Multiple()
        {
            var source =
                """
                123456789
                "a string"
                _firstName
                """;
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            string.Join(' ', tokens.SelectCodeTokens().ToDisplayStrings()).Should().Be("123456789 \"a string\" [_firstName]");
        }

        [Fact]
        public void Lex05_ExpressionWithWhitespace()
        {
            var source =
                """
                y := ( a * a + b * b ) ** ( 1 / 2 )
                """;
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            string.Join(' ', tokens.SelectCodeTokens().ToDisplayStrings()).Should()
                .Be("[y] := ( [a] * [a] + [b] * [b] ) ** ( 1 / 2 )");
        }

        [Fact]
        public void Lex06_ExpressionSansWhitespace()
        {
            var source =
                """
                y:=(a*a+b*b)**(1/2)
                """;
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            string.Join(' ', tokens.SelectCodeTokens().ToDisplayStrings()).Should()
                .Be("[y] := ( [a] * [a] + [b] * [b] ) ** ( 1 / 2 )");
        }

        [Fact]
        public void Lex06_QuotedChars()
        {
            var source =
                """
                'x' '\r'
                """;
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            string.Join(' ', tokens.ToDisplayStrings()).Should().Be("'x' Spc[ ] '\\r'");
        }

        [Fact]
        public void Lex07_Keywords()
        {
            var source =
                """
                null false true
                """;
            var lexer = new CG5Lexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<CG5Token>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            string.Join(' ', tokens.SelectCodeTokens().ToDisplayStrings()).Should().Be("null false true");
        }

    }
}