
using Shouldly;

namespace MetaFac.CG5.Expressions.Tests
{
    public class VersionChecks
    {
        [Fact]
        public void VersionCheck()
        {
            typeof(CG5Parser).Assembly.FullName.ShouldStartWith("MetaFac.CG5.Expressions, Version=1.0.");
        }
    }
}
