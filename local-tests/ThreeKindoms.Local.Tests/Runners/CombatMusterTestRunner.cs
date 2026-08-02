using System.Text;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Local.Tests.Runners
{
    public static class CombatMusterTestRunner
    {
        public const int FactionId = 1;
        public const string TroopKindKey = "blade";
        public const int Soldiers = 5000;
        public const int Wounded = 200;
        public const int CommanderId = 1;
        public const int ViceId = 2;

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

            TestLog.Line(log, "=== Combat 組隊：武將入伍＋帶兵 ===");
            TestLog.Line(log, $"兵種={TroopKindKey} 兵力={Soldiers} 傷兵={Wounded} 主將id={CommanderId} 副將陣列=[{ViceId},3]");
            TestLog.Line(log, "");

            try
            {
                Check(log, ref ok, ref err, "UnitUtil.Create＋副將陣列＋隊名", () =>
                {
                    Unit created = UnitUtil.Create(
                        UnitKind.Combat,
                        FactionId,
                        CommanderId,
                        TroopKindKey,
                        new[] { ViceId, 3 },
                        Soldiers,
                        wounded: Wounded);

                    if (created is not Combat combat)
                        throw new System.InvalidOperationException("應產生 Combat");
                    string expectedName = UnitUtil.ResolveUnitName(null, CommanderId, TroopKindKey, UnitKind.Combat);
                    if (combat.UnitName != expectedName)
                        throw new System.InvalidOperationException($"隊名應為 {expectedName}，實際 {combat.UnitName}");
                    if (combat.IsGarrison)
                        throw new System.InvalidOperationException("新建應 IsGarrison=false");
                    if (combat.ViceOfficer?.RuntimeId != ViceId)
                        throw new System.InvalidOperationException("副將陣列應只取第一位關羽");
                    if (!combat.Commander.IsDeployed || !combat.ViceOfficer.IsDeployed)
                        throw new System.InvalidOperationException("Combat 組隊後主副將應為出戰");
                    AssertMuster(combat);
                    TestLog.Line(log, CombatUnitDisplay.FormatLine(combat));
                    TestLog.Line(log, FormatOfficerDetail(combat));
                });

                Check(log, ref ok, ref err, "執行時 SetCommander／SetVice／SetManpower", () =>
                {
                    Combat combat = UnitUtil.CreateCombat(
                        FactionId,
                        TroopKindKey,
                        soldiers: 1000,
                        commanderOfficerId: 0);
                    combat.SetCommanderFromPool(CommanderId);
                    if (!combat.SetViceOfficerFromPool(ViceId))
                        throw new System.InvalidOperationException("SetViceOfficerFromPool 失敗");
                    combat.SetManpower(Soldiers, Wounded);
                    combat.SetMorale(90);
                    combat.SetStamina(85);

                    AssertMuster(combat);
                    if (combat.Morale != 90 || combat.Stamina != 85)
                        throw new System.InvalidOperationException("士氣／體力未套用");

                    TestLog.Line(log, CombatUnitDisplay.FormatLine(combat));
                    TestLog.Line(log, FormatOfficerDetail(combat));
                });

                Check(log, ref ok, ref err, "副將上限 1 人", () =>
                {
                    Combat combat = UnitUtil.CreateCombat(FactionId, TroopKindKey, 100, CommanderId);
                    if (!combat.AddViceOfficerFromPool(ViceId))
                        throw new System.InvalidOperationException("第一位副將應成功");
                    if (combat.AddViceOfficerFromPool(3))
                        throw new System.InvalidOperationException("第二位副將應被拒絕");
                    if (combat.ViceOfficer?.RuntimeId != ViceId)
                        throw new System.InvalidOperationException("ViceOfficer 應為關羽");
                });

                Check(log, ref ok, ref err, "Deploy：落點可立即移動", () =>
                {
                    var grid = new LocationGrid();
                    var spawn = new HexCoord(0, 0);
                    var next = new HexCoord(1, 0);
                    AbstractTerrain plain = TerrainDefinition.FromTerrainType(TerrainType.Plain);

                    Combat combat = UnitUtil.DeployCombat(
                        FactionId,
                        TroopKindKey,
                        1000,
                        CommanderId,
                        new[] { ViceId },
                        grid,
                        spawn,
                        plain);

                    if (combat.IsGarrison)
                        throw new System.InvalidOperationException("Deploy 後應可行動（非駐紮）");
                    if (!combat.IsOnMap || !combat.CurrentHex.Equals(spawn))
                        throw new System.InvalidOperationException("應在產生地點");
                    if (!combat.Location.EnterHex(next, plain))
                        throw new System.InvalidOperationException("應能移出產生地點");
                    if (!combat.CurrentHex.Equals(next))
                        throw new System.InvalidOperationException("移動後座標不符");
                });

                Check(log, ref ok, ref err, "卸任副將→取消出戰", () =>
                {
                    Combat combat = UnitUtil.CreateCombat(
                        FactionId, TroopKindKey, 100, CommanderId, new[] { ViceId });
                    Officer vice = combat.ViceOfficer;
                    if (vice == null || !vice.IsDeployed)
                        throw new System.InvalidOperationException("副將應先為出戰");
                    if (!combat.RemoveViceOfficer(ViceId))
                        throw new System.InvalidOperationException("RemoveViceOfficer 失敗");
                    if (vice.IsDeployed)
                        throw new System.InvalidOperationException("卸任後 IsDeployed 應為 false");
                });
            }
            catch (System.Exception ex)
            {
                TestLog.Line(log, $"ERROR 未預期例外：{ex.Message}");
                err++;
            }

            TestLog.Line(log, $"--- OK={ok} ERROR={err} ---");
            return new GameTestResult("CombatMuster", err == 0, log.ToString(), ok, err);
        }

        static void AssertMuster(Combat combat)
        {
            if (combat.Kind != UnitKind.Combat)
                throw new System.InvalidOperationException("Kind 應為 Combat");
            if (combat.Soldiers != Soldiers)
                throw new System.InvalidOperationException($"兵力應為 {Soldiers}，實際 {combat.Soldiers}");
            if (combat.Wounded != Wounded)
                throw new System.InvalidOperationException($"傷兵應為 {Wounded}，實際 {combat.Wounded}");
            if (combat.TroopKindKey != TroopKindKey)
                throw new System.InvalidOperationException($"兵種應為 {TroopKindKey}");
            if (combat.Commander == null || combat.Commander.RuntimeId != CommanderId)
                throw new System.InvalidOperationException("主將應為劉備 (id=1)");
            if (combat.ViceOfficer == null || combat.ViceOfficer.RuntimeId != ViceId)
                throw new System.InvalidOperationException("副將應為關羽 (id=2)");
            if (combat.Stats.Attack <= 0)
                throw new System.InvalidOperationException("攻擊力應 > 0");
        }

        static string FormatOfficerDetail(Combat combat)
        {
            Officer cmd = combat.Commander;
            Officer vice = combat.ViceOfficer;
            var s = combat.Stats;
            return
                $"     主將 {cmd.FullName} 統{cmd.LeadershipPerform} 武{cmd.AttackPerform} | " +
                $"副將 {vice.FullName} 統{vice.LeadershipPerform} 武{vice.AttackPerform} | " +
                $"攻{s.Attack} 防{s.Defense} 城{s.Gongcheng} 破{s.Jipo} 策{s.Strategy} 建{s.Construction}";
        }

        static void Check(StringBuilder log, ref int ok, ref int err, string name, System.Action action)
        {
            try
            {
                action();
                TestLog.Line(log, $"OK  {name}");
                ok++;
            }
            catch (System.Exception ex)
            {
                TestLog.Line(log, $"ERROR {name}：{ex.Message}");
                err++;
            }
        }

        static GameTestResult Fail(StringBuilder log, string msg)
        {
            TestLog.Line(log, $"ERROR {msg}");
            return new GameTestResult("CombatMuster", false, log.ToString(), 0, 1);
        }
    }
}
