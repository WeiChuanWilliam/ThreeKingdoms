using System.Collections.Generic;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core
{
    /// <summary>Dijkstra：邊權 = 進入目標格的 enterCost。可限制最大消耗（今日剩餘行動力）。</summary>
    public static class HexPathfinder
    {
        public static PathfindingResult FindPath(
            HexGrid grid,
            HexCoord start,
            HexCoord goal,
            int maxCost = int.MaxValue)
        {
            if (!grid.Contains(start))
                return PathfindingResult.Fail("起點不在地圖上");
            if (!grid.Contains(goal))
                return PathfindingResult.Fail("目標不在地圖上");

            int goalCost = grid.GetEnterCost(goal);
            if (goalCost == int.MaxValue)
                return PathfindingResult.Fail("目標不可進入");

            var dist = new Dictionary<HexCoord, int> { [start] = 0 };
            var prev = new Dictionary<HexCoord, HexCoord>();
            var open = new List<HexCoord> { start };

            while (open.Count > 0)
            {
                int bestIdx = 0;
                int bestDist = dist[open[0]];
                for (int i = 1; i < open.Count; i++)
                {
                    int d = dist[open[i]];
                    if (d < bestDist)
                    {
                        bestDist = d;
                        bestIdx = i;
                    }
                }

                var current = open[bestIdx];
                open.RemoveAt(bestIdx);

                if (current.Equals(goal))
                    return BuildPath(start, goal, prev, bestDist);

                if (bestDist > maxCost)
                    continue;

                foreach (var next in HexMath.GetNeighbors(current))
                {
                    if (!grid.Contains(next))
                        continue;

                    int enter = grid.GetEnterCost(next);
                    if (enter == int.MaxValue)
                        continue;

                    int nextDist = bestDist + enter;
                    if (nextDist > maxCost)
                        continue;

                    if (dist.TryGetValue(next, out int old) && old <= nextDist)
                        continue;

                    dist[next] = nextDist;
                    prev[next] = current;
                    if (!open.Contains(next))
                        open.Add(next);
                }
            }

            return PathfindingResult.Fail("無法到達目標（行動力或地形限制）");
        }

        /// <summary>沿最短路徑移動，直到行動力不足；回傳實際走過的格（含終點）。</summary>
        /// <summary>僅更新座標與行動力；若要連動 Location 請用 <see cref="UnitLocationBinding.MoveAlongPath"/>。</summary>
        public static PathfindingResult MoveAlongPath(
            HexGrid grid,
            IMapUnitMovement unit,
            HexCoord goal)
        {
            var result = FindPath(grid, unit.Position, goal, unit.MovementPointsLeft);
            if (!result.Success)
                return result;

            var walked = new List<HexCoord> { unit.Position };
            int spent = 0;

            for (int i = 1; i < result.Path.Count; i++)
            {
                var step = result.Path[i];
                int cost = grid.GetEnterCost(step);
                if (unit.MovementPointsLeft < cost)
                    break;

                unit.MovementPointsLeft -= cost;
                spent += cost;
                unit.Position = step;
                walked.Add(step);

                if (step.Equals(goal))
                    return PathfindingResult.Ok(walked, spent);
            }

            if (walked.Count <= 1)
                return PathfindingResult.Fail("行動力不足以離開起點");

            return PathfindingResult.Ok(walked, spent);
        }

        static PathfindingResult BuildPath(
            HexCoord start,
            HexCoord goal,
            Dictionary<HexCoord, HexCoord> prev,
            int totalCost)
        {
            var path = new List<HexCoord>();
            var c = goal;
            path.Add(c);
            while (!c.Equals(start))
            {
                if (!prev.TryGetValue(c, out var p))
                    return PathfindingResult.Fail("路徑重建失敗");
                c = p;
                path.Add(c);
            }
            path.Reverse();
            return PathfindingResult.Ok(path, totalCost);
        }
    }
}
