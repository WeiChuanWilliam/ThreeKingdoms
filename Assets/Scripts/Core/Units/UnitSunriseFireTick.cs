using ThreeKindoms.Core.Locations;

namespace ThreeKindoms.Core.Units
{
    /// <summary>日出時部隊在著火格上的每日處理。</summary>
    public static class UnitSunriseFireTick
    {
        /// <summary>
        /// 腳下格已著火且已 <see cref="UnitLocationBinding.SyncFireFromLocation"/> 後呼叫。
        /// 所有部隊走 <see cref="UnitBurnRules.TryApplyDailyBurnDamage"/>。
        /// </summary>
        public static void ApplyDailyFireFormulas(Unit unit, MapLocation location)
        {
            if (unit == null || location == null || !location.LocationFlags.OnFire)
                return;

            UnitBurnRules.TryApplyDailyBurnDamage(unit, location);
        }
    }
}
