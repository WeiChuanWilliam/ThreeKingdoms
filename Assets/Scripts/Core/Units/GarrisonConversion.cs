using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Locations;

namespace ThreeKindoms.Core.Units
{
    /// <summary>野戰 <see cref="Combat"/> ↔ 據點 <see cref="Garrison"/> 轉換。</summary>
    public static class GarrisonConversion
    {
        /// <summary>部隊進入據點格時呼叫（見 <see cref="UnitLocationBinding"/>）。</summary>
        public static bool TryAutoStation(Combat fieldUnit, MapLocation location, ref Unit bindingUnit)
        {
            if (fieldUnit == null || location == null)
                return false;
            AbstractBuilding site = location.Building ?? fieldUnit.Building;
            if (!GarrisonRules.IsGarrisonSite(site))
                return false;

            Garrison garrison = Garrison.FromCombat(fieldUnit, site);
            location.SetFightingUnit(garrison);
            bindingUnit = garrison;
            return true;
        }

        /// <summary>駐軍離開據點，還原野戰部隊。</summary>
        public static Combat Depart(Garrison garrison, MapLocation location, ref Unit bindingUnit)
        {
            if (garrison == null)
                return null;

            Combat field = garrison.ToFieldCombat();
            if (location != null)
                location.SetFightingUnit(field);
            bindingUnit = field;
            return field;
        }
    }
}
