using System;

namespace ThreeKindoms.Data.Locations
{
    /// <summary>對應 C++ LocationFlag 位元欄位。</summary>
    [Serializable]
    public struct LocationFlags
    {
        public bool OnFire;
        /// <summary>陷阱格（當下是否有陷阱；能否設陷阱見地形 <c>TerrainFlags.Trap</c>）。</summary>
        public bool OnTrap;
        public bool OnDefence;
        public bool Built;
        public bool OnUnit;
        public bool Passable;
        public bool Joinable;

        public static LocationFlags DefaultPassable => new()
        {
            Passable = true,
            Joinable = false
        };

        public byte Pack()
        {
            byte b = 0;
            if (OnFire) b |= 1 << 0;
            if (OnDefence) b |= 1 << 1;
            if (Built) b |= 1 << 2;
            if (OnUnit) b |= 1 << 3;
            if (Passable) b |= 1 << 4;
            if (Joinable) b |= 1 << 5;
            if (OnTrap) b |= 1 << 6;
            return b;
        }

        public static LocationFlags Unpack(byte b) => new()
        {
            OnFire = (b & (1 << 0)) != 0,
            OnDefence = (b & (1 << 1)) != 0,
            Built = (b & (1 << 2)) != 0,
            OnUnit = (b & (1 << 3)) != 0,
            Passable = (b & (1 << 4)) != 0,
            Joinable = (b & (1 << 5)) != 0,
            OnTrap = (b & (1 << 6)) != 0
        };
    }
}
