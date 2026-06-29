using ThreeKindoms.Core.Buildings;

using ThreeKindoms.Data.Units;



namespace ThreeKindoms.Core.Units

{

    /// <summary>

    /// 將部隊印成一行。攻100(100)＝<see cref="CombatTroopStatBlock.Attack"/> 最終值 (括號內為 <see cref="CombatStatMath.GetBaseTroopStats"/> 兵種表初始值)。

    /// </summary>

    public static class CombatUnitDisplay

    {

        /// <summary>將戰鬥部隊格式化成單行除錯／測試輸出。</summary>

        public static string FormatLine(Combat unit, bool passed = true) =>

            FormatTroopStats(unit, unit, unit, passed);



        static string FormatTroopStats(

            Unit unit,

            ICombatTroopStatsSource source,

            Unit statContext,

            bool passed,

            string stationedSite = null)

        {

            if (unit == null) return "ERROR (null unit)";



            string prefix = passed ? "OK" : "FAIL";

            string kindLabel = string.IsNullOrEmpty(source.TroopKindKey)

                ? "-"

                : UnitConfigUtil.GetKindDisplayName(source.TroopKindKey);

            string category = unit is Combat c

                ? UnitConfigUtil.GetTroopTypeDisplayName(c.TroopType)

                : "-";

            string commander = unit.Commander == null ? "無" : unit.Commander.FullName;

            var b = CombatStatMath.GetBaseTroopStats(source);

            var e = CombatStatMath.GetEffectiveTroopStats(statContext, source);

            if (string.IsNullOrEmpty(stationedSite) && unit.IsStationed && unit.Building != null)

                stationedSite = StationRules.GetSiteLabel(unit.Building.SiteKind);

            string sitePart = string.IsNullOrEmpty(stationedSite) ? "" : $" 據點{stationedSite}";



            return

                $"{prefix} {unit.UnitName}{sitePart} | 兵科{category} 兵种{kindLabel} | " +

                $"兵力{unit.Soldiers} 士气{unit.Morale} 体力{unit.Stamina} 金{unit.Money} | " +

                $"攻{e.Attack}({b.Attack}) 防{e.Defense}({b.Defense}) " +

                $"机{e.Mobility}({b.Mobility}) 破{e.Jipo}({b.Jipo}) 城{e.Gongcheng}({b.Gongcheng}) " +

                $"耐{e.Stamina}({b.Stamina}) 距{e.AttackRange}({b.AttackRange}) | " +

                $"智力{CombatStatMath.GetUnitIntelligence(unit)} 主将{commander} 副将{unit.ViceOfficers.Count}" +

                (unit is Combat c2 ? $" 戰鬥力{c2.CombatPower}" : "");

        }

    }

}

