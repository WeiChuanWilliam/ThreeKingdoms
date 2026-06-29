using System;
using ThreeKindoms.Local.Tests;
using ThreeKindoms.Local.Tests.Runners;

namespace ThreeKindoms.TestConsole
{
    static class Program
    {
        static int Main()
        {
            string props = TestPaths.UnitPropertiesPath;
            int failed = 0;

            failed += Run(CombatTroopKindTestRunner.Run(props));
            failed += Run(UnitPropertiesTestRunner.Run(props));
            failed += Run(TroopKindTreeTestRunner.Run());
            failed += Run(TroopKindRegistryTestRunner.Run());

            failed += Run(OfficerTestRunner.Run(
                TestPaths.OfficerPropertiesPath,
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath));

            Console.WriteLine(failed == 0 ? "ALL PASSED" : $"FAILED ({failed} suite(s))");
            return failed == 0 ? 0 : 1;
        }

        static int Run(GameTestResult r)
        {
            if (!string.IsNullOrEmpty(r.Report))
                Console.WriteLine(r.Report);

            if (!r.Passed)
                Console.WriteLine($"!! {r.Name} FAILED");

            return r.Passed ? 0 : 1;
        }
    }
}
