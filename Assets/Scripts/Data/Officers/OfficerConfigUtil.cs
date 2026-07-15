using System;
using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core;
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

        public static short GetDefaultLoyalty() =>
            NumericUtil.ClampToTarget((short)GetInt("officer.loyalty.default", 75), (short)0, (short)100);

        public static short GetDefaultStamina() =>
            NumericUtil.ClampToTarget((short)GetInt("officer.stamina.default", 100), (short)0, short.MaxValue);

        public static byte GetDefaultCompatibility() =>
            (byte)NumericUtil.ClampToTarget(GetInt("officer.compatibility.default", 145), 0, 255);

        public static TroopAptitudeGrade GetDefaultAptitudeGrade() =>
            (TroopAptitudeGrade)NumericUtil.ClampToTarget(GetInt("officer.aptitude.default", 0), 0, 3);

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

            return token.ToLowerInvariant() switch
            {
                "infantry" or "foot" or "步" or "步兵" => Set(TroopType.Infantry, out troopType),
                "cavalry" or "horse" or "騎" or "騎兵" => Set(TroopType.Cavalry, out troopType),
                "archer" or "bow" or "弓" or "弓兵" => Set(TroopType.Archer, out troopType),
                "siege" or "器械" => Set(TroopType.Siege, out troopType),
                "navy" or "water" or "水" or "水軍" => Set(TroopType.Navy, out troopType),
                _ => false
            };

            static bool Set(TroopType value, out TroopType result)
            {
                result = value;
                return true;
            }
        }
    }
}
