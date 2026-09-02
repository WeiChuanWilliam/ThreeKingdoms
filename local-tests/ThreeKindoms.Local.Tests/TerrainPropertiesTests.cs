using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Data.Terrain;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class TerrainPropertiesTests
    {
        [Fact]
        public void Terrain_properties_loads_six_types_matching_spike_defaults()
        {
            Assert.True(TerrainConfigUtil.Load(TestPaths.TerrainPropertiesPath));

            Assert.Equal(6, TerrainConfigUtil.TerrainTypeKeys.Count);
            Assert.True(TerrainConfigUtil.TryGetDefinitionById(0, out TerrainDefinition plain));
            Assert.Equal("平原", plain.TerrainName);
            Assert.Equal(15, plain.SuggestedEnterCost);
            Assert.True(plain.TerrainFlags.Reachable);
            Assert.True(plain.TerrainFlags.Construction);

            Assert.True(TerrainConfigUtil.TryGetDefinitionById(5, out TerrainDefinition forest));
            Assert.True(forest.TerrainFlags.Fireable);
            Assert.Equal(1, forest.FireEffect);

            Assert.True(TerrainConfigUtil.TryGetDefinitionById(4, out TerrainDefinition water));
            Assert.False(water.TerrainFlags.Reachable);
            Assert.Equal(99, water.SuggestedEnterCost);
        }
    }
}
