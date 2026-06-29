using System;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 兵種表初始六圍 → 武將／科技／戰法加權 → 地勢加權 → 最終值。
    /// </summary>
    public static class CombatStatMath
    {
        /// <summary>統率基準值（低於此無加成、高於則按比例提升攻防）。</summary>
        public const int LeadershipBaseline = 50;

        /// <summary>統率每點偏離基準時的攻防倍率增量。</summary>
        public const float LeadershipScalePerPoint = 0.01f;

        static readonly CombatStatKind[] AllKinds =
        {
            CombatStatKind.Attack,
            CombatStatKind.Defense,
            CombatStatKind.Mobility,
            CombatStatKind.Jipo,
            CombatStatKind.Gongcheng,
            CombatStatKind.Stamina,
            CombatStatKind.AttackRange
        };

        /// <summary>兵種表原始值（括號內數字）。來自 <see cref="ICombatTroopStatsSource"/>／<see cref="Combat.TroopAttack"/> 等。</summary>
        public static CombatTroopStatBlock GetBaseTroopStats(ICombatTroopStatsSource source)
        {
            if (source == null) return default;
            return new CombatTroopStatBlock(
                source.TroopAttack,
                source.TroopDefense,
                source.TroopMobility,
                source.TroopJipo,
                source.TroopGongcheng,
                source.TroopStamina,
                source.TroopAttackRange);
        }

        /// <summary>從戰鬥部隊取得兵種表原始六圍。</summary>
        public static CombatTroopStatBlock GetBaseTroopStats(Combat unit) => GetBaseTroopStats((ICombatTroopStatsSource)unit);

        /// <summary>經武將／科技／戰法，尚未乘地勢（括號前數字的前一階，若地勢為 1 則與最終相同）。</summary>
        public static CombatTroopStatBlock GetStatsAfterOfficerAndResearch(Unit unit, ICombatTroopStatsSource source) =>
            CombatStatModifierHooks.ModifyByOfficerSkillAndResearch(unit, source, GetBaseTroopStats(source));

        /// <summary>從戰鬥部隊取得經武將／科技／戰法後的六圍。</summary>
        public static CombatTroopStatBlock GetStatsAfterOfficerAndResearch(Combat unit) =>
            GetStatsAfterOfficerAndResearch(unit, unit);

        /// <summary>最終六圍（括號前數字）。</summary>
        public static CombatTroopStatBlock GetEffectiveTroopStats(Unit unit, ICombatTroopStatsSource source) =>
            CombatStatModifierHooks.ModifyByTerrain(unit, source, GetStatsAfterOfficerAndResearch(unit, source));

        /// <summary>從戰鬥部隊取得含地勢的最終六圍。</summary>
        public static CombatTroopStatBlock GetEffectiveTroopStats(Combat unit) =>
            GetEffectiveTroopStats(unit, unit);

        /// <summary>取得指定維度的最終數值。</summary>
        public static short GetEffective(Unit unit, ICombatTroopStatsSource source, CombatStatKind kind) =>
            GetEffectiveTroopStats(unit, source).Get(kind);

        /// <summary>最終攻擊。</summary>
        public static short GetEffectiveAttack(Combat unit) => GetEffective(unit, unit, CombatStatKind.Attack);

        /// <summary>最終防禦。</summary>
        public static short GetEffectiveDefense(Combat unit) => GetEffective(unit, unit, CombatStatKind.Defense);

        /// <summary>最終機動。</summary>
        public static short GetEffectiveMobility(Combat unit) => GetEffective(unit, unit, CombatStatKind.Mobility);

        /// <summary>最終破甲。</summary>
        public static short GetEffectiveJipo(Combat unit) => GetEffective(unit, unit, CombatStatKind.Jipo);

        /// <summary>最終攻城。</summary>
        public static short GetEffectiveGongcheng(Combat unit) => GetEffective(unit, unit, CombatStatKind.Gongcheng);

        /// <summary>最終耐力。</summary>
        public static short GetEffectiveTroopStamina(Combat unit) => GetEffective(unit, unit, CombatStatKind.Stamina);

        /// <summary>最終攻擊距離。</summary>
        public static short GetEffectiveAttackRange(Combat unit) => GetEffective(unit, unit, CombatStatKind.AttackRange);

        /// <summary>主將統率換算為攻防倍率係數。</summary>
        public static float GetLeadershipFactor(Officer commander)
        {
            if (commander == null) return 1f;
            return 1f + (commander.Leadership - LeadershipBaseline) * LeadershipScalePerPoint;
        }

        /// <summary>套用武將戰法與科技倍率至基礎六圍。</summary>
        internal static CombatTroopStatBlock ApplyOfficerAndResearchMultipliers(
            Unit unit,
            ICombatTroopStatsSource source,
            CombatTroopStatBlock baseStats)
        {
            var result = baseStats;
            foreach (CombatStatKind kind in AllKinds)
            {
                float mult = CombatStatModifierHooks.GetOfficerSkillMultiplier(unit, source, kind)
                             * CombatStatModifierHooks.GetResearchMultiplier(unit, source, kind);
                if (kind == CombatStatKind.Attack || kind == CombatStatKind.Defense)
                    mult *= GetLeadershipFactorForUnit(unit);
                result = result.With(kind, ScaleStat(baseStats.Get(kind), mult));
            }
            return result;
        }

        /// <summary>套用地勢倍率至六圍。</summary>
        internal static CombatTroopStatBlock ApplyTerrainMultipliers(
            Unit unit,
            ICombatTroopStatsSource source,
            CombatTroopStatBlock stats)
        {
            AbstractTerrain terrain = unit?.Location?.CurrentTerrain;
            var result = stats;
            foreach (CombatStatKind kind in AllKinds)
            {
                float mult = CombatStatModifierHooks.GetTerrainMultiplier(unit, source, terrain, kind);
                result = result.With(kind, ScaleStat(stats.Get(kind), mult));
            }
            return result;
        }

        /// <summary>部隊智力（主將與最強副將加權合算）。</summary>
        public static short GetUnitIntelligence(Unit unit)
        {
            if (unit == null) return 0;
            Officer cmd = unit.Commander;
            if (cmd == null && unit.ViceOfficers.Count == 0)
                return 0;

            int cmdIntel = cmd?.Intelligence ?? 0;
            int bestVice = 0;
            foreach (Officer v in unit.ViceOfficers)
            {
                if (v.Intelligence > bestVice)
                    bestVice = v.Intelligence;
            }

            if (cmd == null)
                return (short)bestVice;
            if (bestVice <= 0)
                return (short)cmdIntel;

            return (short)((cmdIntel * 2 + bestVice) / 3);
        }

        static float GetLeadershipFactorForUnit(Unit unit)
        {
            float factor = GetLeadershipFactor(unit?.Commander);
            ApplyViceLeadershipBonus(unit, ref factor);
            return factor;
        }

        static void ApplyViceLeadershipBonus(Unit unit, ref float factor)
        {
            if (unit == null || unit.ViceOfficers.Count == 0) return;
            short bestLead = 0;
            foreach (Officer v in unit.ViceOfficers)
            {
                if (v.Leadership > bestLead)
                    bestLead = v.Leadership;
            }
            if (bestLead <= LeadershipBaseline) return;
            factor += (bestLead - LeadershipBaseline) * LeadershipScalePerPoint * 0.5f;
        }

        static short ScaleStat(short baseStat, float multiplier)
        {
            int scaled = (int)Math.Round(baseStat * multiplier, MidpointRounding.AwayFromZero);
            if (scaled < 0) return 0;
            if (scaled > short.MaxValue) return short.MaxValue;
            return (short)scaled;
        }
    }
}
