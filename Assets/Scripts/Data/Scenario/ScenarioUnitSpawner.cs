using System.Collections.Generic;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Data.Scenario
{
    public sealed class ScenarioSpawnedUnit
    {
        public string SpawnKey { get; init; }
        public Unit Unit { get; init; }
        public Core.HexCoord Hex { get; init; }
    }

    /// <summary>
    /// 劇本設定檔 → 部隊實例。
    /// <para>SKELETON：僅保留方法簽名；與玩家 UI 組隊共用鏈路待實作。</para>
    /// </summary>
    public static class ScenarioUnitSpawner
    {
        /// <summary>從 .properties 載入並建立開局部隊。</summary>
        public static List<ScenarioSpawnedUnit> LoadFromPropertiesFile(string absolutePath) => new List<ScenarioSpawnedUnit>();

        /// <summary>依規格建立部隊（Combat / Legion / Transport）。</summary>
        public static Unit CreateUnit(ScenarioUnitSpawnSpec spec) => null;
    }
}
