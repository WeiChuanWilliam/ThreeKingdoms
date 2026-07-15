using ThreeKindoms.Local.Tests.Runners;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class CombatMusterTests
    {
        [Fact]
        public void Muster_officers_into_combat_with_soldiers()
        {
            GameTestResult result = CombatMusterTestRunner.Run();
            Assert.True(result.Passed, result.Report);
        }
    }
}
