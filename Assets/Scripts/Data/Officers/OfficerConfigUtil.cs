using System;
using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Data.Scenario;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>讀取 <c>chinese/officer.properties</c>（武將系統預設與上限）。</summary>
    public static class OfficerConfigUtil
    {
        const string DefaultRelativePath = "chinese/officer.properties";
        const string SignatureTroopPrefix = "officer.signature_troop.";

        static Dictionary<string, string> entries = new();
        static bool loaded;
        static List<SignatureTroopRequirement> signatureTroops = new();

        public static bool IsLoaded => loaded;

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
                signatureTroops = new List<SignatureTroopRequirement>();
                return false;
            }

            lock (entries)
            {
                entries = PropertiesFile.LoadFromFile(absolutePath);
                loaded = true;
                signatureTroops = ParseSignatureTroops(entries);
            }
            return true;
        }

        public static string Get(string key, string defaultValue = "")
        {
            lock (entries)
                return entries.TryGetValue(key, out string v) ? v : defaultValue;
        }

        public static int GetInt(string key, int defaultValue = 0) =>
            int.TryParse(Get(key), out int n) ? n : defaultValue;

        public static short GetDefaultStamina() =>
            (short)Math.Clamp(GetInt("officer.stamina.default", 100), 0, short.MaxValue);

        public static short GetDefaultLifespan() =>
            (short)Math.Clamp(GetInt("officer.lifespan.default", 60), 1, short.MaxValue);

        public static byte GetDefaultCompatibility() =>
            (byte)Math.Clamp(GetInt("officer.compatibility.default", 145), 0, 255);

        public static TroopAptitudeGrade GetDefaultAptitudeGrade() =>
            (TroopAptitudeGrade)Math.Clamp(GetInt("officer.aptitude.default", 3), 0, 3);

        public static int GetMaxBelovedOfficers() => GetInt("officer.relations.beloved_max", 5);
        public static int GetMaxSwornBrothers() => GetInt("officer.relations.sworn_brother_max", 3);
        public static int GetMaxSpousesMale() => GetInt("officer.relations.spouse_max_male", 3);
        public static int GetMaxSpousesFemale() => GetInt("officer.relations.spouse_max_female", 1);

        public static int GetPersonalityGoldMax() => GetInt("officer.personality.gold_max", 3);
        public static int GetPersonalityBlueMax() => GetInt("officer.personality.blue_max", 3);
        public static int GetPersonalityRedMax() => GetInt("officer.personality.red_max", 2);
        public static int GetPersonalityPurpleMaxPerCategory() =>
            GetInt("officer.personality.purple_max_per_category", 1);
        public static int GetPersonalityTotalMax() => GetInt("officer.personality.total_max", 8);

        public static int GetMaxItems() => GetInt("officer.item.max_count", 12);

        public static IReadOnlyList<SignatureTroopRequirement> GetSignatureTroopRequirements()
        {
            if (!loaded)
                return Array.Empty<SignatureTroopRequirement>();
            return signatureTroops;
        }

        static List<SignatureTroopRequirement> ParseSignatureTroops(Dictionary<string, string> map)
        {
            var list = new List<SignatureTroopRequirement>();
            foreach (var kv in map)
            {
                if (!kv.Key.StartsWith(SignatureTroopPrefix, StringComparison.Ordinal))
                    continue;
                string kindKey = kv.Key.Substring(SignatureTroopPrefix.Length);
                if (string.IsNullOrWhiteSpace(kindKey))
                    continue;

                string[] parts = kv.Value.Split(',');
                if (parts.Length < 2)
                    continue;

                if (!TryParseTroopType(parts[0].Trim(), out TroopType troopType))
                    continue;

                list.Add(new SignatureTroopRequirement(kindKey, troopType, parts[1].Trim()));
            }
            list.Sort((a, b) => string.Compare(a.KindKey, b.KindKey, StringComparison.Ordinal));
            return list;
        }

        static bool TryParseTroopType(string token, out TroopType troopType)
        {
            troopType = TroopType.Infantry;
            if (string.IsNullOrWhiteSpace(token))
                return false;

            switch (token.ToLowerInvariant())
            {
                case "infantry":
                case "步兵":
                    troopType = TroopType.Infantry;
                    return true;
                case "cavalry":
                case "騎兵":
                    troopType = TroopType.Cavalry;
                    return true;
                case "archer":
                case "弓兵":
                    troopType = TroopType.Archer;
                    return true;
                case "siege":
                case "器械":
                    troopType = TroopType.Siege;
                    return true;
                case "navy":
                case "水軍":
                    troopType = TroopType.Navy;
                    return true;
                default:
                    return false;
            }
        }
    }
}
