using System;
using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Data.Scenario;

namespace ThreeKindoms.Data.Terrain
{
    /// <summary>
    /// 讀取 <c>chinese/terrain.properties</c>，組裝 <see cref="TerrainDefinition"/>。
    /// 地形類型表；不含格座標與建築。
    /// </summary>
    public static class TerrainConfigUtil
    {
        const string DefaultRelativePath = "chinese/terrain.properties";
        const string TypePrefix = "terrain.type.";
        const string IdSuffix = ".id";

        static Dictionary<string, string> entries = new();
        static List<string> typeKeys = new();
        static bool loaded;

        public static bool IsLoaded => loaded;
        public static string Locale { get; private set; } = "zh";

        public static int BaselineEnterCost => GetInt("terrain.movement.baseline_enter_cost", 15);
        public static int HazardMaxLevel => GetInt("terrain.hazard.max_level", 3);
        public static int WaterNavyEnterCostPerCell => GetInt("terrain.water.navy_enter_cost_per_cell", 4);

        public static IReadOnlyList<string> TerrainTypeKeys
        {
            get
            {
                lock (typeKeys)
                    return typeKeys.AsReadOnly();
            }
        }

        public static bool LoadDefault(string streamingAssetsRoot)
        {
            if (string.IsNullOrEmpty(streamingAssetsRoot))
                return LoadFromRelativePath(DefaultRelativePath);
            return Load(Path.Combine(streamingAssetsRoot, DefaultRelativePath));
        }

        public static bool LoadFromRelativePath(string relativePath)
        {
            string baseDir = Directory.GetCurrentDirectory();
            return Load(Path.Combine(baseDir, relativePath));
        }

        public static bool Load(string absolutePath)
        {
            if (!File.Exists(absolutePath))
            {
                loaded = false;
                typeKeys = new List<string>();
                return false;
            }

            lock (entries)
            {
                entries = PropertiesFile.LoadFromFile(absolutePath);
                loaded = true;
                typeKeys = ParseTypeKeys(entries);
            }

            Locale = Get("locale", "zh");
            return true;
        }

        public static string GetDisplayName(string typeKey) =>
            Get(TypePrefix + typeKey, typeKey);

        public static bool TryCreateDefinition(string typeKey, out TerrainDefinition definition)
        {
            definition = null;
            if (!loaded || string.IsNullOrEmpty(typeKey))
                return false;

            string prefix = TypePrefix + typeKey + ".";
            if (!TryGetInt(prefix + "id", out int id))
                return false;

            byte enterCost = (byte)NumericUtil.ClampToTarget(
                GetInt(prefix + "enter_cost", BaselineEnterCost), 0, byte.MaxValue);

            definition = new TerrainDefinition(id, GetDisplayName(typeKey), enterCost);
            definition.SetFlags(new TerrainFlags
            {
                Reachable = GetBool(prefix + "reachable", true),
                Construction = GetBool(prefix + "construction", false),
                Trap = GetBool(prefix + "trap", false),
                Fort = GetBool(prefix + "fort", false),
                Fireable = GetBool(prefix + "fireable", false)
            });
            definition.SetFireEffect(GetInt(prefix + "fire_effect", 0));
            definition.SetMoralePenalty(GetInt(prefix + "morale_penalty", 0));
            return true;
        }

        public static bool TryGetDefinitionById(int terrainId, out TerrainDefinition definition)
        {
            definition = null;
            foreach (string key in TerrainTypeKeys)
            {
                if (!TryGetInt(TypePrefix + key + IdSuffix, out int id) || id != terrainId)
                    continue;
                return TryCreateDefinition(key, out definition);
            }

            return false;
        }

        public static Dictionary<int, TerrainDefinition> LoadAllDefinitions()
        {
            var map = new Dictionary<int, TerrainDefinition>();
            foreach (string key in TerrainTypeKeys)
            {
                if (!TryCreateDefinition(key, out TerrainDefinition def) || def == null)
                    continue;
                map[def.TerrainId] = def;
            }

            return map;
        }

        static List<string> ParseTypeKeys(Dictionary<string, string> dict)
        {
            var keys = new HashSet<string>(StringComparer.Ordinal);
            foreach (string key in dict.Keys)
            {
                if (!key.StartsWith(TypePrefix, StringComparison.Ordinal) || !key.EndsWith(IdSuffix, StringComparison.Ordinal))
                    continue;

                string middle = key.Substring(TypePrefix.Length, key.Length - TypePrefix.Length - IdSuffix.Length);
                if (!string.IsNullOrEmpty(middle) && !middle.Contains('.'))
                    keys.Add(middle);
            }

            var list = new List<string>(keys);
            list.Sort(StringComparer.Ordinal);
            return list;
        }

        static string Get(string key, string defaultValue = "")
        {
            lock (entries)
                return entries.TryGetValue(key, out string v) ? v : defaultValue;
        }

        static int GetInt(string key, int defaultValue = 0) =>
            int.TryParse(Get(key), out int n) ? n : defaultValue;

        static bool TryGetInt(string key, out int value)
        {
            value = 0;
            if (!int.TryParse(Get(key), out value))
                return false;
            return true;
        }

        static bool GetBool(string key, bool defaultValue)
        {
            string raw = Get(key);
            if (string.IsNullOrEmpty(raw))
                return defaultValue;
            if (bool.TryParse(raw, out bool b))
                return b;
            return raw == "1";
        }
    }
}
