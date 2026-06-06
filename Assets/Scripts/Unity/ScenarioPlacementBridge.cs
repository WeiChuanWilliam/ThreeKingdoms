using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Scenario;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    public static class ScenarioPlacementBridge
    {
        public static List<ScenarioSpawnedUnit> LoadAndPlace(
            string propertiesFileName,
            LocationGrid locations,
            HexGrid grid)
        {
            string jsonPath = Path.Combine(Application.streamingAssetsPath, "scenarios", "opening.json");
            List<ScenarioSpawnedUnit> spawns = File.Exists(jsonPath)
                ? ScenarioJsonLoader.LoadAndSpawnUnits(jsonPath)
                : new List<ScenarioSpawnedUnit>();

            if (spawns.Count == 0)
            {
                string path = Path.Combine(Application.streamingAssetsPath, propertiesFileName);
                spawns = ScenarioUnitSpawner.LoadFromPropertiesFile(path);
            }
            foreach (ScenarioSpawnedUnit s in spawns)
                PlaceUnit(s.Unit, locations, grid, s.Hex);
            return spawns;
        }

        public static void PlaceUnit(Unit unit, LocationGrid locations, HexGrid grid, HexCoord hex)
        {
            if (unit == null) return;
            if (!grid.TryGet(hex, out var cell))
            {
                Debug.LogWarning($"[Scenario] 格子不存在 {hex}，部隊 {unit.UnitName} 未放置");
                return;
            }
            unit.Location.BindToWorld(locations, hex, TerrainDefinition.FromTerrainType(cell.Terrain));
        }
    }
}
