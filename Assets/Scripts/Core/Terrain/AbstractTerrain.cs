using ThreeKindoms.Data.Terrain;

namespace ThreeKindoms.Core.Terrain
{
    /// <summary>
    /// 對應 C++ Terrain::AbstractTerrain（地形類型定義，非單一格實例）。
    /// 六角格 <see cref="CellData"/> 可用 terrainId 指向此定義。
    /// </summary>
    public abstract class AbstractTerrain
    {
        protected string terrainName = "";
        protected int terrainId;
        protected TerrainFlags terrainFlags;
        protected int fireEffect;
        protected int moralePenalty;
        protected readonly TerrainEffectMaps effectMaps = new();

        public string TerrainName => terrainName;
        public int TerrainId => terrainId;
        public TerrainFlags TerrainFlags => terrainFlags;
        public int FireEffect => fireEffect;
        public int MoralePenalty => moralePenalty;
        public TerrainEffectMaps EffectMaps => effectMaps;

        protected AbstractTerrain(int id, string name)
        {
            terrainId = id;
            terrainName = name ?? "";
            terrainFlags = new TerrainFlags { Reachable = true };
        }

        public void SetFireEffect(int value) => fireEffect = value;
        public void SetMoralePenalty(int value) => moralePenalty = value;
        public void SetFlags(TerrainFlags flags) => terrainFlags = flags;
    }
}
