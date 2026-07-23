namespace ThreeKindoms.Core
{
    /// <summary>通用數值工具（clamp、範圍限制等）。</summary>
    public static class NumericUtil
    {
        /// <summary>將 <paramref name="v"/> 限制在 [<paramref name="min"/>, <paramref name="max"/>]；若 min &gt; max 則自動交換。</summary>
        public static short ClampToTarget(short v, short min, short max)
        {
            if (min > max)
                (min, max) = (max, min);

            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        public static int ClampToTarget(int v, int min, int max)
        {
            if (min > max)
                (min, max) = (max, min);

            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        public static byte ClampToTarget(byte v, byte min, byte max)
        {
            if (min > max)
                (min, max) = (max, min);

            if (v < min) return min;
            if (v > max) return max;
            return v;
        }

        public static float ClampToTarget(float v, float min, float max)
        {
            if (min > max)
                (min, max) = (max, min);

            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
    }
}
