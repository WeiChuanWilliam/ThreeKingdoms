namespace ThreeKindoms.Core.Buildings
{
    /// <summary>建築實作（C++ Building::AbstractBuilding 具體類佔位）。</summary>
    public class Building : AbstractBuilding
    {
        /// <summary>以 id、名稱與據點類型建立建築實例。</summary>
        public Building(int buildingId, string name = "", SettlementSiteKind siteKind = SettlementSiteKind.None)
            : base(buildingId, name, siteKind) { }
    }
}
