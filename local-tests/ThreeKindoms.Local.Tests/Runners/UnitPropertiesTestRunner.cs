using System;
using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Local.Tests.Runners
{
    public static class UnitPropertiesTestRunner
    {
        static readonly string[] StatSuffixes = { "attack", "defense", "mobility", "jipo", "gongcheng", "stamina" };

        public static GameTestResult Run(string unitPropertiesPath)
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            if (!UnitConfigUtil.Load(unitPropertiesPath))
            {
                TestLog.Line(log, "ERROR 無法載入 properties");
                return new GameTestResult("UnitProperties", false, log.ToString(), 0, 1);
            }

            TroopKindRegistry.EnsureBuilt();
            TestLog.Line(log, "=== unit.properties ===");

            var keys = new List<string>(TroopKindRegistry.All.Keys);
            keys.Sort(StringComparer.Ordinal);

            foreach (string kindKey in keys)
            {
                bool fail = false;
                if (string.IsNullOrEmpty(UnitConfigUtil.GetKindDisplayName(kindKey)))
                    fail = true;
                foreach (string stat in StatSuffixes)
                {
                    if (!UnitConfigUtil.HasKindStat(kindKey, stat))
                        fail = true;
                }

                if (fail)
                {
                    TestLog.Line(log, $"ERROR [{kindKey}]");
                    err++;
                }
                else
                {
                    TestLog.Line(log, $"OK  [{kindKey}]");
                    ok++;
                }
            }

            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("UnitProperties", err == 0, log.ToString(), ok, err);
        }
    }
}
