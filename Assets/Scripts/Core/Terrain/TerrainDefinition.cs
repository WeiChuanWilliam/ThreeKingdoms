using ThreeKindoms.Core;
using ThreeKindoms.Data.Terrain;

namespace ThreeKindoms.Core.Terrain
{
    /// <summary>可執行的地形定義（從表或程式建立）。</summary>
    public sealed class TerrainDefinition : AbstractTerrain
    {
        public byte SuggestedEnterCost { get; private set; }

        public TerrainDefinition(int id, string name, byte suggestedEnterCost = MovementRules.DefaultDailyPoints)
            : base(id, name)
        {
            SuggestedEnterCost = suggestedEnterCost;
        }

        public static TerrainDefinition FromTerrainType(TerrainType type)
        {
            switch (type)
            {
                case TerrainType.Road:
                    var road = new TerrainDefinition(1, "官道", 10);
                    road.SetFlags(new TerrainFlags { Reachable = true, Construction = true });
                    return road;
                case TerrainType.Hill:
                    var hill = new TerrainDefinition(2, "丘陵", 22);
                    hill.SetFlags(new TerrainFlags { Reachable = true });
                    return hill;
                case TerrainType.Mountain:
                    var mtn = new TerrainDefinition(3, "山地", 35);
                    mtn.SetFlags(new TerrainFlags { Reachable = true, Fort = true });
                    return mtn;
                case TerrainType.Water:
                    var water = new TerrainDefinition(4, "水域", 99);
                    water.SetFlags(new TerrainFlags { Reachable = false });
                    return water;
                case TerrainType.Forest:
                    var forest = new TerrainDefinition(5, "森林", 18);
                    forest.SetFlags(new TerrainFlags { Reachable = true, Fireable = true });
                    return forest;
                default:
                    var plain = new TerrainDefinition(0, "平原", MovementRules.DefaultDailyPoints);
                    plain.SetFlags(new TerrainFlags { Reachable = true, Construction = true });
                    return plain;
            }
        }
    }
}
