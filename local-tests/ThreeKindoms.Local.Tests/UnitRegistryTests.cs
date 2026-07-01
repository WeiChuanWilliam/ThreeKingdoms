using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class UnitRegistryTests
    {
        const string SkipFireReason = "SKELETON: 日出灼燒結算尚未實作";

        [Fact]
        public void BindToWorld_registers_unit_and_exposes_hex()
        {
            UnitRegistry.Clear();

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Plain);
            var hex = new HexCoord(2, 3);
            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            combat.Location.BindToWorld(grid, hex, terrain);

            Assert.Equal(1, UnitRegistry.Count);
            Assert.True(UnitRegistry.Contains(combat));
            Assert.Equal(hex, combat.CurrentHex);
            Assert.True(combat.IsOnMap);
            Assert.NotNull(combat.CurrentMapLocation);
            Assert.Equal(hex, combat.CurrentMapLocation.Hex);
        }

        [Fact(Skip = SkipFireReason)]
        public void ApplyBurnDamage_scans_burning_cells_not_whole_map()
        {
            UnitRegistry.Clear();
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(4);
            var fireHex = new HexCoord(0, 0);
            var safeHex = new HexCoord(9, 9);
            grid.GetOrCreate(fireHex, terrain).SetOnFire();
            grid.GetOrCreate(safeHex, TerrainDefinition.FromTerrainType(TerrainType.Plain));

            var onFire = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            onFire.Location.BindToWorld(grid, fireHex, terrain);
            var safe = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            safe.Location.BindToWorld(grid, safeHex, TerrainDefinition.FromTerrainType(TerrainType.Plain));

            int before = onFire.Soldiers;
            grid.ApplyBurnDamageAtSunrise();

            Assert.True(onFire.Soldiers < before);
            Assert.Equal(1000, safe.Soldiers);
        }

        [Fact(Skip = SkipFireReason)]
        public void TickAllUnitsAtSunrise_syncs_fire_from_tile_without_scanning_all_cells()
        {
            UnitRegistry.Clear();

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            combat.Location.BindToWorld(grid, loc.Hex, terrain);

            loc.CountFire(roll0To99: 0);
            grid.TickAllUnitsAtSunrise();

            Assert.True(combat.IsOnFire);
            Assert.NotEqual(HazardDamageLevel.None, combat.FlameDamage);
        }
    }
}
