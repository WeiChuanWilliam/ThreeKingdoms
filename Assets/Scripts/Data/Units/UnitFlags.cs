namespace ThreeKindoms.Data.Units
{
    /// <summary>
    /// 部隊計略／異常狀態：剩餘天數（0＝無；<see cref="byte"/> 上限 255）。
    /// 行動時以 <c>&gt; 0</c> 視為有效；日出對各欄位各自減 1（斷糧另有結算規則）。
    /// 地形傷害見 <see cref="Terrain.TerrainHazardLevel"/> 與 <see cref="Core.Units.UnitHazardSnapshot"/>。
    /// </summary>
    public sealed class UnitFlags
    {
        /// <summary>混亂剩餘天數（status.on_mess）。</summary>
        public byte Mess { get; set; }

        /// <summary>偽報剩餘天數（status.on_deceive）。</summary>
        public byte Deceive { get; set; }

        /// <summary>止步剩餘天數（status.on_trap；部隊狀態，非格子陷阱）。</summary>
        public byte Stop { get; set; }

        /// <summary>
        /// 斷糧倒數剩餘天數。規則與混亂等不同，見文件：
        /// 兵團被擊破時開始倒數；歸零時清攜糧；倒數期間若糧食補上則立即取消（設 0）。
        /// </summary>
        public byte CutSupply { get; set; }
    }
}
