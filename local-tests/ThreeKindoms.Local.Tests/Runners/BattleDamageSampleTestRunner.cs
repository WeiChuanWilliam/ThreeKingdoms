using System.Collections.Generic;
using System.Text;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Local.Tests.Runners
{
    /// <summary>
    /// 每位武將一隊刀兵 1000：套用戰損，印部隊狀態（兵／傷／士氣／體力）與 Stats 前後差別。
    /// </summary>
    public static class BattleDamageSampleTestRunner
    {
        public const string TroopKindKey = "blade";
        public const int SoldiersBefore = 1000;
        public const int Deaths = 100;
        public const int WoundedIncrease = 200;
        public const short MoraleLoss = 2;
        public const short StaminaLoss = 2;
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

            var damage = new BattleDamageResult(Deaths, WoundedIncrease, MoraleLoss, StaminaLoss);

            TestLog.Line(log, "=== 戰損前後對照：每位武將 × 刀兵1000 ===");
            TestLog.Line(log,
                $"套用 BattleDamageResult(陣亡={Deaths}, 傷兵+={WoundedIncrease}, 士氣-={MoraleLoss}, 體力-={StaminaLoss})");
            TestLog.Line(log,
                $"預期：兵 {SoldiersBefore}→{SoldiersBefore - Deaths - WoundedIncrease}；傷 0→{WoundedIncrease}；" +
                $"士氣 {UnitUtil.DefaultMorale}→{UnitUtil.DefaultMorale - MoraleLoss}；" +
                $"體力 {UnitUtil.DefaultStamina}→{UnitUtil.DefaultStamina - StaminaLoss}");
            TestLog.Line(log, "（陣亡從可戰移除；轉傷＝可戰減、傷兵加。OfficerFive 不因 Apply 改變）");
            TestLog.Line(log, "");
            TestLog.Line(log, "【部隊狀態總表】兵／傷／士氣／體力 前後");
            TestLog.Line(log,
                $"{"id",4} {"名稱",-10} | {"兵前",5}→{"兵後",-5} {"傷前",4}→{"傷後",-4} {"氣前",4}→{"氣後",-4} {"體前",4}→{"體後",-4}");
            TestLog.Line(log, new string('-', 78));

            var detailLog = new StringBuilder();
            var ids = new List<int>(OfficerDatabase.Officers.Keys);
            ids.Sort();

            foreach (int id in ids)
            {
                try
                {
                    Combat combat = UnitUtil.CreateCombat(
                        FactionId,
                        TroopKindKey,
                        SoldiersBefore,
                        commanderOfficerId: id);

                    if (combat.Commander == null)
                        throw new System.InvalidOperationException("主將為 null");
                    if (combat.Soldiers != SoldiersBefore || combat.Wounded != 0)
                        throw new System.InvalidOperationException(
                            $"初始應為兵{SoldiersBefore}/傷0，實際 兵{combat.Soldiers}/傷{combat.Wounded}");

                    string name = combat.Commander.FullName ?? $"#{id}";
                    if (name.Length > 10)
                        name = name.Substring(0, 10);

                    Snapshot before = Snapshot.Capture(combat);

                    BattleDamageResult.Apply(combat, damage);

                    int expectSoldiers = SoldiersBefore - Deaths - WoundedIncrease;
                    int expectMorale = UnitUtil.DefaultMorale - MoraleLoss;
                    int expectStamina = UnitUtil.DefaultStamina - StaminaLoss;
                    if (combat.Soldiers != expectSoldiers)
                        throw new System.InvalidOperationException(
                            $"兵後應為 {expectSoldiers}，實際 {combat.Soldiers}");
                    if (combat.Wounded != WoundedIncrease)
                        throw new System.InvalidOperationException(
                            $"傷後應為 {WoundedIncrease}，實際 {combat.Wounded}");
                    if (combat.Morale != expectMorale)
                        throw new System.InvalidOperationException(
                            $"士氣後應為 {expectMorale}，實際 {combat.Morale}");
                    if (combat.Stamina != expectStamina)
                        throw new System.InvalidOperationException(
                            $"體力後應為 {expectStamina}，實際 {combat.Stamina}");

                    Snapshot after = Snapshot.Capture(combat);

                    TestLog.Line(log,
                        $"{id,4} {name,-10} | {before.Soldiers,5}→{after.Soldiers,-5} {before.Wounded,4}→{after.Wounded,-4} " +
                        $"{before.Morale,4}→{after.Morale,-4} {before.Stamina,4}→{after.Stamina,-4}");

                    TestLog.Line(detailLog, $"--- id={id} {name} ---");
                    TestLog.Line(detailLog, FormatUnitState("前", before));
                    TestLog.Line(detailLog, FormatOfficerFive("前", before));
                    TestLog.Line(detailLog, FormatStats("前", before));
                    TestLog.Line(detailLog, FormatUnitState("後", after));
                    TestLog.Line(detailLog, FormatOfficerFive("後", after));
                    TestLog.Line(detailLog, FormatStats("後", after));
                    TestLog.Line(detailLog, FormatDelta(before, after));
                    TestLog.Line(detailLog, "");
                    ok++;
                }
                catch (System.Exception ex)
                {
                    TestLog.Line(log, $"ERROR id={id}: {ex.Message}");
                    err++;
                }
            }

            TestLog.Line(log, "");
            TestLog.Line(log, "【各隊明細】部隊狀態 + 統合五維 + Stats");
            TestLog.Line(log, "");
            log.Append(detailLog);

            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("BattleDamageSample", err == 0 && ok > 0, log.ToString(), ok, err);
        }

        static string FormatUnitState(string tag, Snapshot s) =>
            $"{tag} 部隊狀態：兵{s.Soldiers} 傷{s.Wounded} 士氣{s.Morale} 體力{s.Stamina} 金{s.Money}";

        static string FormatOfficerFive(string tag, Snapshot s) =>
            $"{tag} 統合五維：統{s.Leadership} 武{s.Force} 智{s.Intelligence} 政{s.Policy} 魅{s.Charisma}";

        static string FormatStats(string tag, Snapshot s) =>
            $"{tag} Stats六項：攻{s.Attack} 防{s.Defense} 破{s.Jipo} 城{s.Gongcheng} 策{s.Strategy} 建{s.Construction}";

        static string FormatDelta(Snapshot before, Snapshot after) =>
            $"差 部隊：兵{Delta(before.Soldiers, after.Soldiers)} 傷{Delta(before.Wounded, after.Wounded)} " +
            $"士氣{Delta(before.Morale, after.Morale)} 體力{Delta(before.Stamina, after.Stamina)} | " +
            $"Stats：攻{Delta(before.Attack, after.Attack)} 防{Delta(before.Defense, after.Defense)} " +
            $"破{Delta(before.Jipo, after.Jipo)} 城{Delta(before.Gongcheng, after.Gongcheng)} " +
            $"策{Delta(before.Strategy, after.Strategy)} 建{Delta(before.Construction, after.Construction)}";

        static string Delta(int before, int after)
        {
            int d = after - before;
            return d == 0 ? "0" : (d > 0 ? $"+{d}" : d.ToString());
        }

        static GameTestResult Fail(StringBuilder log, string message)
        {
            TestLog.Line(log, "ERROR " + message);
            return new GameTestResult("BattleDamageSample", false, log.ToString(), 0, 1);
        }

        readonly struct Snapshot
        {
            public int Soldiers { get; init; }
            public int Wounded { get; init; }
            public short Morale { get; init; }
            public short Stamina { get; init; }
            public int Money { get; init; }
            public short Leadership { get; init; }
            public short Force { get; init; }
            public short Intelligence { get; init; }
            public short Policy { get; init; }
            public short Charisma { get; init; }
            public int Attack { get; init; }
            public int Defense { get; init; }
            public int Jipo { get; init; }
            public int Gongcheng { get; init; }
            public int Strategy { get; init; }
            public int Construction { get; init; }

            public static Snapshot Capture(Combat combat)
            {
                var five = combat.OfficerFive;
                var stats = combat.Stats;
                return new Snapshot
                {
                    Soldiers = combat.Soldiers,
                    Wounded = combat.Wounded,
                    Morale = combat.Morale,
                    Stamina = combat.Stamina,
                    Money = combat.Money,
                    Leadership = five.Leadership,
                    Force = five.Force,
                    Intelligence = five.Intelligence,
                    Policy = five.Policy,
                    Charisma = five.Charisma,
                    Attack = stats.Attack,
                    Defense = stats.Defense,
                    Jipo = stats.Jipo,
                    Gongcheng = stats.Gongcheng,
                    Strategy = stats.Strategy,
                    Construction = stats.Construction
                };
            }
        }
    }
}
