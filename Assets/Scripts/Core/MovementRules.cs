namespace ThreeKindoms.Core
{
    public static class MovementRules
    {
        /// <summary>對應約 15 km/天的行動力。</summary>
        public const int DefaultDailyPoints = 15;

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
