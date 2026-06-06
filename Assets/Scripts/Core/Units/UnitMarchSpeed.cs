using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 地图上移动用的速度倍率（进格 cost ÷ 倍率）。
    /// 战斗部队：unit.march_speed.combat × 兵种 mobility；
    /// 军团／运输队：参数档绝对 mobility（unit.mobility.legion 等）。
    /// </summary>
    public static class UnitMarchSpeed
    {
        /// <summary>进格 cost 折算用；100 表示「标准一步」。</summary>
        public static float MobilityBaseline =>
            UnitConfigUtil.GetFloat("unit.mobility.baseline", 100f);

        /// <summary>用于 <see cref="UnitLocationBinding.MoveAlongPath"/>；≤0 表示不能移动。</summary>
        public static float GetMarchSpeedMultiplier(Unit unit)
        {
            if (unit == null || unit is Garrison)
                return 0f;

            float baseline = MobilityBaseline;
            if (baseline <= 0f)
                baseline = 100f;

            if (unit is Combat combat)
            {
                float factor = UnitConfigUtil.GetCombatMarchSpeedFactor();
                short mobility = combat.TroopMobility;
                if (mobility <= 0)
                    return 0f;
                return factor * mobility / baseline;
            }

            if (unit is Legion)
                return UnitConfigUtil.GetUnitMarchMobility(UnitKind.Legion) / baseline;

            if (unit is Transport)
                return UnitConfigUtil.GetUnitMarchMobility(UnitKind.Transport) / baseline;

            return 1f;
        }
    }
}
