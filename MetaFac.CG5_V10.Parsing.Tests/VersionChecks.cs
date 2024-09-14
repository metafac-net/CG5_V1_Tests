using FluentAssertions;
using PublicApiGenerator;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace MetaFac.CG5.Parsing.Tests
{
    public class RegressionChecks
    {
        [Fact]
        public async Task MetaFac_PublicApi_Parsing()
        {
            // act
            var options = new ApiGeneratorOptions()
            {
                IncludeAssemblyAttributes = false
            };
            string currentApi = ApiGenerator.GeneratePublicApi(typeof(Combinators).Assembly, options);

            // assert
            await Verifier.Verify(currentApi);
        }
    }
    public class VersionChecks
    {
        [Fact]
        public void VersionCheck()
        {
            typeof(Combinators).Assembly.FullName.Should().StartWith("MetaFac.CG5.Parsing, Version=1.0.");
        }
    }
}