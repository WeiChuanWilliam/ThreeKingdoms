namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊最終戰鬥屬性（由 <see cref="CombatStatMath"/> 寫入部隊）。
    /// 攻／防／擊破／破城／策略／建設。
    /// </summary>
    public readonly struct CombatTroopStatBlock
    {
        public static CombatTroopStatBlock Zero => default;

        /// <summary>攻擊。</summary>
        public int Attack { get; }

        /// <summary>防禦。</summary>
        public int Defense { get; }

        /// <summary>擊破。</summary>
        public int Jipo { get; }

        /// <summary>破城。</summary>
        public int Gongcheng { get; }

        /// <summary>策略。</summary>
        public int Strategy { get; }

        /// <summary>建設。</summary>
        public int Construction { get; }

        public CombatTroopStatBlock(
            int attack,
            int defense,
            int jipo,
            int gongcheng,
            int strategy,
            int construction)
        {
            Attack = Clamp(attack);
            Defense = Clamp(defense);
            Jipo = Clamp(jipo);
            Gongcheng = Clamp(gongcheng);
            Strategy = Clamp(strategy);
            Construction = Clamp(construction);
        }

        static int Clamp(int v) => v < 0 ? 0 : v;
    }
}
