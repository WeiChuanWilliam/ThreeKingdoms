using System;
using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core.Locations
{
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

        /// <summary>日出：場上部隊從腳下格／地形同步著火；若仍著火則跑部隊公式。</summary>
        public static void TickAllUnitsFireAtSunrise(this LocationGrid grid)
        {
            if (grid == null)
                return;

            var processed = new HashSet<Unit>();
            foreach (MapLocation loc in grid.All)
            {
                Unit unit = loc.OccupyingUnit;
                if (unit == null || !processed.Add(unit))
                    continue;
                unit.Location?.TickFireAtSunrise();
            }
        }
    }
}
