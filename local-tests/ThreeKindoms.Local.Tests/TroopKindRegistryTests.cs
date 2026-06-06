using ThreeKindoms.Local.Tests.Runners;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class TroopKindRegistryTests
    {
        [Fact]
        public void Count_aliases_and_baima_multiplier()
        {
            var r = TroopKindRegistryTestRunner.Run();
            Assert.True(r.Passed, r.Report);
        }
    }
}
