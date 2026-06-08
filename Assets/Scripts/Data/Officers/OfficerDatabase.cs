using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Data.Officers
{
    public sealed class OfficerDatabase
    {
        readonly Dictionary<int, OfficerDef> _defs = new();
        readonly Dictionary<int, Officer> _runtime = new();
        PersonalityDatabase _personalities = new();

        public IReadOnlyDictionary<int, OfficerDef> Defs => _defs;
        public PersonalityDatabase Personalities => _personalities;

        public static OfficerDatabase LoadFromFile(string officersJsonPath, string personalityJsonPath = null)
        {
            var db = new OfficerDatabase();
            if (!File.Exists(officersJsonPath))
                return db;

            var list = OfficerJsonSerializer.DeserializeOfficers(File.ReadAllText(officersJsonPath));
            if (list?.officers != null)
            {
                foreach (var def in list.officers)
                    db._defs[def.id] = def;
            }

            if (!string.IsNullOrEmpty(personalityJsonPath))
                db._personalities = PersonalityDatabase.LoadFromFile(personalityJsonPath);
            return db;
        }

        public Officer GetOrCreateRuntime(int defId)
        {
            if (_runtime.TryGetValue(defId, out var existing))
                return existing;

            if (!_defs.TryGetValue(defId, out var def))
                return null;

            var officer = OfficerFactory.FromDef(def, _personalities);
            _runtime[defId] = officer;
            return officer;
        }
    }
}
