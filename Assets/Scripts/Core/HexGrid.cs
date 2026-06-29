using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    /// <summary>六角地圖格資料容器（座標 → <see cref="CellData"/>）。</summary>
    public sealed class HexGrid
    {
        readonly Dictionary<HexCoord, CellData> _cells = new();

        /// <summary>查詢座標上的格資料。</summary>
        public bool TryGet(HexCoord c, out CellData cell) => _cells.TryGetValue(c, out cell);

        /// <summary>寫入或覆寫座標上的格資料。</summary>
        public void Set(HexCoord c, CellData cell) => _cells[c] = cell;

        /// <summary>地圖是否包含該座標。</summary>
        public bool Contains(HexCoord c) => _cells.ContainsKey(c);

        /// <summary>所有已登記座標。</summary>
        public IEnumerable<HexCoord> AllCoords => _cells.Keys;

        /// <summary>格總數。</summary>
        public int Count => _cells.Count;

        /// <summary>清空地圖。</summary>
        public void Clear() => _cells.Clear();

        /// <summary>取得進入該格的行動力消耗；不可通行則為 int.MaxValue。</summary>
        public int GetEnterCost(HexCoord c)
        {
            if (!_cells.TryGetValue(c, out var cell) || !cell.Passable)
                return int.MaxValue;
            return cell.EnterCost;
        }

        /// <summary>建立矩形區域的 axial 六角格（Spike 用）。</summary>
        public static HexGrid CreateRectangle(int width, int height, System.Func<HexCoord, CellData> factory)
        {
            var grid = new HexGrid();
            for (int r = 0; r < height; r++)
            {
                for (int q = 0; q < width; q++)
                {
                    var c = new HexCoord(q, r);
                    grid.Set(c, factory(c));
                }
            }
            return grid;
        }
    }
}
