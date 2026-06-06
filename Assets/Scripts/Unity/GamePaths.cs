using System.IO;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    public static class GamePaths
    {
        /// <summary>StreamingAssets（建置後在 xxx_Data/StreamingAssets）。</summary>
        public static string StreamingAssets => Application.streamingAssetsPath;

        /// <summary>exe 同層 Data/（Mod 用，需手動建立或安裝腳本複製）。</summary>
        public static string ExternalData
        {
            get
            {
                var root = Directory.GetParent(Application.dataPath)?.FullName;
                return string.IsNullOrEmpty(root) ? Application.dataPath : Path.Combine(root, "Data");
            }
        }

        public static string HexMapSampleJson => Path.Combine(StreamingAssets, "hex_map_sample.json");

        public static string ChineseUnitProperties =>
            Path.Combine(StreamingAssets, "chinese", "unit.properties");
    }
}
