using System;
using System.Text;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Local.Tests.Runners
{
    public static class TroopKindRegistryTestRunner
    {
        public const int ExpectedKindCount = 37;

        public static GameTestResult Run()
        {
            var log = new StringBuilder();
            int err = 0;
            TroopKindRegistry.EnsureBuilt();
            TestLog.Line(log, "=== TroopKindRegistry ===");

            int count = TroopKindRegistry.All.Count;
            if (count != ExpectedKindCount)
            {
                TestLog.Line(log, $"ERROR count={count} expected={ExpectedKindCount}");
                err++;
            }
            else
            {
                TestLog.Line(log, $"OK  count={count}");
            }

            var chinzhou = TroopKindRegistry.Get("spear.chinzhou");
            if (chinzhou?.KindKey != TroopKindKeys.SpearQingzhou)
            {
                TestLog.Line(log, "ERROR alias spear.chinzhou -> spear.qingzhou");
                err++;
            }
            else
            {
                TestLog.Line(log, $"OK  alias spear.chinzhou -> {chinzhou.KindKey}");
            }

            var baima = TroopKindRegistry.Get(TroopKindKeys.HorsemanBaima);
            var archer = TroopKindRegistry.Get(TroopKindKeys.Archer);
            float mult = baima.GetDamageMultiplierAgainst(archer);
            if (Math.Abs(mult - 1.2f) > 0.001f)
            {
                TestLog.Line(log, $"ERROR baima vs archer mult={mult}");
                err++;
            }
            else
            {
                TestLog.Line(log, $"OK  baima vs archer mult={mult}");
            }

            TestLog.Line(log, $"--- ERROR={err} ---");
            return new GameTestResult("TroopKindRegistry", err == 0, log.ToString(), ExpectedKindCount - err, err);
        }
    }
}
