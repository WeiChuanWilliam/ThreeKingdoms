namespace ThreeKindoms.Core.Buildings
{
    /// <summary>據點類型分類、駐紮防禦加成與建造分類。</summary>
    public static class SettlementSiteRules
    {
        /// <summary>是否允許部隊駐紮。</summary>
        public static bool IsStationable(SettlementSiteKind kind) => kind != SettlementSiteKind.None;

        /// <summary>地圖內建據點（城池、縣城、港灣、關口）。</summary>
        public static bool IsMapPlaced(SettlementSiteKind kind) => kind switch
        {
            SettlementSiteKind.City or SettlementSiteKind.CountyTown
                or SettlementSiteKind.Harbor or SettlementSiteKind.Pass => true,
            _ => false
        };

        /// <summary>玩家可於野戰格建造的營寨（岩砦、要塞、陣、寨）。</summary>
        public static bool IsPlayerBuilt(SettlementSiteKind kind) => kind switch
        {
            SettlementSiteKind.RockFort or SettlementSiteKind.Fortress
                or SettlementSiteKind.Camp or SettlementSiteKind.Stockade => true,
            _ => false
        };

        /// <summary>高防禦建造據點（岩砦、要塞）。</summary>
        public static bool IsHighDefenseBuilt(SettlementSiteKind kind) =>
            kind is SettlementSiteKind.RockFort or SettlementSiteKind.Fortress;

        /// <summary>低防禦、快速建造據點（陣、寨）。</summary>
        public static bool IsLowDefenseBuilt(SettlementSiteKind kind) =>
            kind is SettlementSiteKind.Camp or SettlementSiteKind.Stockade;

        /// <summary>
        /// 駐紮時據點對<strong>全軍防禦</strong>的加成百分比（0～30）。
        /// 見 <c>Docs/SETTLEMENT_SITES.md</c> §7。
        /// </summary>
        public static int GetStationedDefenseBonusPercent(SettlementSiteKind kind) => kind switch
        {
            SettlementSiteKind.Camp => 5,
            SettlementSiteKind.Stockade => 10,
            SettlementSiteKind.RockFort => 15,
            SettlementSiteKind.CountyTown => 20,
            SettlementSiteKind.Fortress => 25,
            SettlementSiteKind.Harbor => 25,
            SettlementSiteKind.Pass => 30,
            SettlementSiteKind.City => 30,
            _ => 0
        };

        /// <summary>駐紮防禦乘數，例如 +20% → 1.20。</summary>
        public static float GetStationedDefenseMultiplier(SettlementSiteKind kind)
        {
            int pct = GetStationedDefenseBonusPercent(kind);
            return 1f + pct / 100f;
        }

        /// <summary>據點顯示名稱。</summary>
        public static string GetDisplayName(SettlementSiteKind kind) => kind switch
        {
            SettlementSiteKind.City => "城池",
            SettlementSiteKind.CountyTown => "縣城",
            SettlementSiteKind.Harbor => "港灣",
            SettlementSiteKind.Pass => "關口",
            SettlementSiteKind.RockFort => "岩砦",
            SettlementSiteKind.Fortress => "要塞",
            SettlementSiteKind.Camp => "陣",
            SettlementSiteKind.Stockade => "寨",
            _ => ""
        };
    }
}
