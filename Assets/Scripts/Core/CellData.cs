namespace ThreeKindoms.Core
{
    public enum TerrainType : byte
    {
        Plain = 0,
        Road = 1,
        Hill = 2,
        Mountain = 3,
        Water = 4,
        Forest = 5
    }

    public struct CellData
    {
        /// <summary>Spike 用 enum；完整版可改為 <c>terrainDefId</c> 對應 TerrainDefinition。</summary>
        public TerrainType Terrain;
        public byte EnterCost;
        public ushort CountyId;
        public byte OwnerId;
        public bool Passable;

        public static CellData Plain(ushort countyId = 0, byte ownerId = 0) => new()
        {
            Terrain = TerrainType.Plain,
            EnterCost = MovementRules.DefaultDailyPoints,
            CountyId = countyId,
            OwnerId = ownerId,
            Passable = true
        };
    }
}
