using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>將部隊印成一行（除錯／測試）。</summary>
    public static class CombatUnitDisplay
    {
        /// <summary>將戰鬥部隊格式化成單行輸出。</summary>
        public static string FormatLine(Combat unit, bool passed = true)
        {
            if (unit == null) return "ERROR (null unit)";

            string prefix = passed ? "OK" : "FAIL";
            string kindLabel = string.IsNullOrEmpty(unit.TroopKindKey)
                ? "-"
                : UnitConfigUtil.GetKindDisplayName(unit.TroopKindKey);
            string category = UnitConfigUtil.GetTroopTypeDisplayName(unit.TroopType);
            string commander = unit.Commander == null ? "無" : unit.Commander.FullName;
            string sitePart = "";
            if (unit.IsGarrison && unit.Building != null)
                sitePart = $" 據點{StationRules.GetSiteLabel(unit.Building.SiteKind)}";

            var s = unit.Stats;
            return
                $"{prefix} {unit.UnitName}{sitePart} | 兵科{category} 兵种{kindLabel} | " +
                $"兵力{unit.Soldiers} 士气{unit.Morale} 体力{unit.Stamina} 金{unit.Money} | " +
                $"統{unit.UnitLeadership} 武{unit.UnitForce} 智{unit.UnitIntelligence} 政{unit.UnitPolicy} 魅{unit.UnitCharisma} | " +
                $"攻{s.Attack} 防{s.Defense} 破{s.Jipo} 城{s.Gongcheng} 策{s.Strategy} 建{s.Construction} | " +
                $"主将{commander} 副将{unit.ViceOfficers.Count}";
        }
    }
}
