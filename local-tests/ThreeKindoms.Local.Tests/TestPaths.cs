using System;
using System.IO;

namespace ThreeKindoms.Local.Tests
{
    public static class TestPaths
    {
        public static string UnitPropertiesPath
        {
            get
            {
                string fromOutput = Path.Combine(AppContext.BaseDirectory, "unit.properties");
                if (File.Exists(fromOutput))
                    return Path.GetFullPath(fromOutput);

                string dir = AppContext.BaseDirectory;
                for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
                {
                    string candidate = Path.Combine(dir, "Assets", "StreamingAssets", "chinese", "unit.properties");
                    if (File.Exists(candidate))
                        return Path.GetFullPath(candidate);
                    dir = Directory.GetParent(dir)?.FullName;
                }

                throw new FileNotFoundException(
                    "找不到 unit.properties。請從 repo 根目錄執行 dotnet test，或確認 StreamingAssets 存在。");
            }
        }

        public static string StreamingAssetsPath => ResolveUnderAssets("StreamingAssets");

        public static string OfficersJsonPath => Path.Combine(StreamingAssetsPath, "officers.json");

        public static string PersonalityTraitsPath => Path.Combine(StreamingAssetsPath, "personality_traits.json");

        public static string OfficerPropertiesPath =>
            Path.Combine(StreamingAssetsPath, "chinese", "officer.properties");

        public static string ScenarioOfficersPath =>
            Path.Combine(StreamingAssetsPath, "scenario_officers", "opening.json");

        static string ResolveUnderAssets(string leaf)
        {
            string dir = AppContext.BaseDirectory;
            for (int i = 0; i < 10 && !string.IsNullOrEmpty(dir); i++)
            {
                string candidate = Path.Combine(dir, "Assets", leaf);
                if (Directory.Exists(candidate))
                    return Path.GetFullPath(candidate);
                dir = Directory.GetParent(dir)?.FullName;
            }

            throw new FileNotFoundException($"找不到 Assets/{leaf}。");
        }
    }
}
