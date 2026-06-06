#if UNITY_INCLUDE_TESTS
using System.IO;
using NUnit.Framework;
using ThreeKindoms.Tests;
using ThreeKindoms.Tests.Runners;
using UnityEngine;

namespace ThreeKindoms.Tests.EditMode
{
    /// <summary>Unity Test Runner（Window → General → Test Runner → EditMode）自動測試。</summary>
    public class GameEditModeTests
    {
        static string PropertiesPath =>
            Path.Combine(Application.dataPath, "StreamingAssets/chinese/unit.properties");

        [Test]
        public void Suite_All_Passes()
        {
            Assert.AreEqual(0, GameTestSuite.RunAll(PropertiesPath), GameTestSuite.LastCombinedReport);
        }

        [Test]
        public void Combat_AllKinds_5000Soldiers()
        {
            var r = CombatTroopKindTestRunner.Run(PropertiesPath);
            Assert.IsTrue(r.Passed, r.Report);
        }

        [Test]
        public void UnitProperties_AllKindsHaveStats()
        {
            var r = UnitPropertiesTestRunner.Run(PropertiesPath);
            Assert.IsTrue(r.Passed, r.Report);
        }

        [Test]
        public void TroopKindTree_RegistryAligned()
        {
            var r = TroopKindTreeTestRunner.Run();
            Assert.IsTrue(r.Passed, r.Report);
        }

        [Test]
        public void TroopKindRegistry_CountAndAliases()
        {
            var r = TroopKindRegistryTestRunner.Run();
            Assert.IsTrue(r.Passed, r.Report);
        }
    }
}
#endif
