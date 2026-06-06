using System;

namespace ThreeKindoms.Data.Terrain
{
    
    /// <summary>對應 C++ TerrainFlags 位元欄位。</summary>
    [Serializable]
    public struct TerrainFlags
    {
        public bool Reachable;
        public bool Construction;
        public bool Trap;
        public bool Fort;
        public bool Fireable;

        public byte Pack()
        {
            byte b = 0;
            if (Reachable) b |= 1 << 0;
            if (Construction) b |= 1 << 1;
            if (Trap) b |= 1 << 2;
            if (Fort) b |= 1 << 3;
            if (Fireable) b |= 1 << 4;
            return b;
        }

        public static TerrainFlags Unpack(byte b) => new()
        {
            Reachable = (b & (1 << 0)) != 0,
            Construction = (b & (1 << 1)) != 0,
            Trap = (b & (1 << 2)) != 0,
            Fort = (b & (1 << 3)) != 0,
            Fireable = (b & (1 << 4)) != 0
        };
    }
}
