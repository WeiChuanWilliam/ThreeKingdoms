namespace ThreeKindoms.Core.Units
{
    /// <summary>兵種六圍＋攻擊距離（初始值或加成後最終值）。</summary>
    public readonly struct CombatTroopStatBlock
    {
        /// <summary>攻擊。</summary>
        public short Attack { get; }

        /// <summary>防禦。</summary>
        public short Defense { get; }

        /// <summary>機動。</summary>
        public short Mobility { get; }

        /// <summary>破甲。</summary>
        public short Jipo { get; }

        /// <summary>攻城。</summary>
        public short Gongcheng { get; }

        /// <summary>耐力。</summary>
        public short Stamina { get; }

        /// <summary>攻擊距離。</summary>
        public short AttackRange { get; }

        /// <summary>以七項數值建立六圍區塊。</summary>
        public CombatTroopStatBlock(
            short attack,
            short defense,
            short mobility,
            short jipo,
            short gongcheng,
            short stamina,
            short attackRange)
        {
            Attack = attack;
            Defense = defense;
            Mobility = mobility;
            Jipo = jipo;
            Gongcheng = gongcheng;
            Stamina = stamina;
            AttackRange = attackRange;
        }

        /// <summary>依維度種類取得對應數值。</summary>
        public short Get(CombatStatKind kind) => kind switch
        {
            CombatStatKind.Attack => Attack,
            CombatStatKind.Defense => Defense,
            CombatStatKind.Mobility => Mobility,
            CombatStatKind.Jipo => Jipo,
            CombatStatKind.Gongcheng => Gongcheng,
            CombatStatKind.Stamina => Stamina,
            CombatStatKind.AttackRange => AttackRange,
            _ => 0
        };

        /// <summary>複製區塊並替換單一維度的數值。</summary>
        public CombatTroopStatBlock With(CombatStatKind kind, short value) => kind switch
        {
            CombatStatKind.Attack => new CombatTroopStatBlock(value, Defense, Mobility, Jipo, Gongcheng, Stamina, AttackRange),
            CombatStatKind.Defense => new CombatTroopStatBlock(Attack, value, Mobility, Jipo, Gongcheng, Stamina, AttackRange),
            CombatStatKind.Mobility => new CombatTroopStatBlock(Attack, Defense, value, Jipo, Gongcheng, Stamina, AttackRange),
            CombatStatKind.Jipo => new CombatTroopStatBlock(Attack, Defense, Mobility, value, Gongcheng, Stamina, AttackRange),
            CombatStatKind.Gongcheng => new CombatTroopStatBlock(Attack, Defense, Mobility, Jipo, value, Stamina, AttackRange),
            CombatStatKind.Stamina => new CombatTroopStatBlock(Attack, Defense, Mobility, Jipo, Gongcheng, value, AttackRange),
            CombatStatKind.AttackRange => new CombatTroopStatBlock(Attack, Defense, Mobility, Jipo, Gongcheng, Stamina, value),
            _ => this
        };
    }
}
