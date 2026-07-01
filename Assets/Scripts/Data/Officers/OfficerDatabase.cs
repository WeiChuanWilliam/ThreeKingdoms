using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>
    /// 武將靜態資料庫：<see cref="Defs"/> 圖鑑（全遊戲）＋<see cref="Runtime"/> 本局 Pool（劇本／存檔）。
    /// 不需建立實例；兩池皆 static。
    /// </summary>
    public static class OfficerDatabase
    {
        static readonly Dictionary<int, OfficerDef> _defs = new();
        static readonly Dictionary<int, Officer> _runtime = new();
        static PersonalityDatabase _personalities = new();
        static bool _catalogLoaded;

        /// <summary>全遊戲武將定義（officers.json），劇本切換不變。</summary>
        public static IReadOnlyDictionary<int, OfficerDef> Defs => _defs;

        /// <summary>本局已 materialize 的武將 Pool（換劇本可 <see cref="ClearRuntime"/> 重建）。</summary>
        public static IReadOnlyDictionary<int, Officer> Runtime => _runtime;

        public static PersonalityDatabase Personalities => _personalities;

        public static bool IsCatalogLoaded => _catalogLoaded;

        public static bool IsRuntimeLoaded => _runtime.Count > 0;

        public static int RuntimeCount => _runtime.Count;

        /// <summary>載入全遊戲武將圖鑑與個性表（靜態，可重複呼叫以熱更新表）。</summary>
        public static void LoadCatalog(string officersJsonPath, string personalityJsonPath = null)
        {
            _defs.Clear();
            if (File.Exists(officersJsonPath))
            {
                var list = OfficerJsonSerializer.DeserializeOfficers(File.ReadAllText(officersJsonPath));
                if (list?.officers != null)
                {
                    foreach (var def in list.officers)
                        _defs[def.id] = def;
                }
            }

            _personalities = !string.IsNullOrEmpty(personalityJsonPath) && File.Exists(personalityJsonPath)
                ? PersonalityDatabase.LoadFromFile(personalityJsonPath)
                : new PersonalityDatabase();

            _catalogLoaded = true;
        }

        /// <summary>載入圖鑑並 materialize 圖鑑內全部武將到 runtime。</summary>
        public static void LoadCatalogAndMaterializeAll(string officersJsonPath, string personalityJsonPath = null)
        {
            LoadCatalog(officersJsonPath, personalityJsonPath);
            MaterializeAllFromCatalog();
        }

        /// <summary>丟棄本局 runtime（開新局或換劇本前）。</summary>
        public static void ClearRuntime() => _runtime.Clear();

        /// <summary>依 id 清單從 <see cref="Defs"/> 建立本局 Pool。</summary>
        public static void MaterializeFromIds(IEnumerable<int> officerIds)
        {
            ClearRuntime();
            if (officerIds == null)
                return;

            foreach (int defId in officerIds)
                TryCreateRuntime(defId);
        }

        /// <summary>從劇本武將清單 JSON（officerIds）建立本局 Pool。</summary>
        public static void MaterializeFromScenarioFile(string scenarioOfficersJsonPath)
        {
            MaterializeFromIds(ScenarioOfficerListLoader.LoadOfficerIds(scenarioOfficersJsonPath));
        }

        /// <summary>開發用：將圖鑑內全部武將 materialize 進 runtime。</summary>
        public static void MaterializeAllFromCatalog()
        {
            ClearRuntime();
            foreach (int defId in _defs.Keys)
                TryCreateRuntime(defId);
        }

        /// <summary>本局 Pool 查詢（不自動從圖鑑建立）。</summary>
        public static Officer TryGetRuntime(int defId) =>
            _runtime.TryGetValue(defId, out var officer) ? officer : null;

        /// <summary>從圖鑑建立一筆 runtime 並登記進本局 Pool。</summary>
        public static Officer TryCreateRuntime(int defId)
        {
            if (_runtime.TryGetValue(defId, out var existing))
                return existing;

            if (!_defs.TryGetValue(defId, out var def))
                return null;

            var officer = OfficerFactory.FromDef(def, _personalities);
            _runtime[defId] = officer;
            return officer;
        }

        public static void SyncAllRelations()
        {
            foreach (Officer officer in _runtime.Values)
                OfficerRelationsSync.EnsureSymmetric(officer, TryGetRuntime);
        }

        /// <summary>從本局 Pool 移除（如武將死亡）；不刪除圖鑑 Def。</summary>
        public static void RemoveFromRuntime(Officer officer)
        {
            if (officer != null)
                _runtime.Remove(officer.RuntimeId);
        }
    }
}
