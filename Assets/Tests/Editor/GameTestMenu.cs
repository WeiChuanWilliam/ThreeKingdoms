using System.IO;
using ThreeKindoms.Tests;
using UnityEditor;
using UnityEngine;

namespace ThreeKindoms.Tests.Editor
{
    /// <summary>Unity 選單執行測試（本專案沒有獨立 exe，請用此處或 Test Runner）。</summary>
    public static class GameTestMenu
    {
        const string MenuRoot = "ThreeKindoms/Tests/";

        [MenuItem(MenuRoot + "Run All (Main)", false, 1)]
        public static void RunAllMain()
        {
            int code = RunAllInternal();
            EditorUtility.DisplayDialog("GameTestSuite",
                code == 0 ? "全部通過 (exit 0)" : "有失敗，見 Console (exit 1)",
                "確定");
        }

        [MenuItem(MenuRoot + "Run All (Main)", true)]
        public static bool RunAllMainValidate() => !EditorApplication.isPlaying;

        [MenuItem(MenuRoot + "Combat / 全兵種 5000", false, 20)]
        public static void RunCombatOnly()
        {
            string path = GetPropertiesPath();
            var r = Runners.CombatTroopKindTestRunner.Run(path);
            LogResult(r);
        }

        [MenuItem(MenuRoot + "Unit Properties / 鍵與六圍", false, 21)]
        public static void RunPropertiesOnly()
        {
            var r = Runners.UnitPropertiesTestRunner.Run(GetPropertiesPath());
            LogResult(r);
        }

        [MenuItem(MenuRoot + "Troop Tree / 升級樹", false, 22)]
        public static void RunTreeOnly()
        {
            var r = Runners.TroopKindTreeTestRunner.Run();
            LogResult(r);
        }

        [MenuItem(MenuRoot + "Troop Registry / 電話簿", false, 23)]
        public static void RunRegistryOnly()
        {
            var r = Runners.TroopKindRegistryTestRunner.Run();
            LogResult(r);
        }

        static int RunAllInternal()
        {
            string path = GetPropertiesPath();
            int code = GameTestSuite.RunAll(path, Debug.Log);
            if (code == 0)
                Debug.Log(GameTestSuite.LastCombinedReport);
            else
                Debug.LogError(GameTestSuite.LastCombinedReport);
            return code;
        }

        static string GetPropertiesPath() =>
            Path.Combine(Application.dataPath, "StreamingAssets/chinese/unit.properties");

        static void LogResult(GameTestResult r)
        {
            if (r.Passed)
                Debug.Log(r.Report);
            else
                Debug.LogError(r.Report);
        }
    }
}
