using ThreeKindoms.Core.Terrain;

namespace ThreeKindoms.Core
{
    /// <summary>地形類型；數值即 <see cref="AbstractTerrain.TerrainId"/>。</summary>
    public enum TerrainType : byte
    {
        MainRoad = 0,       // 官道
        MainPath = 1,       // 主徑
        ForestPath = 2,     // 林道
        Plain = 3,          // 平地
        Grassland = 4,      // 草原
        Forest = 5,         // 森林
        MountainForest = 6, // 山林
        Jungle = 7,         // 叢林
        LowMountain = 8,    // 低山
        MidMountain = 9,    // 中山
        HighMountain = 10,  // 高山
        Wetland = 11,       // 濕地
        Sand = 12,          // 沙地
        MountainPath = 13,  // 山路
        PlankRoad = 14,     // 棧道
        PoisonSwamp = 15,   // 毒沼
        River = 16,         // 河流
        GreatRiver = 17,    // 大河
        Rapids = 18,        // 急流
        FrozenRiver = 19,   // 凍河
        Coast = 20,         // 沿海
        City = 21,          // 城池（城池、關隘、要塞、岩砦）
        Stockade = 22,      // 城寨（港灣、陣、寨、縣城）
        Impassable = 23     // 不可通行
    }

    public struct CellData
    {
        /// <summary>地圖格用地形類型；對應 <see cref="ThreeKindoms.Core.Terrain.AbstractTerrain"/>。</summary>
        public TerrainType Terrain;

        /// <summary>所屬郡縣 id。</summary>
        public ushort CountyId;

        /// <summary>格擁有者勢力 id。</summary>
        public byte OwnerId;

        /// <summary>是否可通行（尋路與移動用）。</summary>
        public bool Passable;

        /// <summary>建立預設平原格資料。</summary>
        public static CellData Plain(ushort countyId = 0, byte ownerId = 0) => new()
        {
            Terrain = TerrainType.Plain,
            CountyId = countyId,
            OwnerId = ownerId,
            Passable = true
        };
    }
}
