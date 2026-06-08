using ThreeKindoms.Core.Locations;

namespace ThreeKindoms.Core.Units
{
    /// <summary>著火格反查部隊後的每日灼燒結算（由 <see cref="LocationGridSunrise.ApplyBurnDamageAtSunrise"/> 呼叫）。</summary>
    public static class UnitSunriseFireTick
    {
        /// <summary>
        /// 腳下格已著火時呼叫；走 <see cref="UnitBurnRules.TryApplyDailyBurnDamage"/>。
        /// 不由部隊清單驅動，而是由著火座標反查 <see cref="MapLocation.OccupyingUnit"/>。
        /// </summary>
        public static void ApplyDailyFireFormulas(Unit unit, MapLocation location)
        {
            if (unit == null || location == null || !location.LocationFlags.OnFire)
                return;

            UnitBurnRules.TryApplyDailyBurnDamage(unit, location);
        }
    }
}
