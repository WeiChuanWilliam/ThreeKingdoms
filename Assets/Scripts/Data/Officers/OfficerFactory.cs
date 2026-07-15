using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>
    /// 從參數檔資料建立 <see cref="Officer"/> 執行時實例。
    /// 典型流程：<c>officers.json</c> → <see cref="OfficerDatabase.Defs"/> → <see cref="FromDef"/> → <see cref="OfficerDatabase.Officers"/>。
    /// 未來新增武將：在 JSON 加一筆 <see cref="OfficerDef"/>，再 materialize 即可。
    /// </summary>
    public static class OfficerFactory
    {
        /// <summary>將單筆 <see cref="OfficerDef"/>（來自 officers.json）組裝為執行時武將。</summary>
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
            o.SetBelong(def.belong, OfficerConfigUtil.GetDefaultLoyalty());
            o.SetBirthYear(def.birthYear);
            o.SetDeathYear(def.deathYear);
            o.SetTitle(def.title);
            o.SetGender(def.gender != 1);
            o.SetInjury((OfficerInjuryState)NumericUtil.ClampToTarget((int)def.injury, 0, 3));
            o.SetCompatibility(def.compatibility);
            o.SetTroopAptitude(def.troopAptitude);
            OfficerPersonalityLoader.ApplyFromIds(o, def.personalityIds, personalityDb);
            OfficerItemLoader.ApplyFromIds(o, def.itemIds);
            return o;
        }
    }
}
