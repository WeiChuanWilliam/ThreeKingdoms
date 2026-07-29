using System;
using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Data.Scenario;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Data.Skill
{
    /// <summary>
    /// 讀取 <c>chinese/skill.properties</c>（多語言文案：顯示名；說明見 <c>.desc</c> 鍵）。
    /// 戰法行為寫死在 code；本 util 不驅動結算。B／A 共用、S 特色；器械每種 1 招。
    /// </summary>
    public static class SkillConfigUtil
    {
        const string DefaultRelativePath = "chinese/skill.properties";

        static Dictionary<string, string> entries = new();
        static bool loaded;

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
                return false;
            }

            lock (entries)
            {
                entries = PropertiesFile.LoadFromFile(absolutePath);
                loaded = true;
            }

            return true;
        }

        public static string Get(string key, string defaultValue = "")
        {
            lock (entries)
                return entries.TryGetValue(key, out string v) ? v : defaultValue;
        }

        /// <summary>
        /// 依兵種取得戰法名稱：兵系 B、兵系 A、特色兵種 S。
        /// 例：horseman.baima → [強襲, 衝鋒, 騎射]。
        /// 器械只回傳該器械的一個 S 戰法。
        /// </summary>
        public static IReadOnlyList<string> GetSkillNamesForTroopKind(string troopKindKey)
        {
            if (string.IsNullOrWhiteSpace(troopKindKey) || !loaded)
                return Array.Empty<string>();

            if (!TroopKindTree.TryGetNode(troopKindKey, out TroopKindNode node))
                return Array.Empty<string>();

            string branch = BranchKey(node.TroopType);
            string signature = SignatureKey(troopKindKey);
            var names = new List<string>(3);

            if (node.TroopType == TroopType.Siege)
            {
                AddIfPresent(names, Get($"skill.{branch}.S.{signature}"));
                return names;
            }

            AddIfPresent(names, Get($"skill.{branch}.B"));
            AddIfPresent(names, Get($"skill.{branch}.A"));
            AddIfPresent(names, Get($"skill.{branch}.S.{signature}"));
            return names;
        }

        /// <summary>取得指定兵系與適性（B／A）的共用戰法名稱。</summary>
        public static string GetSharedSkillName(TroopType troopType, string aptitude)
        {
            string grade = (aptitude ?? "").Trim().ToUpperInvariant();
            return grade is "B" or "A"
                ? Get($"skill.{BranchKey(troopType)}.{grade}")
                : "";
        }

        /// <summary>取得具體特色兵種的 S 級專有戰法名稱。</summary>
        public static string GetSignatureSkillName(string troopKindKey)
        {
            if (!TroopKindTree.TryGetNode(troopKindKey, out TroopKindNode node))
                return "";
            return Get($"skill.{BranchKey(node.TroopType)}.S.{SignatureKey(troopKindKey)}");
        }

        static string BranchKey(TroopType type) => type switch
        {
            TroopType.Infantry => "infantry",
            TroopType.Cavalry => "cavalry",
            TroopType.Archer => "archary",
            TroopType.Siege => "siege",
            TroopType.Navy => "navy",
            _ => "infantry"
        };

        static string SignatureKey(string troopKindKey)
        {
            int dot = troopKindKey.LastIndexOf('.');
            return dot >= 0 ? troopKindKey.Substring(dot + 1) : troopKindKey;
        }

        static void AddIfPresent(List<string> target, string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
                target.Add(value);
        }
    }
}
