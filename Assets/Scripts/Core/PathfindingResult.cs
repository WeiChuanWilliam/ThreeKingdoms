using System.Collections.Generic;

namespace ThreeKindoms.Core
{
    /// <summary>尋路結果：成功路徑、總消耗或失敗原因。</summary>
    public sealed class PathfindingResult
    {
        /// <summary>是否成功找到可行路徑。</summary>
        public bool Success { get; }

        /// <summary>路徑座標序列（含起點與終點）。</summary>
        public IReadOnlyList<HexCoord> Path { get; }

        /// <summary>路徑總行動力消耗。</summary>
        public int TotalCost { get; }

        /// <summary>失敗時的說明文字。</summary>
        public string FailureReason { get; }

        PathfindingResult(bool success, List<HexCoord> path, int totalCost, string reason)
        {
            Success = success;
            Path = path;
            TotalCost = totalCost;
            FailureReason = reason;
        }

        /// <summary>建立成功結果。</summary>
        public static PathfindingResult Ok(List<HexCoord> path, int totalCost) =>
            new(true, path, totalCost, null);

        /// <summary>建立失敗結果並附原因。</summary>
        public static PathfindingResult Fail(string reason) =>
            new(false, new List<HexCoord>(), 0, reason);
    }
}
