using System.Collections.Generic;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Data.Scenario
{
    public sealed class ScenarioSpawnedUnit
    {
        public string SpawnKey { get; init; }
        public Unit Unit { get; init; }
        public Core.HexCoord Hex { get; init; }
    }

    /// <summary>
    /// 讀 .properties → 建 <see cref="UnitDef"/> → <c>new Combat(def)</c> 等。
    /// 與「玩家 UI 組隊」共用同一條建構鏈，只是資料來自設定檔。
    /// </summary>
    public static class ScenarioUnitSpawner
    {
        public static List<ScenarioSpawnedUnit> LoadFromPropertiesFile(string absolutePath)
        {
            var map = PropertiesFile.LoadFromFile(absolutePath);
            var result = new List<ScenarioSpawnedUnit>();
            foreach (string prefix in ScenarioUnitSpawnParser.FindSpawnPrefixes(map))
            {
                ScenarioUnitSpawnSpec spec = ScenarioUnitSpawnParser.ParseEntry(map, prefix);
                result.Add(new ScenarioSpawnedUnit
                {
                    SpawnKey = spec.SpawnKey,
                    Unit = CreateUnit(spec),
                    Hex = spec.Hex
                });
            }
            return result;
        }

        public static Unit CreateUnit(ScenarioUnitSpawnSpec spec)
        {
            UnitDef def = BuildDef(spec);
            return def switch
            {
                CombatUnitDef combat => new Combat(combat),
                LegionUnitDef legion => new Legion(legion),
                TransportUnitDef transport => new Transport(transport),
                _ => new Combat((CombatUnitDef)def)
            };
        }

        static UnitDef BuildDef(ScenarioUnitSpawnSpec spec)
        {
            string type = spec.UnitType?.ToLowerInvariant() ?? "combat";
            UnitDef def = type switch
            {
                "legion" => new LegionUnitDef(spec.FactionId),
                "transport" => new TransportUnitDef(spec.FactionId),
                _ => new CombatUnitDef(spec.FactionId)
            };

            def.CommanderOfficerId = spec.CommanderId;
            def.Soldiers = spec.Soldiers;
            def.Wounded = spec.Wounded;
            def.Morale = spec.Morale;
            def.Stamina = spec.Stamina;
            if (!string.IsNullOrWhiteSpace(spec.CustomUnitName))
                def.CustomUnitName = spec.CustomUnitName.Trim();

            foreach (int viceId in spec.ViceOfficerIds)
                def.AddViceOfficer(viceId);

            if (def is CombatUnitDef combat)
                combat.TroopType = spec.TroopType;

            if (def is LegionUnitDef legion)
                legion.Food = spec.Food;

            return def;
        }
    }
}
