using System;
using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Tests.Runners;

namespace ThreeKindoms.Tests
{
    /// <summary>一次跑完所有手動測試；Editor 選單與 Play 測試共用。</summary>
    public static class GameTestSuite
    {
        public static string LastCombinedReport { get; private set; } = "";
        public static int LastExitCode { get; private set; }

        /// <summary>等同 console Main：回傳 0 成功、1 有失敗。</summary>
        public static int RunAll(string unitPropertiesPath, Action<string> writeLine = null)
        {
            var results = new List<GameTestResult>();
            writeLine ??= static _ => { };

            results.Add(UnitPropertiesTestRunner.Run(unitPropertiesPath));
            results.Add(TroopKindRegistryTestRunner.Run());
            results.Add(TroopKindTreeTestRunner.Run());
            results.Add(CombatTroopKindTestRunner.Run(unitPropertiesPath));

            var report = new StringBuilder();
            report.AppendLine("######## ThreeKindoms GameTestSuite ########");
            report.AppendLine($"properties: {unitPropertiesPath}");
            report.AppendLine();

            int totalErr = 0;
            foreach (GameTestResult r in results)
            {
                report.AppendLine($"====== {r.Name} {(r.Passed ? "PASS" : "FAIL")} ======");
                report.Append(r.Report);
                if (!report.ToString().EndsWith("\n", StringComparison.Ordinal))
                    report.AppendLine();
                report.AppendLine();
                totalErr += r.ErrorCount;
                writeLine($"[{r.Name}] {(r.Passed ? "PASS" : "FAIL")} ok={r.OkCount} err={r.ErrorCount}");
            }

            report.AppendLine($"######## 總計 FAIL 項 err≈{totalErr} ########");
            LastCombinedReport = report.ToString();
            LastExitCode = totalErr > 0 ? 1 : 0;

            writeLine(LastExitCode == 0
                ? "GameTestSuite: 全部通過 (exit 0)"
                : "GameTestSuite: 有失敗 (exit 1)");

            return LastExitCode;
        }
    }
}
