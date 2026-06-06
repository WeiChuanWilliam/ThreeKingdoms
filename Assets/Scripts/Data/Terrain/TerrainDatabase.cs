using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Terrain;
using UnityEngine;
using TerrainType = ThreeKindoms.Core.TerrainType;

namespace ThreeKindoms.Data.Terrain
{
    public sealed class TerrainDatabase
    {
        readonly Dictionary<int, TerrainDefinition> _definitions = new();

        public IReadOnlyDictionary<int, TerrainDefinition> Definitions => _definitions;

        public static TerrainDatabase LoadFromStreamingAssets(string fileName = "terrains.json")
        {
            var db = new TerrainDatabase();
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            if (!File.Exists(path))
            {
                RegisterDefaults(db);
                Debug.Log("[TerrainDatabase] 無 terrains.json，使用內建預設。");
                return db;
            }

            var json = File.ReadAllText(path);
            var list = JsonUtility.FromJson<TerrainDefList>(json);
            if (list?.terrains != null)
            {
                foreach (var d in list.terrains)
                {
                    var t = new TerrainDefinition(d.id, d.terrainName, d.suggestedEnterCost);
                    t.SetFlags(TerrainFlags.Unpack(d.flagsPacked));
                    t.SetFireEffect(d.fireEffect);
                    t.SetMoralePenalty(d.moralePenalty);
                    db._definitions[d.id] = t;
                }
            }

            Debug.Log($"[TerrainDatabase] 已載入 {db._definitions.Count} 種地形");
            return db;
        }

        static void RegisterDefaults(TerrainDatabase db)
        {
            foreach (TerrainType type in System.Enum.GetValues(typeof(TerrainType)))
            {
                var def = TerrainDefinition.FromTerrainType(type);
                db._definitions[def.TerrainId] = def;
            }
        }

        public TerrainDefinition Get(int terrainId) =>
            _definitions.TryGetValue(terrainId, out var d) ? d : null;

        public byte GetEnterCostForCell(TerrainType spikeEnum)
        {
            var def = TerrainDefinition.FromTerrainType(spikeEnum);
            return def.SuggestedEnterCost;
        }
    }
}
