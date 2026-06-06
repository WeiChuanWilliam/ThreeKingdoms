using ThreeKindoms.Local.Tests.Runners;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class TroopKindTreeTests
    {
        [Fact]
        public void Registry_keys_exist_on_tree_and_navy_chain()
        {
            var r = TroopKindTreeTestRunner.Run();
            Assert.True(r.Passed, r.Report);
        }
    }
}
