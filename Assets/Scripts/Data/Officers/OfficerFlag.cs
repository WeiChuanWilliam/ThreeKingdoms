using System;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>對應 C++ OfficerFlag 位元欄位（C# 用明確欄位，存檔時可壓成 byte）。</summary>
    [Serializable]
    public struct OfficerFlag
    {
        public HealthLevel Health;
        public bool Death;
        public OfficerShowState Show;
        public bool Leader;
        public OfficerGender Gender;

        public byte Pack()
        {
            byte b = 0;
            b |= (byte)((int)Health & 0x3);
            if (Death) b |= 1 << 2;
            b |= (byte)(((int)Show & 0x3) << 3);
            if (Leader) b |= 1 << 5;
            if (Gender == OfficerGender.Female) b |= 1 << 6;
            return b;
        }

        public static OfficerFlag Unpack(byte b) => new()
        {
            Health = (HealthLevel)(b & 0x3),
            Death = (b & (1 << 2)) != 0,
            Show = (OfficerShowState)((b >> 3) & 0x3),
            Leader = (b & (1 << 5)) != 0,
            Gender = (b & (1 << 6)) != 0 ? OfficerGender.Female : OfficerGender.Male
        };
    }
}
