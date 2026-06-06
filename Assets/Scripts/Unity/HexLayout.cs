using ThreeKindoms.Core;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>Axial (q,r) → 世界座標（尖頂六角，XY 平面）。</summary>
    public static class HexLayout
    {
        public static Vector3 AxialToWorld(int q, int r, float size)
        {
            float x = size * (1.5f * q);
            float y = size * (Mathf.Sqrt(3f) * (r + q * 0.5f));
            return new Vector3(x, y, 0f);
        }

        public static Vector3 ToWorld(HexCoord c, float size) => AxialToWorld(c.Q, c.R, size);

        public static HexCoord WorldToAxial(Vector3 world, float size)
        {
            float q = (2f / 3f * world.x) / size;
            float r = (-1f / 3f * world.x + Mathf.Sqrt(3f) / 3f * world.y) / size;
            return HexRound(q, r);
        }

        static HexCoord HexRound(float q, float r)
        {
            float s = -q - r;
            int rq = Mathf.RoundToInt(q);
            int rr = Mathf.RoundToInt(r);
            int rs = Mathf.RoundToInt(s);
            float qDiff = Mathf.Abs(rq - q);
            float rDiff = Mathf.Abs(rr - r);
            float sDiff = Mathf.Abs(rs - s);
            if (qDiff > rDiff && qDiff > sDiff)
                rq = -rr - rs;
            else if (rDiff > sDiff)
                rr = -rq - rs;
            return new HexCoord(rq, rr);
        }
    }
}
