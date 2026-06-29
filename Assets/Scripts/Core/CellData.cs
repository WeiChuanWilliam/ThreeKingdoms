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
        /// <summary>進入此格消耗的行動力點數。</summary>
        public byte EnterCost;

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
            EnterCost = MovementRules.DefaultDailyPoints,
            CountyId = countyId,
            OwnerId = ownerId,
            Passable = true
        };
    }
}
