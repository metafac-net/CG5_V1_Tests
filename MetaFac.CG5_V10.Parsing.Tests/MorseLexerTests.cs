using FluentAssertions;
using System;
using System.Collections.Generic;
using Xunit;

namespace MetaFac.CG5.Parsing.Tests
{
    public class MorseLexerTests
    {
        [Fact]
        public void Lex01_SOS()
        {
            var source = """... --- ...""";
            var lexer = new MorseLexer();

            // act
            var errors = new List<Error>();
            var tokens = new List<Token<MorseToken>>();
            foreach (var result in lexer.GetTokens(source.AsMemory())) { result.Switch(errors.Add, tokens.Add); }

            // assert
            errors.Should().BeEmpty();
            tokens.Count.Should().Be(11);
            tokens[0].Kind.Should().Be(MorseToken.Dit);
            tokens[1].Kind.Should().Be(MorseToken.Dit);
            tokens[2].Kind.Should().Be(MorseToken.Dit);
            tokens[3].Kind.Should().Be(MorseToken.Gap);
            tokens[4].Kind.Should().Be(MorseToken.Dah);
            tokens[5].Kind.Should().Be(MorseToken.Dah);
            tokens[6].Kind.Should().Be(MorseToken.Dah);
            tokens[7].Kind.Should().Be(MorseToken.Gap);
            tokens[8].Kind.Should().Be(MorseToken.Dit);
            tokens[9].Kind.Should().Be(MorseToken.Dit);
            tokens[10].Kind.Should().Be(MorseToken.Dit);
        }
    }
}
