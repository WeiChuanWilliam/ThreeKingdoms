using ThreeKindoms.Core.Units;

namespace ThreeKindoms.Data.Units
{
    /// <summary>部隊顯示名稱；轉呼叫 <see cref="UnitUtil.ResolveUnitName"/>。</summary>
    public static class UnitNameBuilder
    {
        public static string Resolve(
            string customUnitName,
            int commanderOfficerId,
            UnitKind kind,
            string troopKindKey = null) =>
            UnitUtil.ResolveUnitName(customUnitName, commanderOfficerId, troopKindKey, kind);
    }
}
