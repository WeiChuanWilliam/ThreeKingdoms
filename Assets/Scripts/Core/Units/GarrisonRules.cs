using ThreeKindoms.Core.Buildings;

namespace ThreeKindoms.Core.Units
{
    public static class GarrisonRules
    {
        public static bool IsGarrisonSite(AbstractBuilding building) =>
            building != null && building.IsGarrisonSite;

        public static string GetSiteLabel(SettlementSiteKind kind) => kind switch
        {
            SettlementSiteKind.City => "城池",
            SettlementSiteKind.SmallTown => "小城",
            SettlementSiteKind.Harbor => "港灣",
            SettlementSiteKind.Fort => "砦",
            SettlementSiteKind.Camp => "陣",
            _ => ""
        };
    }
}
