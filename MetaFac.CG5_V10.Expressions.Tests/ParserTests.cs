﻿using FluentAssertions;
using MetaFac.CG5.Parsing;

namespace MetaFac.CG5.Expressions.Tests
{
    public class ParserTests
    {
        private static ParserInputs<CG5Token> TokenizeSource(string source)
        {
            var lexer = new CG5Lexer();
            var tokens = lexer.GetTokensOnly(source.AsMemory()).SelectCodeTokens().ToArray();
            return new ParserInputs<CG5Token>(tokens);
        }

        [Theory]
        [InlineData("\"\"", "")]
        [InlineData("\"abcdef\"", "abcdef")]
        [InlineData("\"abc\\\"def\"", "abc\\\"def")]
        public void Parse01_StringConstant(string source, string expected)
        {
            // act
            var parser = new CG5Parser();
            var parsed = parser.Parse(source.AsMemory());

            // assert
            var node = parsed as StringConstantNode;
            node.Should().NotBeNull();
            node!.Value.Should().Be(expected);

            // evaluate
            var vars = new Dictionary<string, object?>();

            // assert
        }

        [Theory]
        [InlineData("0", typeof(long), 0)]
        [InlineData("+1", typeof(long), 1)]
        [InlineData("-1", typeof(long), -1)]
        [InlineData("0.0", typeof(double), 0.0D)]
        [InlineData("+1.1", typeof(double), 1.1D)]
        [InlineData("-1.9", typeof(double), -1.9D)]
        [InlineData("2147483647", typeof(long), (long)int.MaxValue)]
        [InlineData("2147483648", typeof(long), (long)int.MaxValue + 1)]
        [InlineData("-2147483648", typeof(long), (long)int.MinValue)]
        [InlineData("-2147483649", typeof(long), (long)int.MinValue - 1)]
        [InlineData("9223372036854775807", typeof(long), long.MaxValue)]
        [InlineData("9223372036854775808", typeof(double), long.MaxValue + 1.0D)]
        [InlineData("-9223372036854775808", typeof(long), long.MinValue)]
        [InlineData("-9223372036854775809", typeof(double), long.MinValue - 1.0D)]
        public void Parse02_NumericConstants(string source, Type et, object ev)
        {
            // act
            var parser = new CG5Parser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.Should().NotBeNull();
            (node as NumericConstantNode).Should().NotBeNull();

            // evaluate
            var vars = new Dictionary<string, object?>();
            var result = node.Evaluate(vars);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType(et);
            result.Should().Be(ev);
        }

        [Theory]
        [InlineData("123 + 456", typeof(long), 579)]
        [InlineData("456 - 123", typeof(long), 333)]
        [InlineData("123.0 + 456.0", typeof(double), 579.0D)]
        [InlineData("456.0 - 123.0", typeof(double), 333.0D)]
        [InlineData("41 * 3", typeof(long), 123)]
        [InlineData("123 / 3", typeof(long), 41)]
        [InlineData("123 % 25", typeof(long), 23)]
        [InlineData("41 ** 2", typeof(double), 1681.0D)] // Math.Pow returns double
        [InlineData("41.0 * 3.0", typeof(double), 123.0D)]
        [InlineData("123.0 / 3.0", typeof(double), 41.0D)]
        [InlineData("123.0 % 25.0", typeof(double), 23.0D)]
        [InlineData("41.0 ** 2.0", typeof(double), 1681.0D)]
        [InlineData("1 < 2", typeof(bool), true)]
        [InlineData("2 < 2", typeof(bool), false)]
        [InlineData("2 > 2", typeof(bool), false)]
        [InlineData("3 > 2", typeof(bool), true)]
        [InlineData("1 <= 2", typeof(bool), true)]
        [InlineData("2 <= 2", typeof(bool), true)]
        [InlineData("3 <= 2", typeof(bool), false)]
        [InlineData("5 == 5", typeof(bool), true)]
        [InlineData("5 != 5", typeof(bool), false)]
        [InlineData("2 == 3", typeof(bool), false)]
        [InlineData("2 != 3", typeof(bool), true)]
        [InlineData("2 > 3 ? 2 : 3", typeof(long), 3)]
        [InlineData("x := 3", typeof(long), 3)]
        [InlineData("false || true", typeof(bool), true)]
        [InlineData("false && true", typeof(bool), false)]
        public void Parse03_Expressions(string source, Type et, object ev)
        {
            // act
            var parser = new CG5Parser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.Should().NotBeNull();

            // evaluate
            var vars = new Dictionary<string, object?>();
            var result = node.Evaluate(vars);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType(et);
            result.Should().Be(ev);
        }

        [Theory]
        [InlineData("12 + 23 + 34", typeof(long), 69)]
        [InlineData("12 + (23 + 34)", typeof(long), 69)]
        [InlineData("(12 + 23) + 34", typeof(long), 69)]
        [InlineData("1 + 2 * 3", typeof(long), 7)]
        [InlineData("1 * 2 + 3", typeof(long), 5)]
        [InlineData("1 + 2 + 3 + 4", typeof(long), 10)]
        [InlineData("1 * 2 + 3 + 4", typeof(long), 9)]
        [InlineData("1 * 2 + 3 * 4", typeof(long), 14)]
        [InlineData("1 * (2 + 3) * 4", typeof(long), 20)]
        [InlineData("((1 + 4) * 3) ** 2", typeof(double), 225)]
        [InlineData("1 + 2 < 3 + 4", typeof(bool), true)]
        [InlineData("1 + 4 == 3 + 2", typeof(bool), true)]
        public void Parse04_OperatorPrecedence(string source, Type et, object ev)
        {
            // act
            var parser = new CG5Parser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.Should().NotBeNull();

            // evaluate
            var vars = new Dictionary<string, object?>();
            var result = node.Evaluate(vars);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType(et);
            result.Should().Be(ev);
        }

        [Theory]
        [InlineData(true, 5, 6)]
        [InlineData(false, 5, 4)]
        public void Parse05_VariableReference(bool valueA, long valueB, int expectedOutput)
        {
            string source = "Calculated := ValueA ? ValueB + 1 : ValueB - 1";

            // act
            var parser = new CG5Parser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.Should().NotBeNull();

            // evaluate
            var vars = new Dictionary<string, object?>();
            vars["ValueA"] = valueA;
            vars["ValueB"] = valueB;
            var result = node.Evaluate(vars);

            // assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(long));
            result.Should().Be(expectedOutput);

            // check output values are set
            vars.Values.Should().HaveCount(3);
            vars.Should().ContainKey("Calculated");
            vars["Calculated"].Should().Be(expectedOutput);
        }
    }
}