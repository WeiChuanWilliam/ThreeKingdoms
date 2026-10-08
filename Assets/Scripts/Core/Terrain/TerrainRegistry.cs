using System.Collections.Generic;

namespace ThreeKindoms.Core.Terrain
{
    /// <summary>持有每種地形唯一的實例；Location 以 id 取得地形資料。</summary>
    public static class TerrainRegistry
    {
        // 索引即 TerrainId，順序須與 TerrainType 一致。
        static readonly AbstractTerrain[] terrains =
        {
            new MainRoadTerrain(),
            new MainPathTerrain(),
            new ForestPathTerrain(),
            new PlainTerrain(),
            new GrasslandTerrain(),
            new ForestTerrain(),
            new MountainForestTerrain(),
            new JungleTerrain(),
            new LowMountainTerrain(),
            new MidMountainTerrain(),
            new HighMountainTerrain(),
            new WetlandTerrain(),
            new SandTerrain(),
            new MountainPathTerrain(),
            new PlankRoadTerrain(),
            new PoisonSwampTerrain(),
            new RiverTerrain(),
            new GreatRiverTerrain(),
            new RapidsTerrain(),
            new FrozenRiverTerrain(),
            new CoastTerrain(),
            new CityTerrain(),
            new StockadeTerrain(),
            new ImpassableTerrain(),
        };

        /// <summary>所有地形（依 id 排序）。</summary>
        public static IReadOnlyList<AbstractTerrain> All => terrains;

        /// <summary>依地形類型取得地形。</summary>
        public static AbstractTerrain Get(TerrainType type) => terrains[(int)type];

        /// <summary>依地形 id 取得地形。</summary>
        public static AbstractTerrain Get(int terrainId) => terrains[terrainId];
    }
}
