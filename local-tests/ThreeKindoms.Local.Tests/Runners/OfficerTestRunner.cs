using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Local.Tests.Runners
{
    public static class OfficerTestRunner
    {
        public static GameTestResult Run(
            string officerPropertiesPath,
            string officersJsonPath,
            string personalityJsonPath,
            string scenarioOfficersPath)
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            if (!OfficerConfigUtil.Load(officerPropertiesPath))
            {
                TestLog.Line(log, "ERROR 無法載入 officer.properties");
                return new GameTestResult("Officers", false, log.ToString(), 0, 1);
            }

            OfficerDatabase.LoadCatalog(officersJsonPath, personalityJsonPath);
            if (OfficerDatabase.Defs.Count == 0)
            {
                TestLog.Line(log, "ERROR officers.json 無資料");
                return new GameTestResult("Officers", false, log.ToString(), 0, 1);
            }

            OfficerDatabase.MaterializeFromScenarioFile(scenarioOfficersPath);
            if (OfficerDatabase.RuntimeCount == 0)
            {
                TestLog.Line(log, "ERROR 劇本武將清單無有效 id");
                return new GameTestResult("Officers", false, log.ToString(), 0, 1);
            }

            OfficerDatabase.SyncAllRelations();

            TestLog.Line(log, "=== officers.json（圖鑑）+ scenario_officers（本劇本 Pool）===");
            TestLog.Line(log, $"圖鑑 {OfficerDatabase.Defs.Count} 名；本劇本 Pool {OfficerDatabase.RuntimeCount} 名（括號內＝表上基礎值，括號外＝發揮值 *Perform）");
            TestLog.Line(log, "");

            var ids = new List<int>(OfficerDatabase.Runtime.Keys);
            ids.Sort();

            foreach (int id in ids)
            {
                Officer o = OfficerDatabase.TryGetRuntime(id);
                if (o == null)
                {
                    TestLog.Line(log, $"ERROR id={id} 無法建立");
                    err++;
                    continue;
                }

                TestLog.Line(log, $"OK  {OfficerDisplay.FormatLine(o)}");
                ok++;
            }

            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("Officers", err == 0, log.ToString(), ok, err);
        }
    }
}
