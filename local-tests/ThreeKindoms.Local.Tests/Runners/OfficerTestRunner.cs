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
            string personalityJsonPath)
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            if (!OfficerConfigUtil.Load(officerPropertiesPath))
            {
                TestLog.Line(log, "ERROR 無法載入 officer.properties");
                return new GameTestResult("Officers", false, log.ToString(), 0, 1);
            }

            OfficerDatabase.Load(officersJsonPath, personalityJsonPath);
            if (OfficerDatabase.Count == 0)
            {
                TestLog.Line(log, "ERROR officers.json 無資料");
                return new GameTestResult("Officers", false, log.ToString(), 0, 1);
            }

            TestLog.Line(log, "=== officers.json（本劇本武將池）===");
            TestLog.Line(log, $"表 {OfficerDatabase.DefCount} 名、執行時 {OfficerDatabase.Count} 名（括號內＝Defs 原始六維，括號外＝*Perform）");
            TestLog.Line(log, "");

            var ids = new List<int>(OfficerDatabase.Officers.Keys);
            ids.Sort();

            foreach (int id in ids)
            {
                Officer o = OfficerDatabase.TryGet(id);
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
