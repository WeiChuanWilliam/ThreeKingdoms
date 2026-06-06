using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core
{
    /// <summary>
    /// Spike 用簡化行軍資料（非 C++ 標頭）。
    /// 正式部隊請用 <see cref="Units.Unit"/> + <see cref="Units.UnitLocationBinding"/>。
    /// </summary>
    public sealed class UnitState : IMapUnitMovement
    {
        public int Id { get; }
        public string DisplayName { get; }
        public HexCoord Position { get; set; }
        public int MovementPointsLeft { get; set; }

        public UnitState(int id, string displayName, HexCoord start)
        {
            Id = id;
            DisplayName = displayName;
            Position = start;
            MovementPointsLeft = MovementRules.DefaultDailyPoints;
        }

        public void RefillMovementAtSunrise() =>
            MovementPointsLeft = MovementRules.DefaultDailyPoints;
    }
}
