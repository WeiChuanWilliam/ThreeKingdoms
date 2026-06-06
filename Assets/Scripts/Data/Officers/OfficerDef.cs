using System;
using ThreeKindoms.Data.Battle;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>
    /// 武將表資料（JSON / ScriptableObject），用於建立 <see cref="Core.Officers.Officer"/> 實例。
    /// </summary>
    [Serializable]
    public class OfficerDef
    {
        public int id;
        public string lastName;
        public string firstName;
        public string aliasName;
        public string biography;
        public short attack;
        public short intelligence;
        public short leadership;
        public short policy;
        public short charisma;
        public short stamina = 100;
        public short birthYear;
        public short ageLimit = 99;
        public string title;
        public byte gender; // 0 male 1 female
        public int[] personalityIds;
        public OfficerBattleSkills battleSkills;
    }

    [Serializable]
    public class OfficerDefList
    {
        public OfficerDef[] officers;
    }
}
