using System;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊屬性計算：
    /// 1) 主／副將 → 部隊整體五維（統／武／智／政／魅）；
    /// 2) 五維 ＋ 兵力／士氣／體力 → 攻／防／擊破／破城／策略／建設；
    /// 3) 寫入 <see cref="Combat.Stats"/>（<see cref="CombatTroopStatBlock"/>）。
    /// </summary>
    public static class CombatStatMath
    {
        /// <summary>重算並寫入部隊；全滅則清零。</summary>
        public static void Recalculate(Combat combat)
        {
            if (combat == null)
                return;

            if (combat.IsAnnihilated)
            {
                combat.ApplyStats(CombatTroopStatBlock.Zero);
                return;
            }

            combat.ApplyStats(Calculate(combat));
        }

        /// <summary>只計算不寫回。</summary>
        public static CombatTroopStatBlock Calculate(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return CombatTroopStatBlock.Zero;

            return new CombatTroopStatBlock(
                CalculateAttack(combat),
                CalculateDefense(combat),
                CalculateJipo(combat),
                CalculateGongcheng(combat),
                CalculateStrategy(combat),
                CalculateConstruction(combat));
        }

        // ----- 部隊整體五維（統／武／智／政／魅）-----
        // 公式本體：BlendOfficerStat。要改合成權重／取副將規則，改那個方法即可。
        // Combat.Unit* 只是轉呼叫，不算另一套公式。

        /// <summary>統率。公式見 <see cref="BlendOfficerStat"/>。</summary>
        public static short GetUnitLeadership(Unit unit) =>
            BlendOfficerStat(unit, o => o.EffectiveLeadership);

        /// <summary>武力。公式見 <see cref="BlendOfficerStat"/>。</summary>
        public static short GetUnitForce(Unit unit) =>
            BlendOfficerStat(unit, o => o.EffectiveAttack);

        /// <summary>智力。公式見 <see cref="BlendOfficerStat"/>。</summary>
        public static short GetUnitIntelligence(Unit unit) =>
            BlendOfficerStat(unit, o => o.EffectiveIntelligence);

        /// <summary>政治。公式見 <see cref="BlendOfficerStat"/>。</summary>
        public static short GetUnitPolicy(Unit unit) =>
            BlendOfficerStat(unit, o => o.EffectivePolicy);

        /// <summary>魅力。公式見 <see cref="BlendOfficerStat"/>。</summary>
        public static short GetUnitCharisma(Unit unit) =>
            BlendOfficerStat(unit, o => o.EffectiveCharisma);

        /// <summary>
        /// 【五維合成公式｜之後改這裡】
        /// 主將權重 2、該屬性最強副將權重 1：結果 = (主將×2 + 最佳副將) / 3。
        /// 僅主將→主將；僅副將→最佳副將；皆無→0。
        /// selector：從武將取出要合成的那一維（例：政治 = o.EffectivePolicy）。
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
        // 最終六項（寫入 Combat.Stats）：各自改下方 Calculate*。
        // -------------------------------------------------------------------------

        /// <summary>攻擊（暫代：武力 × 兵力／士氣／體力）。</summary>
        public static int CalculateAttack(Combat combat) =>
            Placeholder(GetUnitForce(combat), combat);

        /// <summary>防禦（暫代：統率 × 兵力／士氣／體力）。</summary>
        public static int CalculateDefense(Combat combat) =>
            Placeholder(GetUnitLeadership(combat), combat);

        /// <summary>擊破（暫代：武力 × 兵力／士氣／體力）。</summary>
        public static int CalculateJipo(Combat combat) =>
            Placeholder(GetUnitForce(combat), combat);

        /// <summary>破城（暫代：武力 × 兵力／士氣／體力）。</summary>
        public static int CalculateGongcheng(Combat combat) =>
            Placeholder(GetUnitForce(combat), combat);

        /// <summary>策略（暫代：智力 × 兵力／士氣／體力）。</summary>
        public static int CalculateStrategy(Combat combat) =>
            Placeholder(GetUnitIntelligence(combat), combat);

        /// <summary>
        /// 【建設公式｜之後改這裡】
        /// 暫代：政治(GetUnitPolicy／BlendOfficerStat) × 健康兵力/100 × 士氣/100 × 體力/100。
        /// 與五維合成無關；要換基數或算法直接改本方法，勿改 UnitPolicy 名稱含義。
        /// </summary>
        public static int CalculateConstruction(Combat combat) =>
            Placeholder(GetUnitPolicy(combat), combat);

        /// <summary>
        /// 【暫代共同倍率｜之後可刪】屬性 × 健康兵力/100 × 士氣/100 × 體力/100。
        /// 各 Calculate* 正式公式寫好後可不再呼叫。
        /// </summary>
        static int Placeholder(int officerStat, Combat combat)
        {
            if (combat == null || officerStat <= 0)
                return 0;

            int fighting = combat.EffectiveCombatStrength;
            if (fighting <= 0)
                return 0;

            float raw = officerStat
                        * (fighting / 100f)
                        * (combat.Morale / 100f)
                        * (combat.Stamina / 100f);
            return Math.Max(0, (int)MathF.Round(raw, MidpointRounding.AwayFromZero));
        }
    }
}
