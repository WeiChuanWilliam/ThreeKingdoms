namespace ThreeKindoms.Core.Units
{
    /// <summary>提供兵種表六圍（初始值），供 <see cref="CombatStatMath"/> 與駐軍部隊共用。</summary>
    public interface ICombatTroopStatsSource
    {
        /// <summary>兵種表鍵值。</summary>
        string TroopKindKey { get; }

        /// <summary>兵種表攻擊。</summary>
        short TroopAttack { get; }

        /// <summary>兵種表防禦。</summary>
        short TroopDefense { get; }

        /// <summary>兵種表機動。</summary>
        short TroopMobility { get; }

        /// <summary>兵種表破甲。</summary>
        short TroopJipo { get; }

        /// <summary>兵種表攻城。</summary>
        short TroopGongcheng { get; }

        /// <summary>兵種表耐力。</summary>
        short TroopStamina { get; }

        /// <summary>兵種表攻擊距離。</summary>
        short TroopAttackRange { get; }
    }
}
