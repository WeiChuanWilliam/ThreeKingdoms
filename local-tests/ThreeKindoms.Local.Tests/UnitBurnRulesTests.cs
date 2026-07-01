using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class UnitBurnRulesTests
    {
        const string SkipReason = "SKELETON: 地圖火計／灼燒鏈尚未接線";

        [Fact(Skip = SkipReason)]
        public void TryCreateBurnContext_requires_tile_on_fire()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            Assert.False(UnitBurnRules.TryCreateBurnContext(combat, loc, out _));
        }

        [Fact(Skip = SkipReason)]
        public void TryCreateBurnContext_succeeds_when_burning_and_n_nonzero()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(2);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            combat.Location.SyncFireFromLocation();

            Assert.True(UnitBurnRules.IsBurning(combat));
            Assert.True(UnitBurnRules.TryCreateBurnContext(combat, loc, out var ctx));
            Assert.Equal(2, ctx.TerrainN);
            Assert.Equal(HazardDamageLevel.Medium, ctx.FlameLevel);
            Assert.Equal(1f, ctx.FireDamageFactor);
        }

        [Fact(Skip = SkipReason)]
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

            Assert.True(UnitBurnRules.IsBurning(combat));
            Assert.True(UnitBurnRules.IsImmuneToBurnDamage(combat));
            Assert.False(UnitBurnRules.TryCreateBurnContext(combat, loc, out _));
        }

        [Fact(Skip = SkipReason)]
        public void CalculateBurnDamage_applies_combat_factor_and_flame_level()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(2);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            combat.SetManpower(1000, 0);
            combat.Location.SyncFireFromLocation();
            UnitBurnRules.TryCreateBurnContext(combat, loc, out var ctx);

            var result = UnitBurnRules.CalculateBurnDamage(ctx);
            Assert.True(result.HasEffect);
            Assert.True(result.SoldierDeaths > 0);
            Assert.True(result.MoraleLoss > 0);
        }

        [Fact(Skip = SkipReason)]
        public void Transport_uses_lower_fire_damage_factor()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var transport = new Transport(new TransportUnitDef(1, soldiers: 1000));
            transport.Location.BindToWorld(grid, loc.Hex, terrain);
            transport.Location.SyncFireFromLocation();

            Assert.True(UnitBurnRules.TryCreateBurnContext(transport, loc, out var ctx));
            Assert.Equal(0.8f, ctx.FireDamageFactor);
        }

        [Fact(Skip = SkipReason)]
        public void TryApplyDailyBurnDamage_reduces_soldiers()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(2);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = CreateCombatOnLocation(grid, loc, terrain);
            combat.SetManpower(1000, 0);
            combat.Location.SyncFireFromLocation();

            Assert.True(UnitBurnRules.TryApplyDailyBurnDamage(combat, loc));
            Assert.True(combat.Soldiers < 1000);
        }

        static Combat CreateCombatOnLocation(LocationGrid grid, MapLocation loc, TerrainDefinition terrain)
        {
            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            combat.Location.BindToWorld(grid, loc.Hex, terrain);
            return combat;
        }
    }
}
