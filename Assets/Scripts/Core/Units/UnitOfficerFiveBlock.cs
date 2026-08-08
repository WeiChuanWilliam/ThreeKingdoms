namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊統合五維（統／武／智／政／魅）：主副將 Effective* 合成後寫回部隊。
    /// 武將狀態變了就更新本物件；與 <see cref="CombatTroopStatBlock"/>（戰鬥力）分開。
    /// </summary>
    public readonly struct UnitOfficerFiveBlock
    {
        public static UnitOfficerFiveBlock Zero => default;

        /// <summary>統率。</summary>
        public short Leadership { get; }

        /// <summary>武力。</summary>
        public short Force { get; }

        /// <summary>智力。</summary>
        public short Intelligence { get; }

        /// <summary>政治。</summary>
        public short Policy { get; }

        /// <summary>魅力。</summary>
        public short Charisma { get; }

        public UnitOfficerFiveBlock(
            short leadership,
            short force,
            short intelligence,
            short policy,
            short charisma)
        {
            Leadership = Clamp(leadership);
            Force = Clamp(force);
            Intelligence = Clamp(intelligence);
            Policy = Clamp(policy);
            Charisma = Clamp(charisma);
        }

        static short Clamp(short v) => v < 0 ? (short)0 : v;
    }
}
