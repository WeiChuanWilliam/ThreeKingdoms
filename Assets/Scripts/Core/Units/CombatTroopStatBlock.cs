namespace ThreeKindoms.Core.Units
{
    /// <summary>兵種六圍＋攻擊距離（初始值或加成後最終值）。</summary>
    public readonly struct CombatTroopStatBlock
    {
        public short Attack { get; }
        public short Defense { get; }
        public short Mobility { get; }
        public short Jipo { get; }
        public short Gongcheng { get; }
        public short Stamina { get; }
        public short AttackRange { get; }

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
