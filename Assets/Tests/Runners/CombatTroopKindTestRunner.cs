using System;
using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Tests.Runners
{
    public static class CombatTroopKindTestRunner
    {
        public const int DefaultSoldiers = 5000;

        public static GameTestResult Run(string unitPropertiesPath, int soldiers = DefaultSoldiers, int factionId = 1)
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            if (!UnitConfigUtil.IsLoaded && !UnitConfigUtil.Load(unitPropertiesPath))
                return Fail(log, "無法載入 unit.properties", unitPropertiesPath);

            TroopKindRegistry.EnsureBuilt();
            log.AppendLine($"=== new Combat(CombatUnitDef) × {soldiers} 兵 ===");

            var keys = new List<string>(TroopKindRegistry.All.Keys);
            keys.Sort(StringComparer.Ordinal);

            foreach (string kindKey in keys)
            {
                try
                {
                    if (!TroopKindRegistry.TryGet(kindKey, out AbstractTroopKind kind))
                        throw new InvalidOperationException("registry miss");
                    if (!UnitConfigUtil.TryGetKindBaseStats(kindKey, out _))
                        throw new InvalidOperationException("properties 缺少六圍");

                    CombatUnitDef def = CombatUnitDef.FromTroopKind(factionId, kindKey, soldiers);
                    Combat unit = new Combat(def);

                    if (unit.TroopAttack != kind.Attack || unit.TroopDefense != kind.Defense)
                        throw new InvalidOperationException("六圍");

                    log.AppendLine(CombatUnitDisplay.FormatLine(unit));
                    ok++;
                }
                catch (Exception ex)
                {
                    log.AppendLine($"ERROR [{kindKey}] {ex.Message}");
                    err++;
                }
            }

            log.AppendLine($"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("CombatTroopKind", err == 0, log.ToString(), ok, err);
        }

        static GameTestResult Fail(StringBuilder log, string msg, string path)
        {
            log.AppendLine($"ERROR: {msg}");
            log.AppendLine(path);
            return new GameTestResult("CombatTroopKind", false, log.ToString(), 0, 1);
        }
    }
}
