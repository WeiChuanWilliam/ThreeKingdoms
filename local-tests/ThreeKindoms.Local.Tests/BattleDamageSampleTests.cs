using ThreeKindoms.Local.Tests.Runners;
using Xunit;
using Xunit.Abstractions;

namespace ThreeKindoms.Local.Tests
{
    public class BattleDamageSampleTests
    {
        readonly ITestOutputHelper _output;

        public BattleDamageSampleTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void Each_officer_blade_1000_apply_damage_prints_unit_state_before_after()
        {
            GameTestResult result = BattleDamageSampleTestRunner.Run();
            _output.WriteLine(result.Report);
            Assert.True(result.Passed, result.Report);
        }
    }
}
