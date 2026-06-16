using System;



namespace ThreeKindoms.Data.Officers

{

    /// <summary>武將狀態旗標（存檔可壓成 byte）。所屬勢力見 <see cref="Core.Officers.AbstractOfficer.Belong"/>。</summary>

    [Serializable]

    public struct OfficerFlag

    {

        public OfficerInjuryState Injury;



        public OfficerShowState Show;



        public OfficerGender Gender;



        /// <summary>true＝存活；false＝死亡（與傷勢程度分離）。</summary>

        public bool IsAlive;



        public bool Death => !IsAlive;



        public byte Pack()

        {

            byte b = 0;

            b |= (byte)((int)Injury & 0x3);

            b |= (byte)(((int)Show & 0x3) << 3);

            if (!IsAlive) b |= 1 << 5;

            if (Gender == OfficerGender.Female) b |= 1 << 6;

            return b;

        }



        public static OfficerFlag Unpack(byte b) => new()

        {

            Injury = (OfficerInjuryState)(b & 0x3),

            Show = (OfficerShowState)((b >> 3) & 0x3),

            IsAlive = (b & (1 << 5)) == 0,

            Gender = (b & (1 << 6)) != 0 ? OfficerGender.Female : OfficerGender.Male

        };

    }

}

