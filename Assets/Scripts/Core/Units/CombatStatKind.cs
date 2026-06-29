namespace ThreeKindoms.Core.Units
{
    /// <summary>兵種表六圍（含攻擊距離），用於加成公式掛鉤。</summary>
    public enum CombatStatKind : byte
    {
        /// <summary>攻擊。</summary>
        Attack = 0,

        /// <summary>防禦。</summary>
        Defense = 1,

        /// <summary>機動。</summary>
        Mobility = 2,

        /// <summary>破甲。</summary>
        Jipo = 3,

        /// <summary>攻城。</summary>
        Gongcheng = 4,

        /// <summary>耐力。</summary>
        Stamina = 5,

        /// <summary>攻擊距離。</summary>
        AttackRange = 6
    }
}
