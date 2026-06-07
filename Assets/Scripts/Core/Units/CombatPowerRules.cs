using System;
using System.Collections.Generic;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>戰鬥力計算輸入（武將能力、技能組、士氣、體力、部隊六圍與兵力）。</summary>
    public readonly struct CombatPowerContext
    {
        public OfficerCombatAbilities CommanderAbilities { get; }
        public OfficerCombatAbilities ViceAbilities { get; }
        public OfficerCombatAbilities BlendedOfficerAbilities { get; }
        public IReadOnlyCollection<int> OfficerSkillIds { get; }
        public int EquippedSkillCount { get; }
        public short Morale { get; }
        public short Stamina { get; }
        public CombatTroopStatBlock EffectiveTroopStats { get; }
        public int Soldiers { get; }
        public int Wounded { get; }
        public int EffectiveManpower { get; }

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
        public int Rating { get; }

        public CombatPowerResult(int rating)
        {
            Rating = rating < 0 ? 0 : rating;
        }
    }

    /// <summary>戰鬥部隊戰鬥力：武將能力 × 技能組 × 士氣 × 體力 × 六圍 × 有效兵力。</summary>
    public static class CombatPowerRules
    {
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

        public static int GetCombatPower(Combat combat)
        {
            if (!TryCreateContext(combat, out CombatPowerContext context))
                return 0;
            return CalculateCombatPower(context).Rating;
        }
    }
}
