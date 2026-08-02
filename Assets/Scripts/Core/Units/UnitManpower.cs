namespace ThreeKindoms.Core.Units
{
    /// <summary>兵力與傷兵規則（全 Unit 共用）。</summary>
    public static class UnitManpower
    {
        /// <summary>低於此兵力視為殲滅（以總兵力 Soldiers 計）。</summary>
        public const int MinSoldiers = 10;

        /// <summary>
        /// 可作戰兵力：總兵 − 傷兵。傷兵仍屬部隊，但不計入戰鬥力／交戰人力。
        /// </summary>
        public static int FightingStrength(int soldiers, int wounded)
        {
            if (soldiers <= 0) return 0;
            wounded = System.Math.Min(System.Math.Max(0, wounded), soldiers);
            return soldiers - wounded;
        }

        /// <summary>同 <see cref="FightingStrength"/>（戰鬥／評分用人力）。</summary>
        public static int EffectiveCombatStrength(int soldiers, int wounded) =>
            FightingStrength(soldiers, wounded);

        /// <summary>兵力是否已低於殲滅門檻。</summary>
        public static bool IsAnnihilated(int soldiers) => soldiers < MinSoldiers;
    }
}
