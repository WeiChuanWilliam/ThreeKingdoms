namespace ThreeKindoms.Core.Buildings
{
    /// <summary>對應 C++ Building::AbstractBuilding（標頭曾寫 AbstractBuildings）。</summary>
    public abstract class AbstractBuilding
    {
        public int BuildingId { get; protected set; }
        public string Name { get; protected set; }

        /// <summary>非 <see cref="SettlementSiteKind.None"/> 時，戰鬥部隊進駐可轉駐軍。</summary>
        public SettlementSiteKind SiteKind { get; protected set; }

        protected AbstractBuilding(int buildingId, string name = "", SettlementSiteKind siteKind = SettlementSiteKind.None)
        {
            BuildingId = buildingId;
            Name = name ?? "";
            SiteKind = siteKind;
        }

        public bool IsGarrisonSite => SiteKind != SettlementSiteKind.None;
    }
}
