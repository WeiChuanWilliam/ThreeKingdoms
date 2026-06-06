namespace ThreeKindoms.Core.Units
{
    /// <summary>兵種表六圍（含攻擊距離），用於加成公式掛鉤。</summary>
    public enum CombatStatKind : byte
    {
        Attack = 0,
        Defense = 1,
        Mobility = 2,
        Jipo = 3,
        Gongcheng = 4,
        Stamina = 5,
        AttackRange = 6
    }
}
