using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    /// <summary>六角格拓撲與距離計算。</summary>
    public static class HexMath
    {
        /// <summary>六個鄰格相對 (dq, dr) 偏移。</summary>
        public static readonly (int dq, int dr)[] NeighborDeltas =
        {
            (1, 0), (1, -1), (0, -1), (-1, 0), (-1, 1), (0, 1)
        };

        /// <summary>列舉指定格的全部六角鄰居。</summary>
        public static IEnumerable<HexCoord> GetNeighbors(HexCoord c)
        {
            foreach (var (dq, dr) in NeighborDeltas)
                yield return new HexCoord(c.Q + dq, c.R + dr);
        }

        /// <summary>六角步數距離（用於 A* 啟發）。</summary>
        public static int Distance(HexCoord a, HexCoord b)
        {
            int dq = a.Q - b.Q;
            int dr = a.R - b.R;
            int ds = a.S - b.S;
            return (Abs(dq) + Abs(dr) + Abs(ds)) / 2;
        }

        /// <summary>啟發式：假設每格至少 cost 1。</summary>
        public static int Heuristic(HexCoord a, HexCoord b, int minCost = 1) =>
            Distance(a, b) * minCost;

        static int Abs(int v) => v < 0 ? -v : v;
    }
}
