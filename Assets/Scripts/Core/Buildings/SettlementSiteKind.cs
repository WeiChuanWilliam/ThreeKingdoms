namespace ThreeKindoms.Core.Buildings
{
    /// <summary>可駐紮（自動轉 <see cref="Units.Garrison"/>）的據點類型。</summary>
    public enum SettlementSiteKind : byte
    {
        None = 0,
        /// <summary>城池</summary>
        City = 1,
        /// <summary>小城</summary>
        SmallTown = 2,
        /// <summary>港灣</summary>
        Harbor = 3,
        /// <summary>砦</summary>
        Fort = 4,
        /// <summary>陣（營寨）</summary>
        Camp = 5
    }
}
