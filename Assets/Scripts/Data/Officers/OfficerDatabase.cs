using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>
    /// 武將資料庫：兩個池、同一 defId。
    /// <list type="bullet">
    /// <item><see cref="Defs"/> — 表資料（<see cref="OfficerDef"/>），唯讀圖鑑，含六維原始值。</item>
    /// <item><see cref="Officers"/> — 執行時池（<see cref="Officer"/>），局內狀態在此修改。</item>
    /// </list>
    /// </summary>
    public static class OfficerDatabase
    {
        static readonly Dictionary<int, OfficerDef> _defs = new();
        static readonly Dictionary<int, Officer> _officers = new();
        static PersonalityDatabase _personalities = new();
        static bool _loaded;

        /// <summary>表資料池（id → <see cref="OfficerDef"/>）。</summary>
        public static IReadOnlyDictionary<int, OfficerDef> Defs => _defs;

        /// <summary>執行時武將池（id → <see cref="Officer"/>）。</summary>
        public static IReadOnlyDictionary<int, Officer> Officers => _officers;

        public static PersonalityDatabase Personalities => _personalities;

        public static bool IsLoaded => _loaded;

        public static int DefCount => _defs.Count;

        public static int Count => _officers.Count;

        /// <summary>自 officers.json 載入表資料，並 materialize 全部武將至執行時池。</summary>
        public static void Load(string officersJsonPath, string personalityJsonPath = null)
        {
            _defs.Clear();
            _officers.Clear();
            _personalities = !string.IsNullOrEmpty(personalityJsonPath) && File.Exists(personalityJsonPath)
                ? PersonalityDatabase.LoadFromFile(personalityJsonPath)
                : new PersonalityDatabase();

            if (File.Exists(officersJsonPath))
            {
                OfficerDefList list = OfficerJsonSerializer.DeserializeOfficers(File.ReadAllText(officersJsonPath));
                if (list?.officers != null)
                {
                    foreach (OfficerDef def in list.officers)
                        _defs[def.id] = def;
                }
            }

            MaterializeAll();
            _loaded = _defs.Count > 0;
        }

        /// <summary>依目前 <see cref="Defs"/> 重建執行時池（開局篩選後可改為只 materialize 存活者）。</summary>
        public static void MaterializeAll()
        {
            _officers.Clear();
            foreach (KeyValuePair<int, OfficerDef> kv in _defs)
                _officers[kv.Key] = OfficerFactory.FromDef(kv.Value, _personalities);
        }

        /// <summary>將單筆表資料建立為執行時武將並加入池（若已存在則覆蓋）。</summary>
        public static Officer Materialize(int defId)
        {
            if (!_defs.TryGetValue(defId, out OfficerDef def))
                return null;

            Officer officer = OfficerFactory.FromDef(def, _personalities);
            _officers[defId] = officer;
            return officer;
        }

        /// <summary>表資料池查詢（六維原始值等）。</summary>
        public static OfficerDef TryGetDef(int defId) =>
            _defs.TryGetValue(defId, out OfficerDef def) ? def : null;

        /// <summary>執行時池查詢。</summary>
        public static Officer TryGet(int defId) =>
            _officers.TryGetValue(defId, out Officer officer) ? officer : null;

        /// <summary>自執行時池移除（表資料 <see cref="Defs"/> 保留）。</summary>
        public static void Remove(Officer officer)
        {
            if (officer != null)
                _officers.Remove(officer.RuntimeId);
        }

        public static void Clear()
        {
            _defs.Clear();
            _officers.Clear();
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
