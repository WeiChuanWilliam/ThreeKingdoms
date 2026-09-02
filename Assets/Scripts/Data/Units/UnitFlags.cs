using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Data.Units
{
    /// <summary>
    /// 部隊狀態容器：計略／異常負面狀態（混亂、偽報、止步、斷糧）。
    /// 地形傷害見 <see cref="Terrain.TerrainHazardLevel"/> 與 <see cref="Core.Units.UnitHazardSnapshot"/>。
    /// </summary>
    public sealed class UnitFlags
    {
        /// <summary>混亂。</summary>
        public UnitMessStatus Mess { get; } = new();

        /// <summary>偽報。</summary>
        public UnitDeceiveStatus Deceive { get; } = new();

        /// <summary>止步。</summary>
        public UnitStopStatus Stop { get; } = new();

        /// <summary>斷糧。</summary>
        public UnitCutSupplyStatus CutSupply { get; } = new();

        /// <summary>是否有任一負面狀態（目前皆 false）。</summary>
        public bool HasAnyNegative =>
            Mess.IsActive || Deceive.IsActive || Stop.IsActive || CutSupply.IsActive;
    }
}
