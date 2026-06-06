using ThreeKindoms.Local.Tests.Runners;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class UnitPropertiesTests
    {
        [Fact]
        public void Every_registry_kind_has_display_and_six_stats()
        {
            var r = UnitPropertiesTestRunner.Run(TestPaths.UnitPropertiesPath);
            Assert.True(r.Passed, r.Report);
        }
    }
}
