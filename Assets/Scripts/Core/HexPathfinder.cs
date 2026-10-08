using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    /// <summary>Dijkstra：邊權 = 離開目前格的地形 MoveCost。可限制最大消耗（今日剩餘行動力）。</summary>
    public static class HexPathfinder
    {
        /// <summary>在消耗上限內以 Dijkstra 尋找起點至目標的最短路徑。</summary>
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
            if (!grid.CanEnter(goal))
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

                int leave = grid.GetLeaveCost(current);
                foreach (var next in HexMath.GetNeighbors(current))
                {
                    if (!grid.CanEnter(next))
                        continue;

                    int nextDist = bestDist + leave;
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
