using System;
using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>日出時對 <see cref="LocationGrid"/> 執行的火勢與部隊結算擴充方法。</summary>
    public static class LocationGridSunrise
    {
        /// <summary>日出：只遍歷 <see cref="LocationGrid.BurningCells"/> 做續燃與蔓延。</summary>
        public static void TickFireAtSunrise(this LocationGrid grid, Random rng)
        {
            if (grid == null || rng == null)
                return;

            var snapshot = new List<HexCoord>(grid.BurningCells);
            foreach (HexCoord hex in snapshot)
            {
                if (!grid.TryGet(hex, out MapLocation loc))
                {
                    grid.UnregisterBurning(hex);
                    continue;
                }

                if (!loc.LocationFlags.OnFire)
                {
                    grid.UnregisterBurning(hex);
                    continue;
                }

                loc.CountFire(rng.Next(100));
                if (loc.LocationFlags.OnFire)
                    loc.FireExpansion(grid, rng);
            }
        }

        /// <summary>
        /// 日出：掃著火格，反查該格上的部隊與建築並套用灼燒結算。
        /// 火焰傷害由此處觸發，不掃描整張地圖。
        /// </summary>
        public static void ApplyBurnDamageAtSunrise(this LocationGrid grid)
        {
            if (grid == null)
                return;

            var snapshot = new List<HexCoord>(grid.BurningCells);
            foreach (HexCoord hex in snapshot)
            {
                if (!grid.TryGet(hex, out MapLocation loc))
                    continue;
                if (!loc.LocationFlags.OnFire)
                    continue;

                Unit unit = loc.OccupyingUnit;
                if (unit != null && !unit.IsAnnihilated)
                    UnitSunriseFireTick.ApplyDailyFireFormulas(unit, loc);

                AbstractBuilding building = loc.Building;
                if (building != null)
                    BuildingBurnRules.TryApplyDailyBurnDamage(building, loc);
            }
        }

        /// <summary>
        /// 日出：掃 <see cref="UnitRegistry"/>，依各部隊座標同步腳下格狀態並補行動力。
        /// </summary>
        public static void TickAllUnitsAtSunrise(this LocationGrid grid)
        {
            _ = grid;
            foreach (Unit unit in UnitRegistry.All)
            {
                if (unit == null || unit.IsAnnihilated)
                    continue;

                unit.Location.SyncEnvironmentFromLocation();

                if (unit.IsStationed)
                    continue;

                unit.Location.RefillMovementAtSunrise();
            }
        }

        /// <summary>相容舊呼叫；請改用 <see cref="TickAllUnitsAtSunrise"/> + <see cref="ApplyBurnDamageAtSunrise"/>。</summary>
        [Obsolete("改用 TickAllUnitsAtSunrise（同步）與 ApplyBurnDamageAtSunrise（灼燒反查）。")]
        public static void TickAllUnitsFireAtSunrise(this LocationGrid grid)
        {
            grid.TickAllUnitsAtSunrise();
            grid.ApplyBurnDamageAtSunrise();
        }
    }
}
