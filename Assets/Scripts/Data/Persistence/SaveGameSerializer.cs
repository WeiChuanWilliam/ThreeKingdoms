using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Data.Persistence
{
    /// <summary>
    /// 存檔序列化／還原。
    /// <para>SKELETON：僅保留方法簽名；存檔格式待與劇本 Pool 一併定稿後實作。</para>
    /// </summary>
    public static class SaveGameSerializer
    {
        /// <summary>擷取目前地圖部隊與元資料為存檔文件。</summary>
        public static GameSaveDocument Capture(
            IEnumerable<(Unit unit, HexCoord hex)> unitsOnMap,
            string sourceScenarioId = "",
            int gameDay = 0) => new GameSaveDocument();

        /// <summary>存檔轉 JSON 字串。</summary>
        public static string ToJson(GameSaveDocument doc, bool prettyPrint = true) => "{}";

        /// <summary>JSON 字串還原存檔文件。</summary>
        public static GameSaveDocument FromJson(string json) => new GameSaveDocument();

        /// <summary>寫入存檔檔案。</summary>
        public static void WriteFile(GameSaveDocument doc, string absolutePath) { }

        /// <summary>讀取存檔檔案。</summary>
        public static GameSaveDocument ReadFile(string absolutePath) => new GameSaveDocument();

        /// <summary>從存檔還原部隊與座標。</summary>
        public static List<(Unit unit, HexCoord hex)> RestoreUnits(GameSaveDocument doc) => new List<(Unit, HexCoord)>();
    }
}
