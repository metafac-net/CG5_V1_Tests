using Shouldly;
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
            errors.ShouldBeEmpty();
            tokens.Count.ShouldBe(11);
            tokens[0].Kind.ShouldBe(MorseToken.Dit);
            tokens[1].Kind.ShouldBe(MorseToken.Dit);
            tokens[2].Kind.ShouldBe(MorseToken.Dit);
            tokens[3].Kind.ShouldBe(MorseToken.Gap);
            tokens[4].Kind.ShouldBe(MorseToken.Dah);
            tokens[5].Kind.ShouldBe(MorseToken.Dah);
            tokens[6].Kind.ShouldBe(MorseToken.Dah);
            tokens[7].Kind.ShouldBe(MorseToken.Gap);
            tokens[8].Kind.ShouldBe(MorseToken.Dit);
            tokens[9].Kind.ShouldBe(MorseToken.Dit);
            tokens[10].Kind.ShouldBe(MorseToken.Dit);
        }
    }
}
