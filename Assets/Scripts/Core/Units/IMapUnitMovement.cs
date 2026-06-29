using ThreeKindoms.Core;

namespace ThreeKindoms.Core.Units
{
    /// <summary>行軍尋路用（Spike）；部隊本體是 <see cref="Unit"/>。</summary>
    public interface IMapUnitMovement
    {
        /// <summary>地圖上的六角格座標。</summary>
        HexCoord Position { get; set; }

        /// <summary>本日剩餘移動點數。</summary>
        int MovementPointsLeft { get; set; }
    }
}
