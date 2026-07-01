using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Scenario;

namespace ThreeKindoms.Data.Scenario
{
    /// <summary>
    /// 從 .properties 解析開局部隊放置規格。
    /// <para>SKELETON：僅保留資料類別；解析邏輯待劇本格式定稿後實作。</para>
    /// </summary>
    public sealed class ScenarioUnitSpawnSpec
    {
        public string SpawnKey { get; set; } = "default";
        public string UnitType { get; set; } = "combat";
        public int FactionId { get; set; } = 1;
        public int CommanderId { get; set; }
        public IReadOnlyList<int> ViceOfficerIds { get; set; } = System.Array.Empty<int>();
        public int Soldiers { get; set; } = 1000;
        public int Wounded { get; set; }
        public int Food { get; set; }
        public byte Morale { get; set; } = 100;
        public byte Stamina { get; set; } = 100;
        public Units.TroopType TroopType { get; set; } = Units.TroopType.Infantry;
        public string CustomUnitName { get; set; }
        public HexCoord Hex { get; set; }
        public string PropertiesPrefix { get; set; }
    }

    public static class ScenarioUnitSpawnParser
    {
        /// <summary>解析單一 spawn 前綴區塊。</summary>
        public static ScenarioUnitSpawnSpec ParseEntry(Dictionary<string, string> map, string prefix) => new ScenarioUnitSpawnSpec();

        /// <summary>找出所有 spawn.xxx 前綴。</summary>
        public static List<string> FindSpawnPrefixes(Dictionary<string, string> map) => new List<string>();
    }
}
