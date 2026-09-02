using System;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Data.Terrain;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>部隊燃燒判定上下文（通過 <see cref="UnitBurnRules.TryCreateBurnContext"/> 建立）。</summary>
    public readonly struct UnitBurnContext
    {
        /// <summary>受燃燒結算的部隊。</summary>
        public Unit Unit { get; }

        /// <summary>地形火焰強度係數 N。</summary>
        public int TerrainN { get; }

        /// <summary>腳下格火焰傷害等級。</summary>
        public TerrainHazardLevel FlameLevel { get; }

        /// <summary>部隊身上的火焰效果強度。</summary>
        public short FireEffect { get; }

        /// <summary>結算時可戰兵力。</summary>
        public int Soldiers { get; }

        /// <summary>結算時傷兵數。</summary>
        public int Wounded { get; }

        /// <summary>部隊類型火焰傷害係數。</summary>
        public float FireDamageFactor { get; }

        /// <summary>由部隊與腳下格建立燃燒判定快照。</summary>
        internal UnitBurnContext(Unit unit, MapLocation location, int terrainN)
        {
            Unit = unit;
            TerrainN = terrainN;
            FlameLevel = unit.FlameDamage;
            FireEffect = unit.FireEffect;
            Soldiers = unit.Soldiers;
            Wounded = unit.Wounded;
            FireDamageFactor = unit.FireDamageFactor;
        }
    }

    /// <summary>
    /// 部隊火攻：是否燃燒 → 算出 <see cref="BattleDamageResult"/> → 統一 <see cref="BattleDamageResult.Apply"/>。
    /// 陷阱／建築傷害等同理：各自算 Result，再 Apply。
    /// </summary>
    public static class UnitBurnRules
    {
        /// <summary>藤甲個性：受火傷害加劇。</summary>
        public const string TraitTengjia = "藤甲";

        /// <summary>火攻個性：受火傷害減輕。</summary>
        public const string TraitHuogong = "火攻";

        /// <summary>腳下格著火即視為燃燒中。</summary>
        public static bool IsBurning(Unit unit) =>
            unit != null && unit.IsOnFire;

        /// <summary>火神：免疫火傷害（含燃燒傷害）。</summary>
        public static bool IsImmuneToBurnDamage(Unit unit) =>
            unit != null &&
            LocationFireRules.UnitHasPersonalityTrait(unit, LocationFireRules.TraitHuoshen);

        /// <summary>嘗試為著火部隊建立燃燒結算上下文（免疫或格未著火則失敗）。</summary>
        public static bool TryCreateBurnContext(
            Unit unit,
            MapLocation location,
            out UnitBurnContext context)
        {
            context = default;
            if (unit == null || location == null)
                return false;
            if (!IsBurning(unit))
                return false;
            if (!location.LocationFlags.OnFire)
                return false;
            if (IsImmuneToBurnDamage(unit))
                return false;

            int n = LocationFireRules.GetTerrainN(location.Terrain);
            if (n == 0)
                return false;

            context = new UnitBurnContext(unit, location, n);
            return true;
        }

        /// <summary>
        /// 【火攻傷害公式｜之後改這裡】
        /// 依燃燒上下文算出 <see cref="BattleDamageResult"/>（不改 Unit）。
        /// 目前暫代：陣亡＋士氣；傷兵／體力為 0。
        /// </summary>
        public static BattleDamageResult CalculateBurnDamage(in UnitBurnContext context)
        {
            if (context.Unit == null || context.Soldiers <= 0)
                return BattleDamageResult.None;

            float levelMult = GetFlameLevelMultiplier(context.FlameLevel);
            if (levelMult <= 0f)
                return BattleDamageResult.None;

            int basePer1000 = UnitConfigUtil.GetFireBaseDeathsPer1000Soldiers();
            float personalityMult = GetPersonalityFireDamageMultiplier(context.Unit);
            float rawDeaths = context.Soldiers * (basePer1000 / 1000f)
                              * levelMult
                              * context.TerrainN
                              * context.FireDamageFactor
                              * personalityMult;

            int deaths = Math.Max(0, (int)MathF.Round(rawDeaths));
            if (deaths > context.Soldiers)
                deaths = context.Soldiers;

            short moraleLoss = 0;
            if (deaths > 0)
            {
                short baseMorale = UnitConfigUtil.GetFireBaseMoraleLossPerDay();
                moraleLoss = (short)NumericUtil.ClampToTarget((int)MathF.Round(baseMorale * levelMult), 0, 100);
            }

            if (deaths == 0 && moraleLoss == 0)
                return BattleDamageResult.None;

            return new BattleDamageResult(
                soldierDeaths: deaths,
                woundedIncrease: 0,
                moraleLoss: moraleLoss,
                staminaLoss: 0);
        }

        /// <summary>著火格每日結算：算 <see cref="BattleDamageResult"/> 再 <see cref="BattleDamageResult.Apply"/>。</summary>
        public static bool TryApplyDailyBurnDamage(Unit unit, MapLocation location)
        {
            if (!TryCreateBurnContext(unit, location, out UnitBurnContext context))
                return false;

            BattleDamageResult result = CalculateBurnDamage(context);
            BattleDamageResult.Apply(unit, result);
            return result.HasEffect;
        }

        static float GetFlameLevelMultiplier(TerrainHazardLevel level) => level switch
        {
            TerrainHazardLevel.Serious => 1.5f,
            TerrainHazardLevel.Medium => 1f,
            TerrainHazardLevel.Slight => 0.5f,
            _ => 0f
        };

        static float GetPersonalityFireDamageMultiplier(Unit unit)
        {
            if (unit == null)
                return 1f;

            float mult = 1f;
            if (LocationFireRules.UnitHasPersonalityTrait(unit, TraitTengjia))
                mult *= 3f;
            if (LocationFireRules.UnitHasPersonalityTrait(unit, TraitHuogong))
                mult *= 0.7f;
            return mult;
        }
    }
}
