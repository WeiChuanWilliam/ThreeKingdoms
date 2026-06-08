using ThreeKindoms.Core.Locations;

namespace ThreeKindoms.Core.Buildings
{
    /// <summary>著火格上的建築每日灼燒（由日出掃 <see cref="LocationGrid.BurningCells"/> 反查）。</summary>
    public static class BuildingBurnRules
    {
        public static bool TryApplyDailyBurnDamage(AbstractBuilding building, MapLocation location)
        {
            if (building == null || location == null || !location.LocationFlags.OnFire)
                return false;
            // TODO: 城防耐久、建築起火狀態
            return false;
        }
    }
}
