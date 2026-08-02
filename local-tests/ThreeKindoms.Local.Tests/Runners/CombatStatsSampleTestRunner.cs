using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Local.Tests.Runners
{
    /// <summary>
    /// 參數檔每位武將各組一隊：刀兵 1000、無副將、預設士氣／體力 100，列出 Stats 六項。
    /// </summary>
    public static class CombatStatsSampleTestRunner
    {
        public const string TroopKindKey = "blade";
        public const int Soldiers = 1000;
        public const int FactionId = 1;

        public static GameTestResult Run()
        {
            var log = new StringBuilder();
            int ok = 0, err = 0;

            if (!UnitConfigUtil.IsLoaded && !UnitConfigUtil.Load(TestPaths.UnitPropertiesPath))
                return Fail(log, "無法載入 unit.properties");

            if (!OfficerConfigUtil.IsLoaded && !OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath))
                return Fail(log, "無法載入 officer.properties");

            OfficerDatabase.Load(TestPaths.OfficersJsonPath, TestPaths.PersonalityTraitsPath);
            if (OfficerDatabase.Count == 0)
                return Fail(log, "officers.json 無武將");

            TestLog.Line(log, "=== 每位武將 × 刀兵1000：Stats 六項 ===");
            TestLog.Line(log,
                $"兵種={TroopKindKey} 兵力={Soldiers} 傷兵=0 士氣={UnitUtil.DefaultMorale} 體力={UnitUtil.DefaultStamina} 無副將");
            TestLog.Line(log, "公式暫代：五維合成(主×2+副×1)/3；最終＝屬性×兵力/100×士氣/100×體力/100");
            TestLog.Line(log, "");
            TestLog.Line(log,
                $"{"id",4} {"名稱",-10} {"統",4} {"武",4} {"智",4} {"政",4} {"魅",4} | {"攻",6} {"防",6} {"破",6} {"城",6} {"策",6} {"建",6}");
            TestLog.Line(log, new string('-', 88));

            var ids = new List<int>(OfficerDatabase.Officers.Keys);
            ids.Sort();

            foreach (int id in ids)
            {
                try
                {
                    Combat combat = UnitUtil.CreateCombat(
                        FactionId,
                        TroopKindKey,
                        Soldiers,
                        commanderOfficerId: id);

                    if (combat.Commander == null)
                        throw new System.InvalidOperationException("主將為 null");
                    if (combat.Soldiers != Soldiers)
                        throw new System.InvalidOperationException($"兵力應為 {Soldiers}");

                    var s = combat.Stats;
                    string name = combat.Commander.FullName ?? $"#{id}";
                    if (name.Length > 10)
                        name = name.Substring(0, 10);

                    TestLog.Line(log,
                        $"{id,4} {name,-10} {combat.UnitLeadership,4} {combat.UnitForce,4} {combat.UnitIntelligence,4} {combat.UnitPolicy,4} {combat.UnitCharisma,4} | " +
                        $"{s.Attack,6} {s.Defense,6} {s.Jipo,6} {s.Gongcheng,6} {s.Strategy,6} {s.Construction,6}");
                    ok++;
                }
                catch (System.Exception ex)
                {
                    TestLog.Line(log, $"ERROR id={id}: {ex.Message}");
                    err++;
                }
            }

            TestLog.Line(log, "");
            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("CombatStatsSample", err == 0 && ok > 0, log.ToString(), ok, err);
        }

        static GameTestResult Fail(StringBuilder log, string message)
        {
            TestLog.Line(log, "ERROR " + message);
            return new GameTestResult("CombatStatsSample", false, log.ToString(), 0, 1);
        }
    }
}
