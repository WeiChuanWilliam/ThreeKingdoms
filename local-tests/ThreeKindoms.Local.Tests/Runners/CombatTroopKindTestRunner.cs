using System;
using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Local.Tests.Runners
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
            TestLog.Line(log, $"=== new Combat(CombatUnitDef) × {soldiers} 兵，預設士气/体力 100、金 0 ===");

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

                    if (unit.UnitName != def.CustomUnitName)
                        throw new InvalidOperationException($"部隊名 {unit.UnitName}");
                    if (unit.Soldiers != soldiers) throw new InvalidOperationException("兵力");
                    if (unit.Morale != 100 || unit.Stamina != 100) throw new InvalidOperationException("士气/体力");
                    if (unit.Money != 0) throw new InvalidOperationException("金錢");
                    if (unit.CountEquippedSkills() != 0) throw new InvalidOperationException("技能");
                    if (unit.TroopKindKey != kindKey) throw new InvalidOperationException("KindKey");
                    if (unit.TroopType != kind.Category) throw new InvalidOperationException("兵科");
                    if (unit.TroopAttack != kind.Attack) throw new InvalidOperationException("六圍");

                    TestLog.Line(log, CombatUnitDisplay.FormatLine(unit));
                    ok++;
                }
                catch (Exception ex)
                {
                    TestLog.Line(log, $"ERROR [{kindKey}] {ex.Message}");
                    err++;
                }
            }

            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("CombatTroopKind", err == 0, log.ToString(), ok, err);
        }

        static GameTestResult Fail(StringBuilder log, string msg, string path)
        {
            TestLog.Line(log, $"ERROR: {msg}");
            TestLog.Line(log, path);
            return new GameTestResult("CombatTroopKind", false, log.ToString(), 0, 1);
        }
    }
}
