using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Battle;

namespace ThreeKindoms.Data.Officers
{
    public static class OfficerFactory
    {
        public static Officer FromDef(OfficerDef def)
        {
            var o = new Officer(def.id);
            o.SetName(def.lastName, def.firstName, def.aliasName);
            o.SetBiography(def.biography);
            o.SetStats(def.attack, def.intelligence, def.leadership, def.policy, def.charisma);
            if (def.stamina != o.Stamina)
                o.StaminaChange((short)(def.stamina - o.Stamina));
            o.SetBirthYear(def.birthYear);
            o.SetAgeLimit(def.ageLimit);
            o.SetTitle(def.title);
            o.SetGender(def.gender == 1 ? OfficerGender.Female : OfficerGender.Male);
            o.SetBattleSkills(def.battleSkills);
            return o;
        }
    }
}
