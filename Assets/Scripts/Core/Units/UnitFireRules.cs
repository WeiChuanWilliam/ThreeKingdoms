namespace ThreeKindoms.Core.Units
{
    /// <summary>部隊與腳下格著火狀態。</summary>
    public static class UnitFireRules
    {
        /// <summary>部隊是否著火＝腳下格著火。</summary>
        public static bool IsOnFire(Unit unit) =>
            unit?.Location?.IsOnFire == true;
    }
}
