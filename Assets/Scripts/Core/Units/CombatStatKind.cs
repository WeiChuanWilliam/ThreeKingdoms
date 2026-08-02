namespace ThreeKindoms.Core.Units
{
    /// <summary>部隊最終戰鬥屬性維度（攻／防／擊破／破城／策略／建設）。</summary>
    public enum CombatStatKind : byte
    {
        Attack = 0,
        Defense = 1,
        Jipo = 2,
        Gongcheng = 3,
        Strategy = 4,
        Construction = 5
    }
}
