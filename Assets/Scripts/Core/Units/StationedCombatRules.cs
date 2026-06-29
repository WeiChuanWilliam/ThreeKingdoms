using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 駐紮狀態下的戰鬥規則：無面向向量、據點防禦加成。
    /// 野戰面向見 <c>Docs/COMBAT_AND_CAMPAIGN_ARCHITECTURE.md</c> §4。
    /// </summary>
    public static class StationedCombatRules
    {
        /// <summary>駐紮中不計算面向／背面半防；野戰才套用 Facing 修正。</summary>
        public static bool UsesFacingVector(Unit unit) =>
            unit != null && !unit.IsStationed;

        /// <summary>駐紮且腳下有據點時，依據點類型回傳防禦乘數；否則 1.0。</summary>
        public static float GetDefenseMultiplier(Unit unit)
        {
            if (unit == null || !unit.IsStationed)
                return 1f;

            SettlementSiteKind kind = unit.Building?.SiteKind ?? SettlementSiteKind.None;
            return SettlementSiteRules.GetStationedDefenseMultiplier(kind);
        }

        /// <summary>駐紮且腳下有據點時，回傳防禦加成百分比；否則 0。</summary>
        public static int GetDefenseBonusPercent(Unit unit)
        {
            if (unit == null || !unit.IsStationed)
                return 0;

            SettlementSiteKind kind = unit.Building?.SiteKind ?? SettlementSiteKind.None;
            return SettlementSiteRules.GetStationedDefenseBonusPercent(kind);
        }
    }
}
