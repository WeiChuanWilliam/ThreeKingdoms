using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>
    /// 格子著火／續燃／蔓延規則。
    /// <para>SKELETON：僅保留方法簽名與用途說明；公式與個性閘待設計定稿後實作。</para>
    /// </summary>
    public static class LocationFireRules
    {
        public const string TraitHuoshen = "火神";
        public const string TraitMiehuo = "滅火";

        /// <summary>讀取地形火計參數 n（欄位待定）。</summary>
        public static int GetTerrainN(AbstractTerrain terrain) => 0;

        /// <summary>著火機率（%）：依 n 與駐軍個性（火神）判定。</summary>
        public static int ResolveIgnitionChancePercent(int terrainN, Unit occupyingUnit = null) => 0;

        /// <summary>是否可能著火。</summary>
        public static bool CanIgnite(int terrainN, Unit occupyingUnit = null) => false;

        /// <summary>每日續燃機率（%）：依 n、step 與駐軍個性（滅火）判定。</summary>
        public static int ResolveDailyBurnContinuationChancePercent(int terrainN, Unit occupyingUnit = null) => 0;

        /// <summary>日出續燃：roll 與機率比較。</summary>
        public static bool EvaluateDailyBurnContinuation(int terrainN, int roll0To99, Unit occupyingUnit = null) => false;

        /// <summary>著火部隊閘：無部隊或無火神。</summary>
        public static bool PassesIgnitionUnitGate(Unit occupyingUnit) => true;

        /// <summary>續燃部隊閘：無部隊或無滅火。</summary>
        public static bool PassesBurnContinuationUnitGate(Unit occupyingUnit) => true;

        /// <summary>部隊主／副將是否持有指定個性。</summary>
        public static bool UnitHasPersonalityTrait(Unit unit, string traitName) => false;

        /// <summary>日出：對著火格做續燃判定，失敗則熄滅。</summary>
        public static void TickDailyBurnAtSunrise(MapLocation location, int roll0To99) { }

        /// <summary>鄰格火勢蔓延。</summary>
        public static void TrySpreadFireToAdjacentTiles(MapLocation source, LocationGrid grid, System.Random rng) { }
    }
}
