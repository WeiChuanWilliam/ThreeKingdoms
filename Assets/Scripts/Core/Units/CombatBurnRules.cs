using ThreeKindoms.Core.Locations;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>戰鬥部隊燃燒判定上下文（通過 <see cref="CombatBurnRules.TryCreateBurnContext"/> 建立）。</summary>
    public readonly struct CombatBurnContext
    {
        public Combat Unit { get; }
        public int TerrainN { get; }
        public HazardDamageLevel FlameLevel { get; }
        public short FireEffect { get; }
        public int Soldiers { get; }
        public int Wounded { get; }

        internal CombatBurnContext(Combat unit, MapLocation location, int terrainN)
        {
            Unit = unit;
            TerrainN = terrainN;
            FlameLevel = unit.FlameDamage;
            FireEffect = unit.FireEffect;
            Soldiers = unit.Soldiers;
            Wounded = unit.Wounded;
        }
    }

    /// <summary>燃燒傷害結算結果（<see cref="CombatBurnRules.CalculateBurnDamage"/> 產出）。</summary>
    public readonly struct CombatBurnDamageResult
    {
        public static CombatBurnDamageResult None => default;

        public int SoldierDeaths { get; }
        public int WoundedIncrease { get; }
        public short MoraleLoss { get; }

        public bool HasEffect => SoldierDeaths > 0 || WoundedIncrease > 0 || MoraleLoss > 0;

        public CombatBurnDamageResult(int soldierDeaths, int woundedIncrease, short moraleLoss)
        {
            SoldierDeaths = soldierDeaths < 0 ? 0 : soldierDeaths;
            WoundedIncrease = woundedIncrease < 0 ? 0 : woundedIncrease;
            MoraleLoss = moraleLoss < 0 ? (short)0 : moraleLoss;
        }
    }

    /// <summary>戰鬥部隊：是否燃燒 → 燃燒傷害計算（公式待接）。</summary>
    public static class CombatBurnRules
    {
        /// <summary>腳下格著火即視為燃燒中。</summary>
        public static bool IsBurning(Combat combat) =>
            combat != null && combat.IsOnFire;

        /// <summary>火神：免疫火傷害（含燃燒傷害）。</summary>
        public static bool IsImmuneToBurnDamage(Combat combat) =>
            combat != null &&
            LocationFireRules.UnitHasPersonalityTrait(combat, LocationFireRules.TraitHuoshen);

        /// <summary>
        /// 建立燃燒上下文：先確認燃燒、非免疫、地形 n≠0。
        /// </summary>
        public static bool TryCreateBurnContext(
            Combat combat,
            MapLocation location,
            out CombatBurnContext context)
        {
            context = default;
            if (combat == null || location == null)
                return false;
            if (!IsBurning(combat))
                return false;
            if (!location.LocationFlags.OnFire)
                return false;
            if (IsImmuneToBurnDamage(combat))
                return false;

            int n = LocationFireRules.GetTerrainN(location.Terrain);
            if (n == 0)
                return false;

            context = new CombatBurnContext(combat, location, n);
            return true;
        }

        /// <summary>燃燒傷害數值公式（待接：n、FlameLevel、兵種、藤甲等）。</summary>
        public static CombatBurnDamageResult CalculateBurnDamage(in CombatBurnContext context)
        {
            // TODO: 依 context.TerrainN、context.FlameLevel、context.FireEffect 等計算
            return CombatBurnDamageResult.None;
        }

        /// <summary>將 <see cref="CombatBurnDamageResult"/> 寫回部隊（待接）。</summary>
        public static void ApplyBurnDamageResult(Combat combat, in CombatBurnDamageResult result)
        {
            if (combat == null || !result.HasEffect)
                return;

            // TODO: 傷亡、士氣等
            // combat.SetManpower(...);
            // combat.ChangeMorale(...);
        }

        /// <summary>日出／每日：判定燃燒 → 計算 → 套用。</summary>
        public static bool TryApplyDailyBurnDamage(Combat combat, MapLocation location)
        {
            if (!TryCreateBurnContext(combat, location, out CombatBurnContext context))
                return false;

            CombatBurnDamageResult result = CalculateBurnDamage(context);
            ApplyBurnDamageResult(combat, result);
            return true;
        }
    }
}
