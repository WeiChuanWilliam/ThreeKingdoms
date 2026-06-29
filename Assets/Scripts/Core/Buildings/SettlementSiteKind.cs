namespace ThreeKindoms.Core.Buildings
{
    /// <summary>
    /// 可駐紮據點類型（<see cref="Units.Unit.IsStationed"/>）。
    /// 地圖內建 1～9；玩家可建造 10～19。
    /// </summary>
    public enum SettlementSiteKind : byte
    {
        None = 0,

        // —— 地圖內建（劇本／地圖編輯放置，不可由玩家「新建」此類）——

        /// <summary>城池</summary>
        City = 1,

        /// <summary>縣城</summary>
        CountyTown = 2,

        /// <summary>港灣</summary>
        Harbor = 3,

        /// <summary>關口（名關、關隘）</summary>
        Pass = 4,

        // —— 玩家可建造（野戰營寨，可駐紮）——

        /// <summary>岩砦：造價高、防禦強</summary>
        RockFort = 10,

        /// <summary>要塞：造價高、防禦強</summary>
        Fortress = 11,

        /// <summary>陣：造價低、建造快、防禦弱</summary>
        Camp = 12,

        /// <summary>寨：造價低、建造快、防禦弱</summary>
        Stockade = 13,
    }
}
