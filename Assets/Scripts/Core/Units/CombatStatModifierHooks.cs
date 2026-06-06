using ThreeKindoms.Core.Terrain;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 部隊六圍加成掛鉤（武將戰法、科技、地勢）。預設 1；在此檔實作公式。
    /// </summary>
    public static class CombatStatModifierHooks
    {
        public static float GetOfficerSkillMultiplier(Unit unit, ICombatTroopStatsSource source, CombatStatKind stat) => 1f;

        public static float GetResearchMultiplier(Unit unit, ICombatTroopStatsSource source, CombatStatKind stat) => 1f;

        public static float GetTerrainMultiplier(
            Unit unit,
            ICombatTroopStatsSource source,
            AbstractTerrain terrain,
            CombatStatKind stat) => 1f;

        public static CombatTroopStatBlock ModifyByOfficerSkillAndResearch(
            Unit unit,
            ICombatTroopStatsSource source,
            CombatTroopStatBlock baseStats) =>
            CombatStatMath.ApplyOfficerAndResearchMultipliers(unit, source, baseStats);

        public static CombatTroopStatBlock ModifyByTerrain(
            Unit unit,
            ICombatTroopStatsSource source,
            CombatTroopStatBlock stats) =>
            CombatStatMath.ApplyTerrainMultipliers(unit, source, stats);
    }
}
