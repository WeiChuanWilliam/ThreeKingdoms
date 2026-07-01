using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Locations;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class LocationFireRulesTests
    {
        const string SkipReason = "SKELETON: 地圖火計尚未實作";

        [Fact(Skip = SkipReason)]
        public void Ignition_is_100_when_n_nonzero_and_no_huoshen_unit()
        {
            Assert.Equal(0, LocationFireRules.ResolveIgnitionChancePercent(0));
            Assert.Equal(100, LocationFireRules.ResolveIgnitionChancePercent(1));
            Assert.Equal(100, LocationFireRules.ResolveIgnitionChancePercent(1, null));
        }

        [Fact(Skip = SkipReason)]
        public void Ignition_is_0_when_unit_has_huoshen()
        {
            Combat combat = CreateCombatWithTrait(LocationFireRules.TraitHuoshen);
            Assert.Equal(0, LocationFireRules.ResolveIgnitionChancePercent(1, combat));
        }

        [Fact(Skip = SkipReason)]
        public void Daily_burn_requires_no_miehuo_unit_and_n_times_25()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            Assert.Equal(25, LocationFireRules.ResolveDailyBurnContinuationChancePercent(1));
            Assert.True(LocationFireRules.EvaluateDailyBurnContinuation(1, roll0To99: 0));
            Assert.False(LocationFireRules.EvaluateDailyBurnContinuation(1, roll0To99: 99));

            Combat withMiehuo = CreateCombatWithTrait(LocationFireRules.TraitMiehuo);
            Assert.Equal(0, LocationFireRules.ResolveDailyBurnContinuationChancePercent(1, withMiehuo));
            Assert.False(LocationFireRules.EvaluateDailyBurnContinuation(1, roll0To99: 0, withMiehuo));
        }

        [Fact(Skip = SkipReason)]
        public void CountFire_at_sunrise_extinguishes_on_failed_roll()
        {
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = new MapLocation(new HexCoord(0, 0), terrain);
            loc.SetOnFire();
            loc.CountFire(roll0To99: 99);
            Assert.False(loc.LocationFlags.OnFire);
        }

        static Combat CreateCombatWithTrait(string traitName)
        {
            var def = new CombatUnitDef(1, "blade", soldiers: 1000);
            var combat = new Combat(def);
            var cmd = new Officer(1);
            cmd.AddPersonality(0, traitName, traitName);
            combat.SetCommander(cmd);
            return combat;
        }
    }
}
