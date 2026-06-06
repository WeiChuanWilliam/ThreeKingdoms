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
    public class CombatBurnRulesTests
    {
        [Fact]
        public void TryCreateBurnContext_requires_tile_on_fire()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            // 未起火

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            Assert.False(CombatBurnRules.TryCreateBurnContext(combat, loc, out _));
        }

        [Fact]
        public void TryCreateBurnContext_succeeds_when_burning_and_n_nonzero()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(2);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            combat.Location.SyncFireFromLocation();

            Assert.True(CombatBurnRules.IsBurning(combat));
            Assert.True(CombatBurnRules.TryCreateBurnContext(combat, loc, out var ctx));
            Assert.Equal(2, ctx.TerrainN);
            Assert.Equal(HazardDamageLevel.Medium, ctx.FlameLevel);
        }

        [Fact]
        public void Huoshen_trait_blocks_burn_context()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            var cmd = new Officer(1);
            cmd.AddPersonality(0, LocationFireRules.TraitHuoshen, LocationFireRules.TraitHuoshen);
            combat.SetCommander(cmd);
            combat.Location.SyncFireFromLocation();

            Assert.True(CombatBurnRules.IsBurning(combat));
            Assert.True(CombatBurnRules.IsImmuneToBurnDamage(combat));
            Assert.False(CombatBurnRules.TryCreateBurnContext(combat, loc, out _));
        }

        [Fact]
        public void CalculateBurnDamage_is_stub_for_now()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            combat.Location.SyncFireFromLocation();
            CombatBurnRules.TryCreateBurnContext(combat, loc, out var ctx);

            var result = CombatBurnRules.CalculateBurnDamage(ctx);
            Assert.False(result.HasEffect);
        }

        static Combat CreateCombatOnLocation(LocationGrid grid, MapLocation loc, TerrainDefinition terrain)
        {
            var combat = new Combat(new CombatUnitDef(1, "infantry", soldiers: 1000));
            combat.Location.BindToWorld(grid, loc.Hex, terrain);
            return combat;
        }
    }
}
