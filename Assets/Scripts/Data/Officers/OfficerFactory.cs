using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Battle;

namespace ThreeKindoms.Data.Officers
{
    public static class OfficerFactory
    {
        public static Officer FromDef(OfficerDef def, PersonalityDatabase personalityDb = null)
        {
            var o = new Officer(def.id);
            o.SetName(def.lastName, def.firstName, def.aliasName);
            o.SetPresentation(def.tone, def.voice, def.picture);
            o.SetBiography(def.biography);
            o.SetStats(
                def.leadership,
                def.attack,
                def.intelligence,
                def.policy,
                def.charisma,
                def.stamina > 0 ? def.stamina : OfficerConfigUtil.GetDefaultStamina());
            o.SetBelong(def.belong, def.loyalty);
            o.SetBirthYear(def.birthYear);
            o.SetLifespan(ResolveLifespan(def));
            o.SetTitle(def.title);
            o.SetGender(def.gender != 1);
            o.SetInjury((OfficerInjuryState)System.Math.Clamp((int)def.injury, 0, 3));
            o.SetCompatibility(def.compatibility);
            o.SetTroopAptitude(def.troopAptitude);
            o.SetBattleSkills(def.battleSkills);
            o.SetRelations(def.relations);
            OfficerPersonalityLoader.ApplyFromIds(o, def.personalityIds, personalityDb);
            OfficerItemLoader.ApplyFromIds(o, def.itemIds);
            return o;
        }

        static short ResolveLifespan(OfficerDef def)
        {
            if (def.lifespan > 0)
                return def.lifespan;
            if (def.ageLimit > 0)
                return def.ageLimit;
            return OfficerConfigUtil.IsLoaded
                ? OfficerConfigUtil.GetDefaultLifespan()
                : (short)60;
        }
    }
}
