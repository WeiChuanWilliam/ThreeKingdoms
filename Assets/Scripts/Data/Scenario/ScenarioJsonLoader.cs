using System.Collections.Generic;
using ThreeKindoms.Data.Scenario;

namespace ThreeKindoms.Data.Scenario
{
    /// <summary>
    /// 劇本 JSON 載入與部隊生成入口。
    /// <para>SKELETON：僅保留方法簽名；開局放置待劇本流程定稿後實作。</para>
    /// </summary>
    public static class ScenarioJsonLoader
    {
        /// <summary>讀取劇本 JSON 文件。</summary>
        public static ScenarioDocument LoadDocument(string absolutePath) => new ScenarioDocument();

        /// <summary>讀取劇本並建立開局部隊清單。</summary>
        public static List<ScenarioSpawnedUnit> LoadAndSpawnUnits(string absolutePath) => new List<ScenarioSpawnedUnit>();

        /// <summary>將 JSON 單筆部隊轉為生成規格。</summary>
        public static ScenarioUnitSpawnSpec ToSpec(ScenarioUnitEntry e) => new ScenarioUnitSpawnSpec();
    }
}
