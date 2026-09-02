using System.Text;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Local.Tests.Runners
{
    /// <summary>
    /// 劉備刀兵：固定兵力／士氣／體力，只改主將傷勢，對照統合五維與 Stats。
    /// （部隊傷兵 <see cref="Unit.Wounded"/> 固定 0；比較的是武將 <see cref="Officer.Injury"/>。）
    /// </summary>
    public static class LiuBeiInjuryCompareTestRunner
    {
        public const int CommanderId = 1;
        public const int FactionId = 1;
        public const string TroopKindKey = "blade";
        public const int Soldiers = 1000;

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

            Officer liuBei = OfficerPool.Get(CommanderId);
            if (liuBei == null)
                return Fail(log, "找不到劉備 (id=1)");

            var savedInjury = liuBei.Injury;

            TestLog.Line(log, "=== 劉備：同兵力下主將傷勢 vs 部隊戰鬥力 ===");
            TestLog.Line(log,
                $"刀兵 {Soldiers} 兵、傷兵 0、士氣 {UnitUtil.DefaultMorale}、體力 {UnitUtil.DefaultStamina}、無副將");
            TestLog.Line(log, "只改主將 Injury；讀 combat.Stats 時依 OfficerFive × 兵力／士氣／體力重算。");
            TestLog.Line(log, "");
            TestLog.Line(log,
                $"{"傷勢",-8} {"統",4} {"武",4} {"智",4} {"政",4} {"魅",4} | " +
                $"{"攻",5} {"防",5} {"破",5} {"城",5} {"策",5} {"建",5}");
            TestLog.Line(log, new string('-', 72));

            var rows = new System.Collections.Generic.List<Snapshot>();

            foreach (OfficerInjuryState injury in new[]
                     {
                         OfficerInjuryState.Normal,
                         OfficerInjuryState.Light,
                         OfficerInjuryState.Medium,
                         OfficerInjuryState.Severe
                     })
            {
                try
                {
                    Combat combat = UnitUtil.CreateCombat(
                        FactionId,
                        TroopKindKey,
                        Soldiers,
                        CommanderId);

                    if (combat.Commander == null)
                        throw new System.InvalidOperationException("主將為 null");

                    combat.Commander.SetInjury(injury);
                    combat.EnsureStats();

                    Snapshot row = Snapshot.Capture(combat, injury);
                    rows.Add(row);
                    TestLog.Line(log, row.FormatLine());
                    ok++;
                }
                catch (System.Exception ex)
                {
                    TestLog.Line(log, $"ERROR {injury}: {ex.Message}");
                    err++;
                }
            }

            liuBei.SetInjury(savedInjury);

            Snapshot? normalRow = rows.Find(r => r.Injury == OfficerInjuryState.Normal);
            if (normalRow.HasValue && rows.Count >= 2)
            {
                TestLog.Line(log, "");
                TestLog.Line(log, "【相對無傷 (Normal) 的 Stats 差】");
                foreach (Snapshot row in rows)
                {
                    if (row.Injury == OfficerInjuryState.Normal)
                        continue;
                    TestLog.Line(log, $"{InjuryLabel(row.Injury)}：{FormatStatsDelta(normalRow.Value, row)}");
                }
            }

            TestLog.Line(log, "");
            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("LiuBeiInjuryCompare", err == 0 && ok == 4, log.ToString(), ok, err);
        }

        static string InjuryLabel(OfficerInjuryState injury) => injury switch
        {
            OfficerInjuryState.Normal => "無傷",
            OfficerInjuryState.Light => "輕傷",
            OfficerInjuryState.Medium => "中傷",
            OfficerInjuryState.Severe => "重傷",
            _ => injury.ToString()
        };

        static string FormatStatsDelta(Snapshot normal, Snapshot injured) =>
            $"攻{Delta(normal.Attack, injured.Attack)} 防{Delta(normal.Defense, injured.Defense)} " +
            $"破{Delta(normal.Jipo, injured.Jipo)} 城{Delta(normal.Gongcheng, injured.Gongcheng)} " +
            $"策{Delta(normal.Strategy, injured.Strategy)} 建{Delta(normal.Construction, injured.Construction)}";

        static string Delta(int before, int after)
        {
            int d = after - before;
            return d == 0 ? "0" : (d > 0 ? $"+{d}" : d.ToString());
        }

        static GameTestResult Fail(StringBuilder log, string message)
        {
            TestLog.Line(log, "ERROR " + message);
            return new GameTestResult("LiuBeiInjuryCompare", false, log.ToString(), 0, 1);
        }

        readonly struct Snapshot
        {
            public OfficerInjuryState Injury { get; init; }
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

            public static Snapshot Capture(Combat combat, OfficerInjuryState injury)
            {
                var five = combat.OfficerFive;
                var stats = combat.Stats;
                return new Snapshot
                {
                    Injury = injury,
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

            public string FormatLine() =>
                $"{InjuryLabel(Injury),-8} {Leadership,4} {Force,4} {Intelligence,4} {Policy,4} {Charisma,4} | " +
                $"{Attack,5} {Defense,5} {Jipo,5} {Gongcheng,5} {Strategy,5} {Construction,5}";
        }
    }
}
