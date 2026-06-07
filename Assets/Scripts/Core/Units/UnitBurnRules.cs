using System;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>部隊燃燒判定上下文（通過 <see cref="UnitBurnRules.TryCreateBurnContext"/> 建立）。</summary>
    public readonly struct UnitBurnContext
    {
        public Unit Unit { get; }
        public int TerrainN { get; }
        public HazardDamageLevel FlameLevel { get; }
        public short FireEffect { get; }
        public int Soldiers { get; }
        public int Wounded { get; }
        public float FireDamageFactor { get; }

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

    /// <summary>燃燒傷害結算結果（<see cref="UnitBurnRules.CalculateBurnDamage"/> 產出）。</summary>
    public readonly struct UnitBurnDamageResult
    {
        public static UnitBurnDamageResult None => default;

        public int SoldierDeaths { get; }
        public int WoundedIncrease { get; }
        public short MoraleLoss { get; }

        public bool HasEffect => SoldierDeaths > 0 || WoundedIncrease > 0 || MoraleLoss > 0;

        public UnitBurnDamageResult(int soldierDeaths, int woundedIncrease, short moraleLoss)
        {
            SoldierDeaths = soldierDeaths < 0 ? 0 : soldierDeaths;
            WoundedIncrease = woundedIncrease < 0 ? 0 : woundedIncrease;
            MoraleLoss = moraleLoss < 0 ? (short)0 : moraleLoss;
        }
    }

    /// <summary>所有部隊：是否燃燒 → 燃燒傷害計算與套用。</summary>
    public static class UnitBurnRules
    {
        public const string TraitTengjia = "藤甲";
        public const string TraitHuogong = "火攻";

        /// <summary>腳下格著火即視為燃燒中。</summary>
        public static bool IsBurning(Unit unit) =>
            unit != null && unit.IsOnFire;

        /// <summary>火神：免疫火傷害（含燃燒傷害）。</summary>
        public static bool IsImmuneToBurnDamage(Unit unit) =>
            unit != null &&
            LocationFireRules.UnitHasPersonalityTrait(unit, LocationFireRules.TraitHuoshen);

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

        public static UnitBurnDamageResult CalculateBurnDamage(in UnitBurnContext context)
        {
            if (context.Unit == null || context.Soldiers <= 0)
                return UnitBurnDamageResult.None;

            float levelMult = GetFlameLevelMultiplier(context.FlameLevel);
            if (levelMult <= 0f)
                return UnitBurnDamageResult.None;

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
                moraleLoss = (short)Math.Clamp((int)MathF.Round(baseMorale * levelMult), 0, 100);
            }

            if (deaths == 0 && moraleLoss == 0)
                return UnitBurnDamageResult.None;

            return new UnitBurnDamageResult(deaths, woundedIncrease: 0, moraleLoss);
        }

        public static void ApplyBurnDamageResult(Unit unit, in UnitBurnDamageResult result)
        {
            if (unit == null || !result.HasEffect)
                return;

            int newSoldiers = Math.Max(0, unit.Soldiers - result.SoldierDeaths);
            int newWounded = Math.Min(unit.Wounded + result.WoundedIncrease, newSoldiers);
            unit.SetManpower(newSoldiers, newWounded);

            if (result.MoraleLoss > 0)
                unit.ChangeMorale((short)-result.MoraleLoss);
        }

        public static bool TryApplyDailyBurnDamage(Unit unit, MapLocation location)
        {
            if (!TryCreateBurnContext(unit, location, out UnitBurnContext context))
                return false;

            UnitBurnDamageResult result = CalculateBurnDamage(context);
            ApplyBurnDamageResult(unit, result);
            return result.HasEffect;
        }

        static float GetFlameLevelMultiplier(HazardDamageLevel level) => level switch
        {
            HazardDamageLevel.Serious => 1.5f,
            HazardDamageLevel.Medium => 1f,
            HazardDamageLevel.Slight => 0.5f,
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
