using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>兵團／運輸隊等非戰鬥部隊的戰鬥力估算（公式待平衡）。</summary>
    public static class UnitCombatPowerRules
    {
        /// <summary>野戰中的兵團：僅行政與輜重人員，戰鬥力極低。</summary>
        public static int CalculateFieldLegionPower(Legion legion)
        {
            if (legion == null || legion.IsAnnihilated)
                return 0;

            float manpowerFactor = legion.EffectiveCombatStrength
                                   / UnitConfigUtil.GetCombatPowerManpowerDivisor();
            float moraleFactor = legion.Morale / 100f;
            float fieldScale = UnitConfigUtil.GetFloat("unit.combat_power.legion_field_scale", 0.15f);

            Officer cmd = legion.Commander;
            float officerBonus = cmd != null
                ? OfficerCombatAbilities.FromOfficer(cmd).SumCombatRelevant() * 0.01f
                : 0f;

            int rating = (int)System.MathF.Round(
                legion.EffectiveCombatStrength * fieldScale * moraleFactor * (1f + officerBonus),
                System.MidpointRounding.AwayFromZero);
            return System.Math.Max(0, rating);
        }

        /// <summary>駐紮據點的兵團：以駐防姿態作戰，戰鬥力接近戰鬥部隊量級。</summary>
        public static int CalculateStationedLegionPower(Legion legion)
        {
            if (legion == null || legion.IsAnnihilated)
                return 0;

            Officer cmd = legion.Commander;
            var abilities = OfficerCombatAbilities.FromOfficer(cmd);
            float officerCore = abilities.SumCombatRelevant();
            float officerFactor = 1f + officerCore * UnitConfigUtil.GetCombatPowerOfficerScale();

            float moraleFactor = legion.Morale / 100f * UnitConfigUtil.GetCombatPowerMoraleWeight();
            float staminaFactor = legion.Stamina / 100f * UnitConfigUtil.GetCombatPowerStaminaWeight();
            float stationedScale = UnitConfigUtil.GetFloat("unit.combat_power.legion_stationed_scale", 1.2f);

            float raw = (legion.EffectiveCombatStrength * stationedScale)
                        * officerFactor
                        * moraleFactor
                        * staminaFactor
                        / UnitConfigUtil.GetCombatPowerManpowerDivisor();

            return System.Math.Max(0, (int)System.MathF.Round(raw, System.MidpointRounding.AwayFromZero));
        }

        /// <summary>運輸隊：幾乎無作戰能力。</summary>
        public static int CalculateTransportPower(Transport transport)
        {
            if (transport == null || transport.IsAnnihilated)
                return 0;

            float scale = UnitConfigUtil.GetFloat("unit.combat_power.transport_scale", 0.05f);
            return System.Math.Max(0, (int)(transport.EffectiveCombatStrength * scale));
        }
    }
}
