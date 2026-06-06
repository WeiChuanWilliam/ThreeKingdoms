using System;

namespace ThreeKindoms.Data.Terrain
{
    [Serializable]
    public class TerrainDef
    {
        public int id;
        public string terrainName;
        public byte flagsPacked;
        public int fireEffect;
        public int moralePenalty;
        public byte suggestedEnterCost = 15;
    }

    [Serializable]
    public class TerrainDefList
    {
        public TerrainDef[] terrains;
    }
}
