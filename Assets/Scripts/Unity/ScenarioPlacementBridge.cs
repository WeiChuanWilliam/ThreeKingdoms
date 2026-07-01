using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>
    /// 劇本部隊放置橋接（StreamingAssets → 地圖格）。
    /// <para>SKELETON：僅保留方法簽名；開局放置待劇本流程定稿後實作。</para>
    /// </summary>
    public static class ScenarioPlacementBridge
    {
        /// <summary>載入劇本並將部隊綁定至地圖格。</summary>
        public static List<Data.Scenario.ScenarioSpawnedUnit> LoadAndPlace(
            string propertiesFileName,
            LocationGrid locations,
            HexGrid grid) => new List<Data.Scenario.ScenarioSpawnedUnit>();

        /// <summary>將單一部隊放置於指定 hex。</summary>
        public static void PlaceUnit(Unit unit, LocationGrid locations, HexGrid grid, HexCoord hex) { }
    }
}
