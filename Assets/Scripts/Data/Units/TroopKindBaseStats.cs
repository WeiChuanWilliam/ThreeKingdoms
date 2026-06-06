namespace ThreeKindoms.Data.Units
{
    /// <summary>兵種六圍（來自 unit.properties 的 troop.kind.*.attack 等）。</summary>
    public readonly struct TroopKindBaseStats
    {
        public short Attack { get; }
        public short Defense { get; }
        public short Mobility { get; }
        public short Jipo { get; }
        public short Gongcheng { get; }
        public short Stamina { get; }

        public TroopKindBaseStats(short attack, short defense, short mobility,
            short jipo, short gongcheng, short stamina)
        {
            Attack = attack;
            Defense = defense;
            Mobility = mobility;
            Jipo = jipo;
            Gongcheng = gongcheng;
            Stamina = stamina;
        }
    }
}
