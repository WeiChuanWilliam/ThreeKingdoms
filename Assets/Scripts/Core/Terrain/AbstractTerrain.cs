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

        /// <summary>地形顯示名稱。</summary>
        public string TerrainName => terrainName;

        /// <summary>地形定義 id。</summary>
        public int TerrainId => terrainId;

        /// <summary>可達、可建、可燃等旗標。</summary>
        public TerrainFlags TerrainFlags => terrainFlags;

        /// <summary>火計／續燃相關參數 n。</summary>
        public int FireEffect => fireEffect;

        /// <summary>進入或停留時士氣懲罰值。</summary>
        public int MoralePenalty => moralePenalty;

        /// <summary>對各兵種的移動、攻防與可達修正表。</summary>
        public TerrainEffectMaps EffectMaps => effectMaps;

        /// <summary>以 id 與名稱初始化地形定義。</summary>
        protected AbstractTerrain(int id, string name)
        {
            terrainId = id;
            terrainName = name ?? "";
            terrainFlags = new TerrainFlags { Reachable = true };
        }

        /// <summary>設定火計效果參數。</summary>
        public void SetFireEffect(int value) => fireEffect = value;

        /// <summary>設定士氣懲罰值。</summary>
        public void SetMoralePenalty(int value) => moralePenalty = value;

        /// <summary>設定地形旗標。</summary>
        public void SetFlags(TerrainFlags flags) => terrainFlags = flags;
    }
}
