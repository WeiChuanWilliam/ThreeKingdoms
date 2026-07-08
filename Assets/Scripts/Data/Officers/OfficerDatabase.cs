using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>
    /// 試玩版武將池：自 <c>officers.json</c> 一次載入本劇本全部武將（單一 Dictionary）。
    /// </summary>
    public static class OfficerDatabase
    {
        static readonly Dictionary<int, Officer> _pool = new();
        static PersonalityDatabase _personalities = new();
        static bool _loaded;

        /// <summary>本劇本武將 Pool（id → 執行時 <see cref="Officer"/>）。</summary>
        public static IReadOnlyDictionary<int, Officer> Officers => _pool;

        public static PersonalityDatabase Personalities => _personalities;

        public static bool IsLoaded => _loaded;

        public static int Count => _pool.Count;

        /// <summary>自 officers.json 載入並建立全部武將實例。</summary>
        public static void Load(string officersJsonPath, string personalityJsonPath = null)
        {
            _pool.Clear();
            _personalities = !string.IsNullOrEmpty(personalityJsonPath) && File.Exists(personalityJsonPath)
                ? PersonalityDatabase.LoadFromFile(personalityJsonPath)
                : new PersonalityDatabase();

            if (File.Exists(officersJsonPath))
            {
                OfficerDefList list = OfficerJsonSerializer.DeserializeOfficers(File.ReadAllText(officersJsonPath));
                if (list?.officers != null)
                {
                    foreach (OfficerDef def in list.officers)
                        _pool[def.id] = OfficerFactory.FromDef(def, _personalities);
                }
            }

            _loaded = _pool.Count > 0;
        }

        public static Officer TryGet(int defId) =>
            _pool.TryGetValue(defId, out Officer officer) ? officer : null;

        public static void Remove(Officer officer)
        {
            if (officer != null)
                _pool.Remove(officer.RuntimeId);
        }

        public static void Clear()
        {
            _pool.Clear();
            _loaded = false;
        }

        // 相容舊呼叫端
        public static IReadOnlyDictionary<int, Officer> Runtime => Officers;
        public static bool IsRuntimeLoaded => IsLoaded;
        public static int RuntimeCount => Count;
        public static Officer TryGetRuntime(int defId) => TryGet(defId);
        public static void RemoveFromRuntime(Officer officer) => Remove(officer);
    }
}
