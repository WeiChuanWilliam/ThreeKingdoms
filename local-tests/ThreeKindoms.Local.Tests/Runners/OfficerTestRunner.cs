using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Local.Tests.Runners
{
    public static class OfficerTestRunner
    {
        public static GameTestResult Run(string officerPropertiesPath, string officersJsonPath, string personalityJsonPath)
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            if (!OfficerConfigUtil.Load(officerPropertiesPath))
            {
                TestLog.Line(log, "ERROR 無法載入 officer.properties");
                return new GameTestResult("Officers", false, log.ToString(), 0, 1);
            }

            OfficerDatabase db = OfficerDatabase.LoadFromFile(officersJsonPath, personalityJsonPath);
            if (db.Defs.Count == 0)
            {
                TestLog.Line(log, "ERROR officers.json 無資料");
                return new GameTestResult("Officers", false, log.ToString(), 0, 1);
            }

            db.MaterializeAllRuntimes();
            db.SyncAllRelations();
            OfficerPool.Initialize(db);

            TestLog.Line(log, "=== officers.json + officer.properties ===");
            TestLog.Line(log, $"載入 {db.Defs.Count} 名武將（括號內＝表上基礎值，括號外＝發揮值 *Perform）");
            TestLog.Line(log, "");

            var ids = new List<int>(db.Defs.Keys);
            ids.Sort();

            foreach (int id in ids)
            {
                Officer o = db.GetOrCreateRuntime(id);
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
