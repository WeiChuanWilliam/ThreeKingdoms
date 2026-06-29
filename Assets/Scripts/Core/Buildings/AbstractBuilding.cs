namespace ThreeKindoms.Core.Buildings
{
    /// <summary>對應 C++ Building::AbstractBuilding（標頭曾寫 AbstractBuildings）。</summary>
    public abstract class AbstractBuilding
    {
        /// <summary>建築定義 id。</summary>
        public int BuildingId { get; protected set; }

        /// <summary>建築顯示名稱。</summary>
        public string Name { get; protected set; }

        /// <summary>非 <see cref="SettlementSiteKind.None"/> 時可駐紮（<see cref="Units.Unit.IsStationed"/>）。</summary>
        public SettlementSiteKind SiteKind { get; protected set; }

        /// <summary>地圖內建據點（城池、縣城、港灣、關口）。</summary>
        public bool IsMapPlacedSite => SettlementSiteRules.IsMapPlaced(SiteKind);

        /// <summary>玩家建造的營寨（岩砦、要塞、陣、寨）。</summary>
        public bool IsPlayerBuiltSite => SettlementSiteRules.IsPlayerBuilt(SiteKind);

        /// <summary>以 id、名稱與據點類型初始化建築基底。</summary>
        protected AbstractBuilding(int buildingId, string name = "", SettlementSiteKind siteKind = SettlementSiteKind.None)
        {
            BuildingId = buildingId;
            Name = name ?? "";
            SiteKind = siteKind;
        }

        /// <summary>是否為可駐軍據點（城池、港灣等）。</summary>
        /// <summary>是否為可駐紮據點。</summary>
        public bool IsGarrisonSite => SettlementSiteRules.IsStationable(SiteKind);
    }
}
