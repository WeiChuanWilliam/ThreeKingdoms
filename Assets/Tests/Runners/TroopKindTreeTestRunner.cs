using System;
using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Tests.Runners
{
    /// <summary>升級樹：registry 每鍵在樹上、水軍線性鏈、葉子 stage。</summary>
    public static class TroopKindTreeTestRunner
    {
        public static GameTestResult Run()
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            TroopKindRegistry.EnsureBuilt();
            log.AppendLine("=== TroopKindTree ===");

            foreach (string kindKey in SortedRegistryKeys())
            {
                if (!TroopKindTree.TryGetNode(kindKey, out TroopKindNode node))
                {
                    log.AppendLine($"ERROR [{kindKey}] 不在樹上");
                    err++;
                    continue;
                }

                int stage = TroopKindTree.GetStage(kindKey);
                if (stage < 1 || stage > 4)
                {
                    log.AppendLine($"ERROR [{kindKey}] stage={stage}");
                    err++;
                    continue;
                }

                log.AppendLine($"OK  [{kindKey}] type={node.TroopType} stage={stage} parent={node.ParentKey ?? "(root)"}");
                ok++;
            }

            if (CheckNavyChain(log))
                ok++;
            else
                err++;

            log.AppendLine($"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("TroopKindTree", err == 0, log.ToString(), ok, err);
        }

        static bool CheckNavyChain(StringBuilder log)
        {
            string[] chain =
            {
                TroopKindKeys.NavySmall,
                TroopKindKeys.NavyMedium,
                TroopKindKeys.NavyLarge,
                TroopKindKeys.NavyFinal
            };

            for (int i = 0; i < chain.Length - 1; i++)
            {
                string next = TroopKindTree.GetNavyNext(chain[i]);
                if (next != chain[i + 1])
                {
                    log.AppendLine($"ERROR 水軍鏈 {chain[i]} -> 期望 {chain[i + 1]} 實際 {next}");
                    return false;
                }
            }

            log.AppendLine("OK  水軍鏈 small→medium→large→final");
            return true;
        }

        static List<string> SortedRegistryKeys()
        {
            var keys = new List<string>(TroopKindRegistry.All.Keys);
            keys.Sort(StringComparer.Ordinal);
            return keys;
        }
    }
}
