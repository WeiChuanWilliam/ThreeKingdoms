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

        public IEnumerable<MapLocation> All => _cells.Values;
        public int Count => _cells.Count;

        /// <summary>目前著火的 hex（sparse index）。</summary>
        public IReadOnlyCollection<HexCoord> BurningCells => _burningCells.All;

        public int BurningCellCount => _burningCells.Count;

        public bool IsBurning(HexCoord hex) => _burningCells.Contains(hex);

        public bool TryGet(HexCoord hex, out MapLocation location) => _cells.TryGetValue(hex, out location);

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

        internal void RegisterBurning(HexCoord hex) => _burningCells.Register(hex);

        internal void UnregisterBurning(HexCoord hex) => _burningCells.Unregister(hex);

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
