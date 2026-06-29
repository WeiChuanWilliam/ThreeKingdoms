namespace ThreeKindoms.Core.Units
{
    /// <summary>兵力與傷兵換算（全 Unit 共用規則）。</summary>
    public static class UnitManpower
    {
        /// <summary>低於此兵力視為殲滅。</summary>
        public const int MinSoldiers = 10;

        /// <summary>傷兵戰鬥力係數（0.5 → 500 傷兵 ≈ 250 有效兵力）。</summary>
        public const float WoundedCombatFactor = 0.5f;

        /// <summary>
        /// 有效戰鬥人力：healthy + wounded×0.5。
        /// 例：1000 人、500 傷兵 → 500 + 250 = 750。
        /// </summary>
        public static int EffectiveCombatStrength(int soldiers, int wounded)
        {
            if (soldiers <= 0) return 0;
            wounded = System.Math.Min(wounded, soldiers);
            int healthy = soldiers - wounded;
            return healthy + (int)(wounded * WoundedCombatFactor);
        }

        /// <summary>兵力是否已低於殲滅門檻。</summary>
        public static bool IsAnnihilated(int soldiers) => soldiers < MinSoldiers;
    }
}
