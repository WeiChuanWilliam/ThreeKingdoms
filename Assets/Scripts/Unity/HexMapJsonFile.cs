using System.IO;
using ThreeKindoms.Core;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    public static class HexMapJsonFile
    {
        public static HexGrid LoadOrCreateDefault()
        {
            string path = GamePaths.HexMapSampleJson;
            if (!File.Exists(path))
            {
                Debug.LogWarning($"找不到 {path}，使用程式產生的 16×16 平原。");
                return HexGrid.CreateRectangle(16, 16, _ => CellData.Plain());
            }

            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<HexMapJson>(json);
            var grid = HexMapLoader.FromJson(data);
            Debug.Log($"已載入地圖：{grid.Count} 格，來源 {path}");
            return grid;
        }
    }
}
