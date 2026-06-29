namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 非四種設計部隊類型（Legion／Combat／Transport）之一；
    /// 舊版 C++ 佔位，用於地圖格多支部隊編組（<see cref="AbstractLocation.GroupUnit"/>）。
    /// </summary>
    public class GroupUnit
    {
        /// <summary>編組識別 id。</summary>
        public int GroupId { get; }

        /// <summary>所屬勢力。</summary>
        public int FactionBelonged { get; set; }

        /// <summary>建立地圖格部隊編組佔位實例。</summary>
        public GroupUnit(int groupId, int factionBelonged = 0)
        {
            GroupId = groupId;
            FactionBelonged = factionBelonged;
        }
    }
}
