using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ThreeKindoms.Data.Officers
{
    public sealed class PersonalityDatabase
    {
        readonly Dictionary<int, PersonalityTraitDef> _byId = new();
        readonly Dictionary<string, PersonalityTraitDef> _byName = new();

        public IReadOnlyDictionary<int, PersonalityTraitDef> ById => _byId;
        public IReadOnlyDictionary<string, PersonalityTraitDef> ByName => _byName;

        public static PersonalityDatabase LoadFromStreamingAssets(string fileName = "personality_traits.json")
        {
            var db = new PersonalityDatabase();
            string path = Path.Combine(Application.streamingAssetsPath, fileName);
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[PersonalityDatabase] 找不到 {path}");
                return db;
            }

            var json = File.ReadAllText(path);
            var list = JsonUtility.FromJson<PersonalityTraitList>(json);
            if (list?.traits == null)
                return db;

            foreach (var def in list.traits)
            {
                db._byId[def.id] = def;
                if (!string.IsNullOrEmpty(def.name))
                    db._byName[def.name] = def;
            }

            Debug.Log($"[PersonalityDatabase] 已載入 {db._byId.Count} 筆個性定義");
            return db;
        }

        public PersonalityTraitDef GetById(int id) =>
            _byId.TryGetValue(id, out var def) ? def : null;

        public PersonalityTraitDef GetByName(string name) =>
            !string.IsNullOrEmpty(name) && _byName.TryGetValue(name, out var def) ? def : null;
    }
}
