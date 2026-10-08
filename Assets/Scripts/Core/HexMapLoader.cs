using System;
using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    /// <summary>六角地圖 JSON 反序列化根結構。</summary>
    [Serializable]
    public class HexMapJson
    {
        public int version = 1;
        public int width = 16;
        public int height = 16;
        public HexCellJson[] cells;
    }

    /// <summary>單一六角格的 JSON 資料。</summary>
    [Serializable]
    public class HexCellJson
    {
        public int q;
        public int r;
        public string terrain = "plain";
        public int countyId;
        public int ownerId;
        public bool passable = true;
    }

    /// <summary>將 JSON 地圖描述轉為執行時 <see cref="HexGrid"/>。</summary>
    public static class HexMapLoader
    {
        /// <summary>解析 <see cref="HexMapJson"/> 並建立含預設平原的矩形地圖，再套用 cells 覆寫。</summary>
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
                grid.Set(coord, new CellData
                {
                    Terrain = ParseTerrain(c.terrain),
                    CountyId = (ushort)c.countyId,
                    OwnerId = (byte)c.ownerId,
                    Passable = c.passable
                });
            }

            return grid;
        }

        /// <summary>接受 <see cref="TerrainType"/> 名稱（不分大小寫）或數值 id；無法辨識時視為平地。</summary>
        static TerrainType ParseTerrain(string s) =>
            Enum.TryParse(s, ignoreCase: true, out TerrainType t) && Enum.IsDefined(typeof(TerrainType), t)
                ? t
                : TerrainType.Plain;
    }
}
