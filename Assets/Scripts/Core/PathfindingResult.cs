using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    public sealed class PathfindingResult
    {
        public bool Success { get; }
        public IReadOnlyList<HexCoord> Path { get; }
        public int TotalCost { get; }
        public string FailureReason { get; }

        PathfindingResult(bool success, List<HexCoord> path, int totalCost, string reason)
        {
            Success = success;
            Path = path;
            TotalCost = totalCost;
            FailureReason = reason;
        }

        public static PathfindingResult Ok(List<HexCoord> path, int totalCost) =>
            new(true, path, totalCost, null);

        public static PathfindingResult Fail(string reason) =>
            new(false, new List<HexCoord>(), 0, reason);
    }
}
