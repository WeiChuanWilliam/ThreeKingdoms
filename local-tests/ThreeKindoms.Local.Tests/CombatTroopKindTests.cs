using ThreeKindoms.Local.Tests.Runners;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class CombatTroopKindTests
    {
        [Fact]
        public void All_kinds_5000_soldiers_no_skills()
        {
            var r = CombatTroopKindTestRunner.Run(TestPaths.UnitPropertiesPath);
            Assert.True(r.Passed, r.Report);
        }
    }
}
