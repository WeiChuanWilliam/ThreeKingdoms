using ThreeKindoms.Tests;
using ThreeKindoms.Tests.Runners;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>Play 時可選跑 <see cref="GameTestSuite"/>（建議編輯器用選單 ThreeKindoms/Tests）。</summary>
    public class CombatTroopKindTestBootstrap : MonoBehaviour
    {
        [SerializeField] int soldiersPerKind = Runners.CombatTroopKindTestRunner.DefaultSoldiers;
        [SerializeField] int factionId = 1;
        [SerializeField] bool runFullSuiteOnStart = true;

        void Start()
        {
            if (runFullSuiteOnStart)
                RunAllTests();
        }

        [ContextMenu("Run GameTestSuite (All)")]
        public void RunAllTests()
        {
            int code = GameTestSuite.RunAll(GamePaths.ChineseUnitProperties, Debug.Log);
            if (code != 0)
                Debug.LogError(GameTestSuite.LastCombinedReport);
            else
                Debug.Log(GameTestSuite.LastCombinedReport);
        }

        [ContextMenu("Run Combat Troop Kind Only")]
        public void RunCombatOnly()
        {
            var r = Runners.CombatTroopKindTestRunner.Run(GamePaths.ChineseUnitProperties, soldiersPerKind, factionId);
            if (r.Passed)
                Debug.Log(r.Report);
            else
                Debug.LogError(r.Report);
        }
    }
}
