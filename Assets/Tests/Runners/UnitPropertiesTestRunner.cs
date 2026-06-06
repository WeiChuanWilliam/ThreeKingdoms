using System;
using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Tests.Runners
{
    /// <summary>unit.properties：每個 registry 兵種必有顯示名與六圍。</summary>
    public static class UnitPropertiesTestRunner
    {
        static readonly string[] StatSuffixes = { "attack", "defense", "mobility", "jipo", "gongcheng", "stamina" };

        public static GameTestResult Run(string unitPropertiesPath)
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            if (!UnitConfigUtil.Load(unitPropertiesPath))
                return new GameTestResult("UnitProperties", false, $"ERROR: 無法載入\n{unitPropertiesPath}", 0, 1);

            TroopKindRegistry.EnsureBuilt();
            log.AppendLine("=== unit.properties 兵種鍵 ===");

            foreach (string kindKey in SortedRegistryKeys())
            {
                string display = UnitConfigUtil.GetKindDisplayName(kindKey);
                if (string.IsNullOrEmpty(display) || display == kindKey)
                {
                    log.AppendLine($"ERROR [{kindKey}] 缺少 troop.kind.{kindKey} 顯示名");
                    err++;
                    continue;
                }

                bool statsOk = true;
                foreach (string stat in StatSuffixes)
                {
                    if (!UnitConfigUtil.HasKindStat(kindKey, stat))
                    {
                        log.AppendLine($"ERROR [{kindKey}] 缺少 .{stat}");
                        statsOk = false;
                    }
                }

                if (!statsOk)
                {
                    err++;
                    continue;
                }

                log.AppendLine($"OK  [{kindKey}] = {display}");
                ok++;
            }

            log.AppendLine($"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("UnitProperties", err == 0, log.ToString(), ok, err);
        }

        static List<string> SortedRegistryKeys()
        {
            TroopKindRegistry.EnsureBuilt();
            var keys = new List<string>(TroopKindRegistry.All.Keys);
            keys.Sort(StringComparer.Ordinal);
            return keys;
        }
    }
}
