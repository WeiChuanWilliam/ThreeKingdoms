namespace ThreeKindoms.Core.Buildings
{
    /// <summary>建築實作（C++ Building::AbstractBuilding 具體類佔位）。</summary>
    public class Building : AbstractBuilding
    {
        public Building(int buildingId, string name = "", SettlementSiteKind siteKind = SettlementSiteKind.None)
            : base(buildingId, name, siteKind) { }
    }
}
