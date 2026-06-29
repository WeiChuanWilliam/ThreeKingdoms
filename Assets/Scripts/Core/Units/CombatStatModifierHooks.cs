using ThreeKindoms.Core.Terrain;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊六圍加成掛鉤（武將戰法、科技、地勢）。預設 1；在此檔實作公式。
    /// </summary>
    public static class CombatStatModifierHooks
    {
        /// <summary>武將戰法對指定六圍的倍率（預設 1）。</summary>
        public static float GetOfficerSkillMultiplier(Unit unit, ICombatTroopStatsSource source, CombatStatKind stat) => 1f;

        /// <summary>科技對指定六圍的倍率（預設 1）。</summary>
        public static float GetResearchMultiplier(Unit unit, ICombatTroopStatsSource source, CombatStatKind stat) => 1f;

        /// <summary>地勢對指定六圍的倍率（預設 1）。</summary>
        public static float GetTerrainMultiplier(
            Unit unit,
            ICombatTroopStatsSource source,
            AbstractTerrain terrain,
            CombatStatKind stat) => 1f;

        /// <summary>依武將戰法與科技修正基礎六圍。</summary>
        public static CombatTroopStatBlock ModifyByOfficerSkillAndResearch(
            Unit unit,
            ICombatTroopStatsSource source,
            CombatTroopStatBlock baseStats) =>
            CombatStatMath.ApplyOfficerAndResearchMultipliers(unit, source, baseStats);

        /// <summary>依地勢修正六圍。</summary>
        public static CombatTroopStatBlock ModifyByTerrain(
            Unit unit,
            ICombatTroopStatsSource source,
            CombatTroopStatBlock stats) =>
            CombatStatMath.ApplyTerrainMultipliers(unit, source, stats);
    }
}
