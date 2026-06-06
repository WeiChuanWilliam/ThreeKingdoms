using System;
using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    [Serializable]
    public class HexMapJson
    {
        public int version = 1;
        public int width = 16;
        public int height = 16;
        public HexCellJson[] cells;
    }

    [Serializable]
    public class HexCellJson
    {
        public int q;
        public int r;
        public string terrain = "plain";
        public int enterCost = -1;
        public int countyId;
        public int ownerId;
        public bool passable = true;
    }

    public static class HexMapLoader
    {
        public static HexGrid FromJson(HexMapJson data)
        {
            int w = data?.width ?? 16;
            int h = data?.height ?? 16;
            var grid = HexGrid.CreateRectangle(w, h, _ => CellData.Plain());

            if (data?.cells == null || data.cells.Length == 0)
                return grid;

            foreach (var c in data.cells)
            {
                var coord = new HexCoord(c.q, c.r);
                var terrain = ParseTerrain(c.terrain);
                int cost = c.enterCost >= 0 ? c.enterCost : MovementRules.GetDefaultEnterCost(terrain);
                grid.Set(coord, new CellData
                {
                    Terrain = terrain,
                    EnterCost = (byte)Math.Min(cost, 255),
                    CountyId = (ushort)c.countyId,
                    OwnerId = (byte)c.ownerId,
                    Passable = c.passable
                });
            }

            return grid;
        }

        static TerrainType ParseTerrain(string s) => s?.ToLowerInvariant() switch
        {
            "road" => TerrainType.Road,
            "hill" => TerrainType.Hill,
            "mountain" => TerrainType.Mountain,
            "water" => TerrainType.Water,
            "forest" => TerrainType.Forest,
            _ => TerrainType.Plain
        };
    }
}
