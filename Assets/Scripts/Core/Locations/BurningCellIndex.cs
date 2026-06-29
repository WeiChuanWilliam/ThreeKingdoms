using System.Collections.Generic;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>著火格的 sparse index（reverse index）；O(1) 查詢、日出只遍歷此集合。</summary>
    public sealed class BurningCellIndex
    {
        readonly HashSet<HexCoord> _cells = new();

        /// <summary>已登記著火格數量。</summary>
        public int Count => _cells.Count;

        /// <summary>所有著火 hex 座標（唯讀）。</summary>
        public IReadOnlyCollection<HexCoord> All => _cells;

        /// <summary>查詢 hex 是否在著火索引中。</summary>
        public bool Contains(HexCoord hex) => _cells.Contains(hex);

        /// <summary>格子起火；已存在則 false。</summary>
        public bool Register(HexCoord hex) => _cells.Add(hex);

        /// <summary>格子熄滅；不存在則 false。</summary>
        public bool Unregister(HexCoord hex) => _cells.Remove(hex);

        /// <summary>清空著火索引。</summary>
        public void Clear() => _cells.Clear();
    }
}
