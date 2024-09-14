using FluentAssertions;

namespace MetaFac.CG5.Expressions.Tests
{
    public class VersionChecks
    {
        [Fact]
        public void VersionCheck()
        {
            typeof(CG5Parser).Assembly.FullName.Should().StartWith("MetaFac.CG5.Expressions, Version=1.0.");
        }
    }
}
