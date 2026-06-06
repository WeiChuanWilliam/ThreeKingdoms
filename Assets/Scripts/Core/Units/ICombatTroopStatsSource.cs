namespace ThreeKindoms.Core.Units
{
    /// <summary>提供兵種表六圍（初始值），供 <see cref="CombatStatMath"/> 與駐軍部隊共用。</summary>
    public interface ICombatTroopStatsSource
    {
        string TroopKindKey { get; }
        short TroopAttack { get; }
        short TroopDefense { get; }
        short TroopMobility { get; }
        short TroopJipo { get; }
        short TroopGongcheng { get; }
        short TroopStamina { get; }
        short TroopAttackRange { get; }
    }
}
