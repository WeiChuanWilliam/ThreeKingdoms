using ThreeKindoms.Core.Terrain;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 最終戰鬥屬性加成掛鉤（之後可依維度加倍率）。目前恒為 1。
    /// </summary>
    public static class CombatStatModifierHooks
    {
        public static float GetOfficerSkillMultiplier(Unit unit, CombatStatKind stat) => 1f;

        public static float GetResearchMultiplier(Unit unit, CombatStatKind stat) => 1f;

        public static float GetTerrainMultiplier(Unit unit, AbstractTerrain terrain, CombatStatKind stat) => 1f;
    }
}
