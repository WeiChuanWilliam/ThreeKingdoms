using System;



namespace ThreeKindoms.Data.Officers

{

    /// <summary>武將身上持有的個性（以 <see cref="Id"/> 為 Set 唯一鍵）。</summary>

    [Serializable]

    public struct PersonalityDef : IEquatable<PersonalityDef>

    {

        public int Id;

        public string Key;

        public string DisplayName;



        public bool Equals(PersonalityDef other) => Id == other.Id;



        public override bool Equals(object obj) => obj is PersonalityDef other && Equals(other);



        public override int GetHashCode() => Id;



        public static bool operator ==(PersonalityDef left, PersonalityDef right) => left.Equals(right);



        public static bool operator !=(PersonalityDef left, PersonalityDef right) => !left.Equals(right);

    }



    public enum PersonalityCategory : byte

    {

        Command = 0,

        Combat = 1,

        Strategy = 2,

        Politics = 3,

        Special = 4

    }



    public enum PersonalityTier : byte

    {

        Purple = 0,

        Gold = 1,

        Blue = 2,

        Red = 3

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

