namespace ThreeKindoms.Core
{
    /// <summary>部隊移動與地形進入消耗規則。</summary>
    public static class MovementRules
    {
        /// <summary>對應約 15 km/天的行動力。</summary>
        public const int DefaultDailyPoints = 15;

        /// <summary>依地形類型回傳預設進入消耗（行動力點數）。</summary>
        public static int GetDefaultEnterCost(TerrainType terrain) => terrain switch
        {
            TerrainType.Road => 10,
            TerrainType.Plain => 15,
            TerrainType.Forest => 18,
            TerrainType.Hill => 22,
            TerrainType.Mountain => 35,
            TerrainType.Water => 99,
            _ => 15
        };
    }
}
