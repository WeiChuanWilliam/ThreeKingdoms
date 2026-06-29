using System;
using System.Collections.Generic;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>戰鬥力計算輸入（武將能力、技能組、士氣、體力、部隊六圍與兵力）。</summary>
    public readonly struct CombatPowerContext
    {
        /// <summary>主將戰鬥相關能力。</summary>
        public OfficerCombatAbilities CommanderAbilities { get; }

        /// <summary>副將戰鬥相關能力。</summary>
        public OfficerCombatAbilities ViceAbilities { get; }

        /// <summary>主副將加權合併後的能力。</summary>
        public OfficerCombatAbilities BlendedOfficerAbilities { get; }

        /// <summary>主將與副將合併後的技能 id。</summary>
        public IReadOnlyCollection<int> OfficerSkillIds { get; }

        /// <summary>四槽已裝備戰法總數。</summary>
        public int EquippedSkillCount { get; }

        /// <summary>當前士氣。</summary>
        public short Morale { get; }

        /// <summary>當前體力。</summary>
        public short Stamina { get; }

        /// <summary>含加成後的最終六圍。</summary>
        public CombatTroopStatBlock EffectiveTroopStats { get; }

        /// <summary>現役兵力。</summary>
        public int Soldiers { get; }

        /// <summary>傷兵數。</summary>
        public int Wounded { get; }

        /// <summary>有效戰鬥人力（含傷兵折半）。</summary>
        public int EffectiveManpower { get; }

        /// <summary>從戰鬥部隊快照建立戰鬥力計算輸入。</summary>
        internal CombatPowerContext(
            OfficerCombatAbilities commander,
            OfficerCombatAbilities vice,
            OfficerCombatAbilities blended,
            IReadOnlyCollection<int> officerSkillIds,
            int equippedSkillCount,
            short morale,
            short stamina,
            CombatTroopStatBlock effectiveTroopStats,
            int soldiers,
            int wounded,
            int effectiveManpower)
        {
            CommanderAbilities = commander;
            ViceAbilities = vice;
            BlendedOfficerAbilities = blended;
            OfficerSkillIds = officerSkillIds ?? Array.Empty<int>();
            EquippedSkillCount = equippedSkillCount < 0 ? 0 : equippedSkillCount;
            Morale = morale;
            Stamina = stamina;
            EffectiveTroopStats = effectiveTroopStats;
            Soldiers = soldiers;
            Wounded = wounded;
            EffectiveManpower = effectiveManpower;
        }
    }

    /// <summary>戰鬥力結算結果。</summary>
    public readonly struct CombatPowerResult
    {
        /// <summary>戰鬥力評分。</summary>
        public int Rating { get; }

        /// <summary>建立戰鬥力結算結果（負值歸零）。</summary>
        public CombatPowerResult(int rating)
        {
            Rating = rating < 0 ? 0 : rating;
        }
    }

    /// <summary>戰鬥部隊戰鬥力：武將能力 × 技能組 × 士氣 × 體力 × 六圍 × 有效兵力。</summary>
    public static class CombatPowerRules
    {
        /// <summary>從戰鬥部隊組裝戰鬥力計算上下文。</summary>
        public static bool TryCreateContext(Combat combat, out CombatPowerContext context)
        {
            context = default;
            if (combat == null || combat.IsAnnihilated)
                return false;

            Officer cmd = combat.Commander;
            Officer vice = combat.ViceOfficer;
            var commanderAbilities = OfficerCombatAbilities.FromOfficer(cmd);
            var viceAbilities = OfficerCombatAbilities.FromOfficer(vice);
            var blended = OfficerCombatAbilities.BlendCommanderAndVice(cmd, vice);

            context = new CombatPowerContext(
                commanderAbilities,
                viceAbilities,
                blended,
                combat.OfficerSkillIds,
                combat.CountEquippedSkills(),
                combat.Morale,
                combat.Stamina,
                combat.EffectiveTroopStats,
                combat.Soldiers,
                combat.Wounded,
                combat.EffectiveCombatStrength);
            return true;
        }

        /// <summary>依上下文公式計算戰鬥力評分。</summary>
        public static CombatPowerResult CalculateCombatPower(in CombatPowerContext context)
        {
            if (context.EffectiveManpower <= 0)
                return new CombatPowerResult(0);

            float troopCore = context.EffectiveTroopStats.Attack
                              + context.EffectiveTroopStats.Defense
                              + context.EffectiveTroopStats.Mobility;

            float officerCore = context.BlendedOfficerAbilities.SumCombatRelevant();
            float officerFactor = 1f + officerCore * UnitConfigUtil.GetCombatPowerOfficerScale();

            int skillCount = context.OfficerSkillIds.Count + context.EquippedSkillCount;
            float skillFactor = 1f + skillCount * UnitConfigUtil.GetCombatPowerSkillBonusPerSkill();

            float moraleFactor = context.Morale / 100f * UnitConfigUtil.GetCombatPowerMoraleWeight();
            float staminaFactor = context.Stamina / 100f * UnitConfigUtil.GetCombatPowerStaminaWeight();

            float raw = troopCore
                        * officerFactor
                        * skillFactor
                        * moraleFactor
                        * staminaFactor
                        * context.EffectiveManpower
                        / UnitConfigUtil.GetCombatPowerManpowerDivisor();

            int rating = Math.Max(0, (int)MathF.Round(raw, MidpointRounding.AwayFromZero));
            return new CombatPowerResult(rating);
        }

        /// <summary>取得戰鬥部隊的戰鬥力評分。</summary>
        public static int GetCombatPower(Combat combat)
        {
            if (!TryCreateContext(combat, out CombatPowerContext context))
                return 0;
            return CalculateCombatPower(context).Rating;
        }
    }
}
