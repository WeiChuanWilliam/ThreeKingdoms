using System;
using System.Text;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Tests.Runners
{
    /// <summary>兵種電話簿：數量、別名、虎豹／白马等特殊邏輯。</summary>
    public static class TroopKindRegistryTestRunner
    {
        public const int ExpectedKindCount = 37;

        public static GameTestResult Run()
        {
            var log = new StringBuilder();
            int err = 0;

            TroopKindRegistry.EnsureBuilt();
            log.AppendLine("=== TroopKindRegistry ===");

            int count = TroopKindRegistry.All.Count;
            if (count != ExpectedKindCount)
            {
                log.AppendLine($"ERROR 兵種數 {count}，期望 {ExpectedKindCount}");
                err++;
            }
            else
            {
                log.AppendLine($"OK  兵種數 {count}");
            }

            if (TroopKindRegistry.Get("spear.chinzhou")?.KindKey != TroopKindKeys.SpearQingzhou)
            {
                log.AppendLine("ERROR 別名 spear.chinzhou");
                err++;
            }
            else log.AppendLine("OK  別名 spear.chinzhou → qingzhou");

            if (TroopKindRegistry.Get("armor.baihau")?.KindKey != TroopKindKeys.ArmorBaimao)
            {
                log.AppendLine("ERROR 別名 armor.baihau");
                err++;
            }
            else log.AppendLine("OK  別名 armor.baihau → baimao");

            var hubao = TroopKindRegistry.Get(TroopKindKeys.KnightHubao);
            var baima = TroopKindRegistry.Get(TroopKindKeys.HorsemanBaima);
            if (hubao == null || baima == null)
            {
                log.AppendLine("ERROR 缺少 hubao 或 baima");
                err++;
            }
            else
            {
                float vsArcher = baima.GetDamageMultiplierAgainst(TroopKindRegistry.Get(TroopKindKeys.Archer));
                if (Math.Abs(vsArcher - 1.2f) > 0.001f)
                {
                    log.AppendLine($"ERROR 白马對弓倍率 {vsArcher} 期望 1.2");
                    err++;
                }
                else
                    log.AppendLine("OK  白马對弓 1.2x");
            }

            log.AppendLine($"--- ERROR={err} ---");
            return new GameTestResult("TroopKindRegistry", err == 0, log.ToString(), count - err, err);
        }
    }
}
