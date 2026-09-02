using ThreeKindoms.Local.Tests.Runners;
using Xunit;
using Xunit.Abstractions;

namespace ThreeKindoms.Local.Tests
{
    public class LiuBeiInjuryCompareTests
    {
        readonly ITestOutputHelper _output;

        public LiuBeiInjuryCompareTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void Liu_bei_same_soldiers_injury_levels_compare_stats()
        {
            GameTestResult result = LiuBeiInjuryCompareTestRunner.Run();
            _output.WriteLine(result.Report);
            Assert.True(result.Passed, result.Report);
        }
    }
}
