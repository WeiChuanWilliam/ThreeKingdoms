using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    public sealed class HexGrid
    {
        readonly Dictionary<HexCoord, CellData> _cells = new();

        public bool TryGet(HexCoord c, out CellData cell) => _cells.TryGetValue(c, out cell);

        public void Set(HexCoord c, CellData cell) => _cells[c] = cell;

        public bool Contains(HexCoord c) => _cells.ContainsKey(c);

        public IEnumerable<HexCoord> AllCoords => _cells.Keys;

        public int Count => _cells.Count;

        public void Clear() => _cells.Clear();

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
