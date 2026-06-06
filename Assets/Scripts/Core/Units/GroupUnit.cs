namespace ThreeKindoms.Core.Units
{
    /// <summary>對應 C++ Unit::GroupUnit*（佔位：多支部隊編組）。</summary>
    public class GroupUnit
    {
        public int GroupId { get; }
        public int FactionBelonged { get; set; }

        public GroupUnit(int groupId, int factionBelonged = 0)
        {
            GroupId = groupId;
            FactionBelonged = factionBelonged;
        }
    }
}
