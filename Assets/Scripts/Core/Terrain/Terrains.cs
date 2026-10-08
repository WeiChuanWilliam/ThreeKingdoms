using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Terrain;

namespace ThreeKindoms.Core.Terrain
{
    /// <summary>官道。</summary>
    public sealed class MainRoadTerrain : AbstractTerrain
    {
        internal MainRoadTerrain() : base(TerrainType.MainRoad, "官道", 3,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true, Fort = true }) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>主徑。</summary>
    public sealed class MainPathTerrain : AbstractTerrain
    {
        internal MainPathTerrain() : base(TerrainType.MainPath, "主徑", 4,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true, Fort = true, Fireable = true },
            fireEffect: 1, staminaPenalty: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>林道。</summary>
    public sealed class ForestPathTerrain : AbstractTerrain
    {
        internal ForestPathTerrain() : base(TerrainType.ForestPath, "林道", 6,
            new TerrainFlags { Reachable = true, Trap = true, Fireable = true },
            fireEffect: 3, staminaPenalty: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>平地。</summary>
    public sealed class PlainTerrain : AbstractTerrain
    {
        internal PlainTerrain() : base(TerrainType.Plain, "平地", 5,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true, Fort = true, Fireable = true },
            fireEffect: 1, staminaPenalty: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>草原。</summary>
    public sealed class GrasslandTerrain : AbstractTerrain
    {
        internal GrasslandTerrain() : base(TerrainType.Grassland, "草原", 5,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true, Fort = true, Fireable = true },
            fireEffect: 2, staminaPenalty: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>森林。</summary>
    public sealed class ForestTerrain : AbstractTerrain
    {
        internal ForestTerrain() : base(TerrainType.Forest, "森林", 8,
            new TerrainFlags { Reachable = true, Fireable = true },
            fireEffect: 3, moralePenalty: 1, staminaPenalty: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>山林。</summary>
    public sealed class MountainForestTerrain : AbstractTerrain
    {
        internal MountainForestTerrain() : base(TerrainType.MountainForest, "山林", 16,
            new TerrainFlags { Reachable = true, Fireable = true },
            fireEffect: 3, moralePenalty: 2, staminaPenalty: 3) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>叢林。</summary>
    public sealed class JungleTerrain : AbstractTerrain
    {
        internal JungleTerrain() : base(TerrainType.Jungle, "叢林", 12,
            new TerrainFlags { Reachable = true, Trap = true, Fireable = true },
            fireEffect: 2, moralePenalty: 3, staminaPenalty: 3) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>低山。</summary>
    public sealed class LowMountainTerrain : AbstractTerrain
    {
        internal LowMountainTerrain() : base(TerrainType.LowMountain, "低山", 8,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true, Fireable = true },
            fireEffect: 1, moralePenalty: 1, staminaPenalty: 2) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>中山。</summary>
    public sealed class MidMountainTerrain : AbstractTerrain
    {
        internal MidMountainTerrain() : base(TerrainType.MidMountain, "中山", 12,
            new TerrainFlags { Reachable = true, Trap = true },
            moralePenalty: 2, staminaPenalty: 3) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>高山。</summary>
    public sealed class HighMountainTerrain : AbstractTerrain
    {
        internal HighMountainTerrain() : base(TerrainType.HighMountain, "高山", 16,
            new TerrainFlags { Reachable = true },
            moralePenalty: 3, staminaPenalty: 3) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>濕地。</summary>
    public sealed class WetlandTerrain : AbstractTerrain
    {
        internal WetlandTerrain() : base(TerrainType.Wetland, "濕地", 12,
            new TerrainFlags { Reachable = true },
            moralePenalty: 2, staminaPenalty: 2) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>沙地。</summary>
    public sealed class SandTerrain : AbstractTerrain
    {
        internal SandTerrain() : base(TerrainType.Sand, "沙地", 6,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true, Fort = true }) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>山路。</summary>
    public sealed class MountainPathTerrain : AbstractTerrain
    {
        internal MountainPathTerrain() : base(TerrainType.MountainPath, "山路", 6,
            new TerrainFlags { Reachable = true, Trap = true, Fireable = true },
            fireEffect: 1, staminaPenalty: 2) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>棧道。</summary>
    public sealed class PlankRoadTerrain : AbstractTerrain
    {
        internal PlankRoadTerrain() : base(TerrainType.PlankRoad, "棧道", 8,
            new TerrainFlags { Reachable = true },
            moralePenalty: 3, staminaPenalty: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>毒沼。</summary>
    public sealed class PoisonSwampTerrain : AbstractTerrain
    {
        internal PoisonSwampTerrain() : base(TerrainType.PoisonSwamp, "毒沼", 12,
            new TerrainFlags { Reachable = true },
            moralePenalty: 3, staminaPenalty: 2) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>河流。</summary>
    public sealed class RiverTerrain : AbstractTerrain
    {
        internal RiverTerrain() : base(TerrainType.River, "河流", 8,
            new TerrainFlags { Reachable = true },
            moralePenalty: 1, staminaPenalty: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>大河。</summary>
    public sealed class GreatRiverTerrain : AbstractTerrain
    {
        internal GreatRiverTerrain() : base(TerrainType.GreatRiver, "大河", 4,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true },
            shipRequirement: 0) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>急流。</summary>
    public sealed class RapidsTerrain : AbstractTerrain
    {
        internal RapidsTerrain() : base(TerrainType.Rapids, "急流", 5,
            new TerrainFlags { Reachable = true },
            shipRequirement: 1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>凍河。</summary>
    public sealed class FrozenRiverTerrain : AbstractTerrain
    {
        internal FrozenRiverTerrain() : base(TerrainType.FrozenRiver, "凍河", 6,
            new TerrainFlags { Reachable = true }) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>沿海。</summary>
    public sealed class CoastTerrain : AbstractTerrain
    {
        internal CoastTerrain() : base(TerrainType.Coast, "沿海", 4,
            new TerrainFlags { Reachable = true, Construction = true, Trap = true },
            shipRequirement: 2) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>城池（城池、關隘、要塞）。</summary>
    public sealed class CityTerrain : AbstractTerrain
    {
        internal CityTerrain() : base(TerrainType.City, "城池", 0,
            new TerrainFlags { Reachable = true, Fireable = true },
            fireEffect: 1, moralePenalty: -1, staminaPenalty: -1) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>城寨（港灣、陣、寨、縣城）。</summary>
    public sealed class StockadeTerrain : AbstractTerrain
    {
        internal StockadeTerrain() : base(TerrainType.Stockade, "城寨", 0,
            new TerrainFlags { Reachable = true, Fireable = true },
            fireEffect: 2) { }

        public override bool CalculateMovement(Unit unit) => SpendMoveCost(unit);
    }

    /// <summary>不可通行。</summary>
    public sealed class ImpassableTerrain : AbstractTerrain
    {
        internal ImpassableTerrain() : base(TerrainType.Impassable, "不可通行", byte.MaxValue,
            new TerrainFlags()) { }

        public override bool CalculateMovement(Unit unit) => false;
    }
}
