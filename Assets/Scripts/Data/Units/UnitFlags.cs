using System;

namespace ThreeKindoms.Data.Units
{
    /// <summary>火焰／陷阱等「格子上環境傷害」共用 0～3 級（格視同火焰傷害規則）。</summary>
    public enum HazardDamageLevel : byte
    {
        None = 0,
        Slight = 1,
        Medium = 2,
        Serious = 3
    }

    /// <summary>對應 C++ unitFlags。火焰與陷阱分開記，計算時可視為同類環境傷害。</summary>
    [Serializable]
    public struct UnitFlags
    {
        public bool Reachable;
        public HazardDamageLevel FlameDamage;
        public HazardDamageLevel TrapDamage;

        public static HazardDamageLevel ClampHazard(HazardDamageLevel value)
        {
            if ((byte)value > 3) return HazardDamageLevel.Serious;
            return value;
        }

        public static HazardDamageLevel FromByte(byte value) =>
            ClampHazard((HazardDamageLevel)value);

        public byte Pack()
        {
            byte b = 0;
            if (Reachable) b |= 1 << 0;
            b |= (byte)(((byte)ClampHazard(FlameDamage) & 0x3) << 1);
            b |= (byte)(((byte)ClampHazard(TrapDamage) & 0x3) << 3);
            return b;
        }

        public static UnitFlags Unpack(byte b) => new()
        {
            Reachable = (b & (1 << 0)) != 0,
            FlameDamage = FromByte((byte)((b >> 1) & 0x3)),
            TrapDamage = FromByte((byte)((b >> 3) & 0x3))
        };
    }
}
