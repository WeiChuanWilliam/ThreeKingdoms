using ThreeKindoms.Data.Terrain;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊腳下格環境傷害快照（由 <see cref="UnitLocationBinding"/> 自地形／Location 同步）。
    /// 供火傷等公式讀取；不是 <see cref="UnitFlags"/> 計略狀態。
    /// </summary>
    public readonly struct UnitHazardSnapshot
    {
        public static UnitHazardSnapshot None => default;

        /// <summary>著火格傷害等級。</summary>
        public TerrainHazardLevel FlameLevel { get; }

        /// <summary>陷阱格傷害等級。</summary>
        public TerrainHazardLevel TrapLevel { get; }

        public UnitHazardSnapshot(TerrainHazardLevel flameLevel, TerrainHazardLevel trapLevel)
        {
            FlameLevel = flameLevel;
            TrapLevel = trapLevel;
        }

        /// <summary>火焰與陷阱取較重者。</summary>
        public TerrainHazardLevel EnvironmentalLevel =>
            (TerrainHazardLevel)System.Math.Max((byte)FlameLevel, (byte)TrapLevel);
    }
}
