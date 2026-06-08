using System;
using ThreeKindoms.Data.Battle;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>
    /// 武將表資料（JSON），用於建立 <see cref="Core.Officers.Officer"/> 實例。
    /// 姓名三欄：姓 lastName、名 firstName、字 aliasName。
    /// </summary>
    [Serializable]
    public class OfficerDef
    {
        public int id;

        public string lastName;
        public string firstName;
        public string aliasName;

        public string biography;
        public string tone;
        public string voice;
        public string picture;

        public short leadership;
        public short attack;
        public short intelligence;
        public short policy;
        public short charisma;
        public short stamina = 100;

        /// <summary>0 男、1 女（表資料）；執行時以 <see cref="OfficerGender"/> 表示。</summary>
        public byte gender;

        public short loyalty = 100;
        public short belong;

        /// <summary>傷病 0～3：正常／輕傷／重傷／死亡。</summary>
        public byte injury;

        /// <summary>相性 0～255。</summary>
        public byte compatibility = 145;

        public OfficerTroopAptitude troopAptitude;

        /// <summary>個性 id 集合；定義見 personality_traits.json。</summary>
        public int[] personalityIds;

        /// <summary>道具 id 集合（統一 Set，不細分欄位）。</summary>
        public int[] itemIds;

        public OfficerRelations relations;

        /// <summary>出生年（西元或劇情紀年，與劇本一致即可）。</summary>
        public short birthYear;

        /// <summary>壽命（年）；卒年＝出生年＋壽命。</summary>
        public short lifespan;

        /// <summary>相容舊欄位；若 lifespan 為 0 則 fallback 用此值。</summary>
        public short ageLimit;

        public string title;

        public OfficerBattleSkills battleSkills;
    }

    [Serializable]
    public class OfficerDefList
    {
        public OfficerDef[] officers;
    }
}
