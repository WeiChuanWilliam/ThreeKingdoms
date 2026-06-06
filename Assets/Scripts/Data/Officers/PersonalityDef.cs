using System;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>武將身上持有的個性（執行時槽位，指向表 id）。</summary>
    [Serializable]
    public struct PersonalityDef
    {
        public int Id;
        public string Key;
        public string DisplayName;
    }

    public enum PersonalityCategory : byte
    {
        Command = 0,   // 統御
        Combat = 1,    // 戰鬥
        Strategy = 2,  // 智略
        Politics = 3,  // 政治
        Special = 4    // 特殊
    }

    public enum PersonalityTier : byte
    {
        Purple = 0, // 紫（合成）
        Gold = 1,   // 金
        Blue = 2,   // 藍
        Red = 3     // 紅
    }

    /// <summary>武將個性表資料（<c>StreamingAssets/personality_traits.json</c>）。</summary>
    [Serializable]
    public class PersonalityTraitDef
    {
        public int id;
        public string key;
        public string name;
        public string category;
        public string tier;
        public string effect;
        public string[] composes;
    }

    [Serializable]
    public class PersonalityTraitList
    {
        public int version;
        public string sourceDoc;
        public string[] categoryOrder;
        public string[] tierOrder;
        public string composeSeparator;
        public int purplePerCategoryMax;
        public PersonalityTraitDef[] traits;
    }
}
