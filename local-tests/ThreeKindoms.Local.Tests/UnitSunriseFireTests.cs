using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Locations;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class UnitSunriseFireTests
    {
        const string SkipReason = "SKELETON: 日出火計結算尚未實作";

        [Fact(Skip = SkipReason)]
        public void Unit_syncs_fire_from_tile_after_location_continues_burning()
        {
            UnitRegistry.Clear();
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            combat.Location.BindToWorld(grid, loc.Hex, terrain);

            loc.CountFire(roll0To99: 0);
            Assert.True(loc.LocationFlags.OnFire);

            grid.TickAllUnitsAtSunrise();
            Assert.True(combat.IsOnFire);
            Assert.NotEqual(HazardDamageLevel.None, combat.FlameDamage);
            Assert.True(combat.FireEffect > 0);
        }

        [Fact(Skip = SkipReason)]
        public void Unit_clears_fire_when_tile_extinguished_at_sunrise()
        {
            UnitRegistry.Clear();
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            combat.Location.BindToWorld(grid, loc.Hex, terrain);

            loc.CountFire(roll0To99: 99);
            Assert.False(loc.LocationFlags.OnFire);

            grid.TickAllUnitsAtSunrise();
            Assert.False(combat.IsOnFire);
            Assert.Equal(HazardDamageLevel.None, combat.FlameDamage);
        }
    }
}
