using System.Collections.Generic;
using System.IO;

namespace ThreeKindoms.Data.Officers
{
    public sealed class PersonalityDatabase
    {
        readonly Dictionary<int, PersonalityTraitDef> _byId = new();
        readonly Dictionary<string, PersonalityTraitDef> _byName = new();

        public IReadOnlyDictionary<int, PersonalityTraitDef> ById => _byId;
        public IReadOnlyDictionary<string, PersonalityTraitDef> ByName => _byName;

        public static PersonalityDatabase LoadFromFile(string absolutePath)
        {
            var db = new PersonalityDatabase();
            if (!File.Exists(absolutePath))
                return db;

            var list = OfficerJsonSerializer.DeserializePersonalities(File.ReadAllText(absolutePath));
            if (list?.traits == null)
                return db;

            foreach (var def in list.traits)
            {
                db._byId[def.id] = def;
                if (!string.IsNullOrEmpty(def.name))
                    db._byName[def.name] = def;
            }

            return db;
        }

        public PersonalityTraitDef GetById(int id) =>
            _byId.TryGetValue(id, out var def) ? def : null;

        public PersonalityTraitDef GetByName(string name) =>
            !string.IsNullOrEmpty(name) && _byName.TryGetValue(name, out var def) ? def : null;
    }
}
