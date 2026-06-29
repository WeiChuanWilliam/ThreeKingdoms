using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ThreeKindoms.Data.Scenario
{
    public static class ScenarioJsonLoader
    {
        public static ScenarioDocument LoadDocument(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                Debug.LogWarning($"[Scenario] 找不到 {absolutePath}");
                return new ScenarioDocument();
            }

            string json = File.ReadAllText(absolutePath);
            var doc = JsonUtility.FromJson<ScenarioDocument>(json);
            return doc ?? new ScenarioDocument();
        }

        public static List<ScenarioSpawnedUnit> LoadAndSpawnUnits(string absolutePath)
        {
            ScenarioDocument doc = LoadDocument(absolutePath);
            var result = new List<ScenarioSpawnedUnit>();
            if (doc.units == null) return result;

            foreach (ScenarioUnitEntry entry in doc.units)
            {
                ScenarioUnitSpawnSpec spec = ToSpec(entry);
                result.Add(new ScenarioSpawnedUnit
                {
                    SpawnKey = string.IsNullOrEmpty(entry.spawnKey) ? "unit" : entry.spawnKey,
                    Unit = ScenarioUnitSpawner.CreateUnit(spec),
                    Hex = spec.Hex
                });
            }

            return result;
        }

        public static ScenarioUnitSpawnSpec ToSpec(ScenarioUnitEntry e) => new()
        {
            SpawnKey = e.spawnKey,
            UnitType = e.type,
            FactionId = e.faction,
            CommanderId = e.commander,
            Food = e.food,
            ViceOfficerIds = e.vice ?? System.Array.Empty<int>(),
            Soldiers = e.soldiers,
            Wounded = e.wounded,
            Morale = e.morale,
            Stamina = e.stamina,
            TroopType = (Units.TroopType)e.troopType,
            CustomUnitName = e.customName,
            Hex = new Core.HexCoord(e.hexQ, e.hexR)
        };
    }
}
