using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Locations;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊駐紮規則。
    /// 駐紮不是獨立部隊類型：仍是 <see cref="Combat"/> 或 <see cref="Legion"/>，
    /// 僅透過 <see cref="Unit.IsStationed"/> 布林值切換狀態。
    /// </summary>
    public static class StationRules
    {
        /// <summary>建築是否可作為駐紮據點。</summary>
        public static bool IsStationSite(AbstractBuilding building) =>
            building != null && building.IsGarrisonSite;

        /// <summary>據點類型對應的顯示名稱。</summary>
        public static string GetSiteLabel(SettlementSiteKind kind) =>
            SettlementSiteRules.GetDisplayName(kind);

        /// <summary>
        /// 部隊進入可駐紮據點格時，自動設 <see cref="Unit.IsStationed"/> = true。
        /// 由 <see cref="UnitLocationBinding"/> 在進格時呼叫。
        /// </summary>
        public static void TryAutoStation(Unit unit, MapLocation location)
        {
            if (unit == null || location == null || unit.IsStationed)
                return;
            if (unit is not Combat and not Legion)
                return;

            AbstractBuilding site = location.Building ?? unit.Building;
            if (!IsStationSite(site))
                return;

            unit.SetStationed(true);
            if (site != null)
                unit.SetBuilding(site);
        }

        /// <summary>離開駐紮狀態，恢復可移動（仍須自行離開據點格或另行設計）。</summary>
        public static void DepartStation(Unit unit)
        {
            if (unit == null) return;
            unit.SetStationed(false);
        }
    }
}
