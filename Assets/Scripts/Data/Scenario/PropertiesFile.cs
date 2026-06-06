using System.Collections.Generic;
using System.IO;

namespace ThreeKindoms.Data.Scenario
{
    /// <summary>
    /// 簡易 .properties（key=value、# 註解），給人類手寫關卡／開局部隊位置用。
    /// </summary>
    public static class PropertiesFile
    {
        public static Dictionary<string, string> Parse(string text)
        {
            var map = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(text)) return map;

            using var reader = new StringReader(text);
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                line = line.Trim();
                if (line.Length == 0 || line.StartsWith("#"))
                    continue;

                int eq = line.IndexOf('=');
                if (eq <= 0) continue;

                string key = line.Substring(0, eq).Trim();
                string value = line.Substring(eq + 1).Trim();
                if (key.Length > 0)
                    map[key] = value;
            }

            return map;
        }

        public static Dictionary<string, string> LoadFromFile(string absolutePath)
        {
            if (!File.Exists(absolutePath))
                return new Dictionary<string, string>();
            return Parse(File.ReadAllText(absolutePath));
        }

        public static string Get(Dictionary<string, string> map, string key, string defaultValue = "") =>
            map.TryGetValue(key, out string v) ? v : defaultValue;

        public static int GetInt(Dictionary<string, string> map, string key, int defaultValue = 0) =>
            map.TryGetValue(key, out string v) && int.TryParse(v, out int n) ? n : defaultValue;
    }
}
