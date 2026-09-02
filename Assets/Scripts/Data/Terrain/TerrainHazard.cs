namespace ThreeKindoms.Data.Terrain
{
    /// <summary>
    /// 地形／格子環境傷害強度（著火、陷阱、毒沼、棧道等共用 0～3 級）。
    /// 屬於地形與 Location，不是部隊狀態。
    /// </summary>
    public enum TerrainHazardLevel : byte
    {
        None = 0,
        Slight = 1,
        Medium = 2,
        Serious = 3
    }
}
