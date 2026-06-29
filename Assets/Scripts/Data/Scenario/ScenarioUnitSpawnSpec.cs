using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Data.Scenario
{
    /// <summary>從 .properties 解析出的「開局／劇情放置」一筆部隊（不是玩家 UI 即時組隊）。</summary>
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
        public TroopType TroopType { get; set; } = TroopType.Infantry;
        public string CustomUnitName { get; set; }
        public HexCoord Hex { get; set; }

        public string PropertiesPrefix { get; set; }
    }

    public static class ScenarioUnitSpawnParser
    {
        public static ScenarioUnitSpawnSpec ParseEntry(Dictionary<string, string> map, string prefix)
        {
            string p = prefix + ".";
            var spec = new ScenarioUnitSpawnSpec
            {
                PropertiesPrefix = prefix,
                SpawnKey = prefix,
                UnitType = PropertiesFile.Get(map, p + "type", "combat"),
                FactionId = PropertiesFile.GetInt(map, p + "faction", 1),
                CommanderId = PropertiesFile.GetInt(map, p + "commander"),
                Soldiers = PropertiesFile.GetInt(map, p + "soldiers", 1000),
                Wounded = PropertiesFile.GetInt(map, p + "wounded"),
                Food = PropertiesFile.GetInt(map, p + "food"),
                Morale = (byte)PropertiesFile.GetInt(map, p + "morale", 100),
                Stamina = (byte)PropertiesFile.GetInt(map, p + "stamina", 100),
                TroopType = (TroopType)PropertiesFile.GetInt(map, p + "troop_type"),
                CustomUnitName = PropertiesFile.Get(map, p + "custom_name"),
                Hex = new HexCoord(
                    PropertiesFile.GetInt(map, p + "hex_q"),
                    PropertiesFile.GetInt(map, p + "hex_r"))
            };
            spec.ViceOfficerIds = ParseIdList(PropertiesFile.Get(map, p + "vice"));
            return spec;
        }

        /// <summary>找出所有 spawn.xxx 前綴（xxx 不含點）。</summary>
        public static List<string> FindSpawnPrefixes(Dictionary<string, string> map)
        {
            var prefixes = new HashSet<string>();
            foreach (string key in map.Keys)
            {
                if (!key.StartsWith("spawn.")) continue;
                int dot = key.IndexOf('.', 6);
                if (dot < 0) continue;
                prefixes.Add(key.Substring(0, dot));
            }

            var list = new List<string>(prefixes);
            list.Sort();
            return list;
        }

        static int[] ParseIdList(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return System.Array.Empty<int>();
            string[] parts = csv.Split(',');
            var ids = new List<int>();
            foreach (string part in parts)
            {
                if (int.TryParse(part.Trim(), out int id) && id > 0)
                    ids.Add(id);
            }
            return ids.ToArray();
        }
    }
}
