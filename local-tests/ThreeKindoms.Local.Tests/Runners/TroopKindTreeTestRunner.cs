using System;
using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Local.Tests.Runners
{
    public static class TroopKindTreeTestRunner
    {
        public static GameTestResult Run()
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;
            TroopKindRegistry.EnsureBuilt();
            TestLog.Line(log, "=== TroopKindTree ===");

            foreach (string kindKey in SortedKeys())
            {
                if (!TroopKindTree.TryGetNode(kindKey, out _))
                {
                    TestLog.Line(log, $"ERROR [{kindKey}] 不在樹上");
                    err++;
                }
                else
                {
                    TestLog.Line(log, $"OK  [{kindKey}]");
                    ok++;
                }
            }

            string[] chain = { TroopKindKeys.NavySmall, TroopKindKeys.NavyMedium, TroopKindKeys.NavyLarge, TroopKindKeys.NavyFinal };
            for (int i = 0; i < chain.Length - 1; i++)
            {
                string next = TroopKindTree.GetNavyNext(chain[i]);
                if (next != chain[i + 1])
                {
                    TestLog.Line(log, $"ERROR navy {chain[i]} -> {next} (expected {chain[i + 1]})");
                    err++;
                }
                else
                {
                    TestLog.Line(log, $"OK  navy {chain[i]} -> {next}");
                }
            }

            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("TroopKindTree", err == 0, log.ToString(), ok, err);
        }

        static List<string> SortedKeys()
        {
            var keys = new List<string>(TroopKindRegistry.All.Keys);
            keys.Sort(StringComparer.Ordinal);
            return keys;
        }
    }
}
