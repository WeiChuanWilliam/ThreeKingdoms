using ThreeKindoms.Core;

namespace ThreeKindoms.Core.Units
{
    /// <summary>行軍尋路用（Spike）；部隊本體是 <see cref="Unit"/>。</summary>
    public interface IMapUnitMovement
    {
        HexCoord Position { get; set; }
        int MovementPointsLeft { get; set; }
    }
}
