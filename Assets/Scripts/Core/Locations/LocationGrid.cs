using System;
using System.Collections.Generic;
using ThreeKindoms.Core.Terrain;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>整張地圖的 Location 索引（hex → MapLocation）。</summary>
    public sealed class LocationGrid
    {
        readonly Dictionary<HexCoord, MapLocation> _cells = new();
        readonly BurningCellIndex _burningCells = new();

        /// <summary>地圖上所有 Location 格。</summary>
        public IEnumerable<MapLocation> All => _cells.Values;

        /// <summary>已建立 Location 格數。</summary>
        public int Count => _cells.Count;

        /// <summary>目前著火的 hex（sparse index）。</summary>
        public IReadOnlyCollection<HexCoord> BurningCells => _burningCells.All;

        /// <summary>目前著火格數量。</summary>
        public int BurningCellCount => _burningCells.Count;

        /// <summary>指定 hex 是否正在燃燒。</summary>
        public bool IsBurning(HexCoord hex) => _burningCells.Contains(hex);

        /// <summary>依座標查詢 Location；不存在則失敗。</summary>
        public bool TryGet(HexCoord hex, out MapLocation location) => _cells.TryGetValue(hex, out location);

        /// <summary>取得或新建指定 hex 的 Location 並綁定地形。</summary>
        public MapLocation GetOrCreate(HexCoord hex, AbstractTerrain terrain)
        {
            if (_cells.TryGetValue(hex, out var existing))
                return existing;

            var loc = new MapLocation(hex, terrain);
            loc.AttachGrid(this);
            _cells[hex] = loc;
            return loc;
        }

        /// <summary>對已有格同步 index（測試／讀檔後若直接改 flags 時用）。</summary>
        public void SyncBurningIndex(MapLocation location)
        {
            if (location == null)
                return;
            if (location.LocationFlags.OnFire)
                RegisterBurning(location.Hex);
            else
                UnregisterBurning(location.Hex);
        }

        /// <summary>將 hex 登記為著火格（內部索引維護）。</summary>
        internal void RegisterBurning(HexCoord hex) => _burningCells.Register(hex);

        /// <summary>將 hex 自著火索引移除。</summary>
        internal void UnregisterBurning(HexCoord hex) => _burningCells.Unregister(hex);

        /// <summary>依矩形範圍與地形工廠建立整張 Location 網格。</summary>
        public static LocationGrid FromTerrainRectangle(int width, int height, Func<HexCoord, AbstractTerrain> terrainFactory)
        {
            var grid = new LocationGrid();
            for (int r = 0; r < height; r++)
            for (int q = 0; q < width; q++)
            {
                var hex = new HexCoord(q, r);
                grid.GetOrCreate(hex, terrainFactory(hex));
            }
            return grid;
        }
    }
}
