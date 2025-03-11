using Shouldly;
using System;
using Xunit;

namespace MetaFac.CG5.Parsing.Tests
{
    public class MorseParseTests
    {
        [Fact]
        public void Parse01_Letter()
        {
            var source = """.-""";

            // act
            var parser = new MorseParser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.HasErrors.ShouldBeFalse();
            node.Result.ShouldBe("A");
        }

        [Fact]
        public void Parse02_Word()
        {
            var source = """... --- ...""";

            // act
            var parser = new MorseParser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.HasErrors.ShouldBeFalse();
            node.Result.ShouldBe("SOS");
        }

        [Fact]
        public void Parse03_Sentence()
        {
            var source = """... --- ...  ... --- ...""";

            // act
            var parser = new MorseParser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.HasErrors.ShouldBeFalse();
            node.Result.ShouldBe("SOS SOS");
        }

        [Fact]
        public void Parse04_Paragraph()
        {
            var source = """... --- ...   ... --- ...  ... --- ...""";

            // act
            var parser = new MorseParser();
            var node = parser.Parse(source.AsMemory());

            // assert
            node.HasErrors.ShouldBeFalse();
            node.Result.ShouldBe("SOS. SOS SOS");
        }
    }
}
