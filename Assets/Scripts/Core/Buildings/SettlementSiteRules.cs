namespace ThreeKindoms.Core.Buildings
{
    /// <summary>
    /// 據點類型分類、駐紮防禦加成與建造分類。
    /// <para>SKELETON：僅保留方法簽名；數值見 Docs/SETTLEMENT_SITES.md，待建築系統實作時填入。</para>
    /// </summary>
    public static class SettlementSiteRules
    {
        /// <summary>是否允許部隊駐紮。</summary>
        public static bool IsStationable(SettlementSiteKind kind) => false;

        /// <summary>地圖內建據點（城池、縣城、港灣、關口）。</summary>
        public static bool IsMapPlaced(SettlementSiteKind kind) => false;

        /// <summary>玩家建造的營寨。</summary>
        public static bool IsPlayerBuilt(SettlementSiteKind kind) => false;

        /// <summary>高防禦建造據點。</summary>
        public static bool IsHighDefenseBuilt(SettlementSiteKind kind) => false;

        /// <summary>低防禦、快速建造據點。</summary>
        public static bool IsLowDefenseBuilt(SettlementSiteKind kind) => false;

        /// <summary>駐紮時全軍防禦加成百分比（0～30）。</summary>
        public static int GetStationedDefenseBonusPercent(SettlementSiteKind kind) => 0;

        /// <summary>駐紮防禦乘數。</summary>
        public static float GetStationedDefenseMultiplier(SettlementSiteKind kind) => 1f;

        /// <summary>據點顯示名稱。</summary>
        public static string GetDisplayName(SettlementSiteKind kind) => "";
    }
}
