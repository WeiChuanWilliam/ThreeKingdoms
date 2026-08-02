using ThreeKindoms.Local.Tests.Runners;
using Xunit;
using Xunit.Abstractions;

namespace ThreeKindoms.Local.Tests
{
    public class CombatStatsSampleTests
    {
        readonly ITestOutputHelper _output;

        public CombatStatsSampleTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void Each_officer_blade_1000_prints_six_stats()
        {
            GameTestResult result = CombatStatsSampleTestRunner.Run();
            _output.WriteLine(result.Report);
            Assert.True(result.Passed, result.Report);
        }
    }
}
