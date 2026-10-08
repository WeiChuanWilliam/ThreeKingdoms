using System;

namespace ThreeKindoms.Data.Terrain
{
    /// <summary>地形類型「能不能」：可達、可建、可設陷阱、可築寨、可燃。格子當下狀態見 LocationFlags。</summary>
    [Serializable]
    public struct TerrainFlags
    {
        public bool Reachable;
        public bool Construction;
        public bool Trap;
        public bool Fort;
        public bool Fireable;
    }
}
