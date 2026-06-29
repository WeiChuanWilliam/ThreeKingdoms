using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Core
{
    /// <summary>
    /// Spike 用簡化行軍資料（非 C++ 標頭）。
    /// 正式部隊請用 <see cref="Units.Unit"/> + <see cref="Units.UnitLocationBinding"/>。
    /// </summary>
    public sealed class UnitState : IMapUnitMovement
    {
        /// <summary>單位 id。</summary>
        public int Id { get; }

        /// <summary>介面顯示名稱。</summary>
        public string DisplayName { get; }

        /// <summary>目前所在六角座標。</summary>
        public HexCoord Position { get; set; }

        /// <summary>今日剩餘行動力點數。</summary>
        public int MovementPointsLeft { get; set; }

        /// <summary>建立 Spike 用簡化部隊並置於起點。</summary>
        public UnitState(int id, string displayName, HexCoord start)
        {
            Id = id;
            DisplayName = displayName;
            Position = start;
            MovementPointsLeft = MovementRules.DefaultDailyPoints;
        }

        /// <summary>日出時將行動力補滿至每日預設值。</summary>
        public void RefillMovementAtSunrise() =>
            MovementPointsLeft = MovementRules.DefaultDailyPoints;
    }
}
