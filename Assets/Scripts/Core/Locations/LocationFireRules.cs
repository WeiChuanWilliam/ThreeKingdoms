using System;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>
    /// 格子著火／續燃／蔓延。地形參數 n 暫用 <see cref="AbstractTerrain.FireEffect"/>（專用欄位待定）。
    /// </summary>
    public static class LocationFireRules
    {
        public const string TraitHuoshen = "火神";
        public const string TraitMiehuo = "滅火";

        /// <summary>地形參數 n（專用欄位未定；暫用 fireEffect）。</summary>
        public static int GetTerrainN(AbstractTerrain terrain) =>
            terrain?.FireEffect ?? 0;

        /// <summary>
        /// 著火率（%）：<c>(n != 0 &amp;&amp; 通過部隊閘) ? 100 : 0</c>。
        /// 部隊閘：無部隊，或部隊主／副將皆無 <see cref="TraitHuoshen"/>。
        /// </summary>
        public static int ResolveIgnitionChancePercent(int terrainN, Unit occupyingUnit = null)
        {
            if (terrainN == 0)
                return 0;
            if (!PassesIgnitionUnitGate(occupyingUnit))
                return 0;
            return 100;
        }

        /// <summary>著火率 &gt; 0 才可能著火。</summary>
        public static bool CanIgnite(int terrainN, Unit occupyingUnit = null) =>
            ResolveIgnitionChancePercent(terrainN, occupyingUnit) > 0;

        /// <summary>
        /// 每日續燃機率（%）：<c>n != 0 &amp;&amp; 通過部隊閘</c> 時為 <c>n×step</c>（step 預設 25），否則 0。
        /// 部隊閘：無部隊，或部隊主／副將皆無 <see cref="TraitMiehuo"/>。
        /// </summary>
        public static int ResolveDailyBurnContinuationChancePercent(int terrainN, Unit occupyingUnit = null)
        {
            if (terrainN == 0 || !PassesBurnContinuationUnitGate(occupyingUnit))
                return 0;
            int step = UnitConfigUtil.GetFireBurnContinuationStepPercent();
            return (int)Math.Clamp((long)terrainN * step, 0, 100);
        }

        /// <summary>日出續燃：<c>(n != 0 &amp;&amp; 通過部隊閘) &amp;&amp; roll &lt; n×25%</c>。</summary>
        public static bool EvaluateDailyBurnContinuation(int terrainN, int roll0To99, Unit occupyingUnit = null)
        {
            if (terrainN == 0 || !PassesBurnContinuationUnitGate(occupyingUnit))
                return false;
            return roll0To99 < ResolveDailyBurnContinuationChancePercent(terrainN, occupyingUnit);
        }

        /// <summary>無部隊，或部隊上沒有火神（主將／副將）。</summary>
        public static bool PassesIgnitionUnitGate(Unit occupyingUnit)
        {
            if (occupyingUnit == null)
                return true;
            return !UnitHasPersonalityTrait(occupyingUnit, TraitHuoshen);
        }

        /// <summary>無部隊，或部隊上沒有滅火（主將／副將）。</summary>
        public static bool PassesBurnContinuationUnitGate(Unit occupyingUnit)
        {
            if (occupyingUnit == null)
                return true;
            return !UnitHasPersonalityTrait(occupyingUnit, TraitMiehuo);
        }

        public static bool UnitHasPersonalityTrait(Unit unit, string traitName)
        {
            if (unit == null || string.IsNullOrEmpty(traitName))
                return false;
            if (OfficerPersonalityUtil.HasTrait(unit.Commander, traitName))
                return true;
            foreach (Officer vice in unit.ViceOfficers)
            {
                if (OfficerPersonalityUtil.HasTrait(vice, traitName))
                    return true;
            }
            return false;
        }

        /// <summary>日出：未通過續燃判定則熄滅。</summary>
        public static void TickDailyBurnAtSunrise(MapLocation location, int roll0To99)
        {
            if (location == null || !location.LocationFlags.OnFire)
                return;

            int n = GetTerrainN(location.Terrain);
            Unit unit = location.OccupyingUnit;
            if (!EvaluateDailyBurnContinuation(n, roll0To99, unit))
                location.Extinguish();
        }

        /// <summary>鄰格火勢蔓延（規則未定；保留呼叫入口，實作先註解）。</summary>
        public static void TrySpreadFireToAdjacentTiles(
            MapLocation source,
            LocationGrid grid,
            Random rng)
        {
            // TODO: 鄰近格蔓延 — 風向、地形 fireable、n 等尚未定案
            // if (source == null || grid == null || rng == null || !source.LocationFlags.OnFire)
            //     return;
            // foreach (HexCoord neighbor in HexTopology.Neighbors(source.Hex))
            // {
            //     if (!grid.TryGet(neighbor, out MapLocation target))
            //         continue;
            //     ...
            // }
        }
    }
}
