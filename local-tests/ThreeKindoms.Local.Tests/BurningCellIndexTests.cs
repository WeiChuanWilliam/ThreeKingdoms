using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class BurningCellIndexTests
    {
        const string SkipFireReason = "SKELETON: 日出火計結算尚未實作";

        [Fact]
        public void SetOnFire_registers_hex_in_grid_index()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            var loc = grid.GetOrCreate(new HexCoord(3, 4), terrain);

            Assert.False(grid.IsBurning(loc.Hex));
            Assert.True(loc.SetOnFire());
            Assert.True(grid.IsBurning(loc.Hex));
            Assert.Equal(1, grid.BurningCellCount);
        }

        [Fact]
        public void Extinguish_unregisters_hex()
        {
            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            var loc = grid.GetOrCreate(new HexCoord(1, 1), terrain);
            loc.SetOnFire();

            loc.Extinguish();
            Assert.False(grid.IsBurning(loc.Hex));
            Assert.False(loc.LocationFlags.OnFire);
            Assert.Equal(0, grid.BurningCellCount);
        }

        [Fact(Skip = SkipFireReason)]
        public void Failed_burn_continuation_at_sunrise_clears_index()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var grid = new LocationGrid();
            var terrain = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            terrain.SetFireEffect(1);
            var loc = grid.GetOrCreate(new HexCoord(0, 0), terrain);
            loc.SetOnFire();

            loc.CountFire(roll0To99: 99);

            Assert.False(loc.LocationFlags.OnFire);
            Assert.False(grid.IsBurning(loc.Hex));
        }

        [Fact(Skip = SkipFireReason)]
        public void TickFireAtSunrise_only_visits_burning_cells()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var grid = new LocationGrid();
            var plain = TerrainDefinition.FromTerrainType(TerrainType.Plain);
            var forest = TerrainDefinition.FromTerrainType(TerrainType.Forest);
            forest.SetFireEffect(4);

            grid.GetOrCreate(new HexCoord(0, 0), plain);
            var burning = grid.GetOrCreate(new HexCoord(1, 0), forest);
            burning.SetOnFire();

            grid.TickFireAtSunrise(new System.Random(99));

            Assert.Equal(1, grid.BurningCellCount);
            Assert.True(grid.IsBurning(burning.Hex));
        }
    }
}
