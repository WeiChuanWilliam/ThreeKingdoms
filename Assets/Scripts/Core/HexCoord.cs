using System;

namespace ThreeKindoms.Core
{
    /// <summary>軸向座標 (q, r)。平面六角格標準表示。</summary>
    [Serializable]
    public readonly struct HexCoord : IEquatable<HexCoord>
    {
        /// <summary>軸向座標 q（列）。</summary>
        public int Q { get; }

        /// <summary>軸向座標 r（行）。</summary>
        public int R { get; }

        /// <summary>以 q、r 建立六角座標。</summary>
        public HexCoord(int q, int r)
        {
            Q = q;
            R = r;
        }

        /// <summary>立方座標 s（由 q、r 推得，恆滿足 q+r+s=0）。</summary>
        public int S => -Q - R;

        /// <summary>與另一座標是否相等。</summary>
        public bool Equals(HexCoord other) => Q == other.Q && R == other.R;

        /// <summary>與物件是否為相同六角座標。</summary>
        public override bool Equals(object obj) => obj is HexCoord other && Equals(other);

        /// <summary>雜湊碼（供字典鍵用）。</summary>
        public override int GetHashCode() => HashCode.Combine(Q, R);

        /// <summary>可讀字串（q,r）。</summary>
        public override string ToString() => $"({Q},{R})";

        /// <summary>座標相等比較。</summary>
        public static bool operator ==(HexCoord a, HexCoord b) => a.Equals(b);

        /// <summary>座標不等比較。</summary>
        public static bool operator !=(HexCoord a, HexCoord b) => !a.Equals(b);
    }
}
