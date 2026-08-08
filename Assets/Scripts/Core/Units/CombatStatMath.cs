using System;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊屬性兩段式：
    /// A) <see cref="RefreshOfficerFive"/>：武將狀態 → 寫回部隊統合五維（統武智政魅）— 隨武將變更更新。
    /// B) <see cref="Recalculate"/>：五維＋兵力／士氣／體力 → <see cref="CombatTroopStatBlock"/> — 戰鬥／查看時再算。
    /// 分流：類型 Combat／Transport／Legion；Combat／Legion 再看駐紮。Transport 不看駐紮。
    /// 目前只有「非駐紮 Combat」有完整戰鬥力公式。
    /// </summary>
    public static class CombatStatMath
    {
        /// <summary>尚未實作路徑的暫代結果（各維 1）。</summary>
        public static CombatTroopStatBlock PlaceholderOnes { get; } =
            new CombatTroopStatBlock(1, 1, 1, 1, 1, 1);

        // -------------------------------------------------------------------------
        // A) 統合五維：隨時可寫回部隊
        // -------------------------------------------------------------------------

        /// <summary>依主副將 Effective* 重算並寫入 <see cref="Combat.OfficerFive"/>。</summary>
        public static void RefreshOfficerFive(Combat combat)
        {
            if (combat == null)
                return;
            combat.ApplyOfficerFive(ComputeOfficerFive(combat));
        }

        /// <summary>只計算統合五維，不寫回。</summary>
        public static UnitOfficerFiveBlock ComputeOfficerFive(Unit unit)
        {
            if (unit == null)
                return UnitOfficerFiveBlock.Zero;

            return new UnitOfficerFiveBlock(
                BlendOfficerStat(unit, o => o.EffectiveLeadership),
                BlendOfficerStat(unit, o => o.EffectiveAttack),
                BlendOfficerStat(unit, o => o.EffectiveIntelligence),
                BlendOfficerStat(unit, o => o.EffectivePolicy),
                BlendOfficerStat(unit, o => o.EffectiveCharisma));
        }

        /// <summary>
        /// 【五維合成公式｜之後改這裡】
        /// 主將權重 2、該屬性最強副將權重 1：結果 = (主將×2 + 最佳副將) / 3。
        /// </summary>
        static short BlendOfficerStat(Unit unit, Func<Officer, byte> selector)
        {
            if (unit == null) return 0;
            Officer cmd = unit.Commander;
            if (cmd == null && unit.ViceOfficers.Count == 0)
                return 0;

            int bestVice = 0;
            foreach (Officer v in unit.ViceOfficers)
            {
                int s = selector(v);
                if (s > bestVice)
                    bestVice = s;
            }

            if (cmd == null)
                return (short)bestVice;

            int cmdStat = selector(cmd);
            if (bestVice <= 0)
                return (short)cmdStat;

            return (short)((cmdStat * 2 + bestVice) / 3);
        }

        // -------------------------------------------------------------------------
        // B) 戰鬥力 CombatTroopStatBlock：戰鬥／查看時再算
        // -------------------------------------------------------------------------

        /// <summary>重算並寫回（僅 <see cref="Combat"/> 有 Stats）。戰鬥／查看時呼叫。</summary>
        public static void Recalculate(Unit unit)
        {
            if (unit is not Combat combat)
                return;

            combat.ApplyStats(
                combat.IsAnnihilated ? CombatTroopStatBlock.Zero : Calculate(combat));
        }

        /// <summary>
        /// 只計算不寫回。
        /// 第 1 層：Combat / Transport / Legion；
        /// 第 2 層（Combat、Legion）：是否駐紮。Transport 不看駐紮。
        /// </summary>
        public static CombatTroopStatBlock Calculate(Unit unit)
        {
            if (unit == null)
                return CombatTroopStatBlock.Zero;

            switch (unit)
            {
                case Combat combat:
                    return CalculateCombat(combat);
                case Transport transport:
                    return CalculateTransport(transport);
                case Legion legion:
                    return CalculateLegion(legion);
                default:
                    return PlaceholderOnes;
            }
        }

        static CombatTroopStatBlock CalculateCombat(Combat combat)
        {
            if (combat.IsAnnihilated)
                return CombatTroopStatBlock.Zero;

            if (combat.IsGarrison)
                return CalculateCombatGarrison(combat);

            return CalculateCombatField(combat);
        }

        /// <summary>
        /// 【非駐紮 Combat｜目前唯一完整公式】
        /// 讀已寫回的 <see cref="Combat.OfficerFive"/>，再乘兵力／士氣／體力（暫代 Placeholder）。
        /// </summary>
        static CombatTroopStatBlock CalculateCombatField(Combat combat)
        {
            var five = combat.OfficerFive;
            return new CombatTroopStatBlock(
                Placeholder(five.Force, combat),
                Placeholder(five.Leadership, combat),
                Placeholder(five.Force, combat),
                Placeholder(five.Force, combat),
                Placeholder(five.Intelligence, combat),
                Placeholder(five.Policy, combat));
        }

        /// <summary>【駐紮 Combat｜TODO】</summary>
        static CombatTroopStatBlock CalculateCombatGarrison(Combat combat) =>
            PlaceholderOnes;

        /// <summary>【Transport｜TODO】不分支駐紮。</summary>
        static CombatTroopStatBlock CalculateTransport(Transport transport) =>
            PlaceholderOnes;

        static CombatTroopStatBlock CalculateLegion(Legion legion) =>
            legion.IsGarrison ? CalculateLegionGarrison(legion) : CalculateLegionField(legion);

        /// <summary>【非駐紮 Legion｜TODO】</summary>
        static CombatTroopStatBlock CalculateLegionField(Legion legion) =>
            PlaceholderOnes;

        /// <summary>【駐紮 Legion｜TODO】</summary>
        static CombatTroopStatBlock CalculateLegionGarrison(Legion legion) =>
            PlaceholderOnes;

        /// <summary>是否走「非駐紮 Combat」完整公式。</summary>
        public static bool UsesFullCombatFieldFormula(Unit unit) =>
            unit is Combat combat && !combat.IsGarrison && !combat.IsAnnihilated;

        // -------------------------------------------------------------------------
        // 單維入口（戰鬥／查看用；讀 OfficerFive）
        // -------------------------------------------------------------------------

        public static int CalculateAttack(Unit unit) =>
            ResolveCombatFieldStat(unit, c => Placeholder(c.OfficerFive.Force, c));

        public static int CalculateDefense(Unit unit) =>
            ResolveCombatFieldStat(unit, c => Placeholder(c.OfficerFive.Leadership, c));

        public static int CalculateJipo(Unit unit) =>
            ResolveCombatFieldStat(unit, c => Placeholder(c.OfficerFive.Force, c));

        public static int CalculateGongcheng(Unit unit) =>
            ResolveCombatFieldStat(unit, c => Placeholder(c.OfficerFive.Force, c));

        public static int CalculateStrategy(Unit unit) =>
            ResolveCombatFieldStat(unit, c => Placeholder(c.OfficerFive.Intelligence, c));

        public static int CalculateConstruction(Unit unit) =>
            ResolveCombatFieldStat(unit, c => Placeholder(c.OfficerFive.Policy, c));

        static int ResolveCombatFieldStat(Unit unit, Func<Combat, int> fullFormula)
        {
            if (unit == null)
                return 0;
            if (unit is not Combat combat)
                return 1;
            if (combat.IsAnnihilated)
                return 0;
            if (!UsesFullCombatFieldFormula(combat))
                return 1;
            return fullFormula(combat);
        }

        /// <summary>
        /// 【暫代共同倍率｜之後可刪】屬性 × 健康兵力/100 × 士氣/100 × 體力/100。
        /// </summary>
        static int Placeholder(int officerStat, Unit unit)
        {
            if (unit == null || officerStat <= 0)
                return 0;

            int fighting = unit.EffectiveCombatStrength;
            if (fighting <= 0)
                return 0;

            float raw = officerStat
                        * (fighting / 100f)
                        * (unit.Morale / 100f)
                        * (unit.Stamina / 100f);
            return Math.Max(0, (int)MathF.Round(raw, MidpointRounding.AwayFromZero));
        }
    }
}
