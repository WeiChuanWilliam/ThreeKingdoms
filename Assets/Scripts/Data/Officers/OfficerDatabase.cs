using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core.Officers;
using UnityEngine;

namespace ThreeKindoms.Data.Officers
{
    public sealed class OfficerDatabase
    {
        readonly Dictionary<int, OfficerDef> _defs = new();
        readonly Dictionary<int, Officer> _runtime = new();

        public IReadOnlyDictionary<int, OfficerDef> Defs => _defs;

        public static OfficerDatabase LoadFromStreamingAssets(string fileName = "officers.json")
        {
            var db = new OfficerDatabase();
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[OfficerDatabase] 找不到 {path}");
                return db;
            }

            var json = File.ReadAllText(path);
            var list = JsonUtility.FromJson<OfficerDefList>(json);
            if (list?.officers == null)
                return db;

            foreach (var def in list.officers)
                db._defs[def.id] = def;

            Debug.Log($"[OfficerDatabase] 已載入 {_defs.Count} 筆武將定義");
            return db;
        }

        public Officer GetOrCreateRuntime(int defId)
        {
            if (_runtime.TryGetValue(defId, out var existing))
                return existing;

            if (!_defs.TryGetValue(defId, out var def))
            {
                Debug.LogWarning($"[OfficerDatabase] 無 def id={defId}");
                return null;
            }

            var officer = OfficerFactory.FromDef(def);
            _runtime[defId] = officer;
            return officer;
        }
    }
}
