namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 一次普攻（或同類交戰）的傷害結算結果。
    /// 先算出來，再由 <see cref="CombatBattleFormulas.ApplyDamage"/> 寫回防守方。
    /// </summary>
    public readonly struct BattleDamageResult
    {
        /// <summary>無傷害。</summary>
        public static BattleDamageResult None => default;

        /// <summary>防守方陣亡兵力。</summary>
        public int SoldierDeaths { get; }

        /// <summary>防守方新增傷兵。</summary>
        public int WoundedIncrease { get; }

        /// <summary>防守方士氣下降（正值＝扣士氣）。</summary>
        public short MoraleLoss { get; }

        public BattleDamageResult(int soldierDeaths, int woundedIncrease, short moraleLoss)
        {
            SoldierDeaths = soldierDeaths < 0 ? 0 : soldierDeaths;
            WoundedIncrease = woundedIncrease < 0 ? 0 : woundedIncrease;
            MoraleLoss = moraleLoss < 0 ? (short)0 : moraleLoss;
        }

        /// <summary>是否有任何效果。</summary>
        public bool HasEffect => SoldierDeaths > 0 || WoundedIncrease > 0 || MoraleLoss > 0;
    }

    /// <summary>
    /// 戰鬥結算靜態入口（普攻／攻防；戰法暫不納入）。
    /// 攻防等讀 <see cref="Combat.Stats"/>。
    /// </summary>
    public static class CombatBattleFormulas
    {
        public static int CalculateAttack(Unit unit) =>
            unit is Combat combat ? CalculateAttack(combat) : 0;

        public static int CalculateAttack(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.Stats.Attack;

        public static int CalculateDefense(Unit unit) =>
            unit is Combat combat ? CalculateDefense(combat) : 0;

        public static int CalculateDefense(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.Stats.Defense;

        public static int CalculateJipo(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.Stats.Jipo;

        public static int CalculateGongcheng(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.Stats.Gongcheng;

        public static int CalculateStrategy(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.Stats.Strategy;

        public static int CalculateConstruction(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.Stats.Construction;

        public static int CalculateMobility(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.TroopMobility;

        public static int CalculateTroopStamina(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.TroopStamina;

        public static int CalculateAttackRange(Combat combat) =>
            combat == null || combat.IsAnnihilated ? 0 : combat.TroopAttackRange;

        /// <summary>計算一次普攻傷害（不改狀態）。空殼。</summary>
        public static BattleDamageResult CalculateNormalAttackDamage(Unit attacker, Unit defender)
        {
            if (attacker == null || defender == null)
                return BattleDamageResult.None;
            if (attacker.IsAnnihilated || defender.IsAnnihilated)
                return BattleDamageResult.None;
            // TODO: 用 Stats.Attack / Stats.Defense 等實作
            return BattleDamageResult.None;
        }

        /// <summary>將傷害結果套用到防守方。</summary>
        public static void ApplyDamage(Unit defender, in BattleDamageResult damage)
        {
            if (defender == null || !damage.HasEffect)
                return;
            // TODO: 寫回兵力／傷兵／士氣
        }

        /// <summary>計算並套用一次普攻。</summary>
        public static BattleDamageResult ResolveNormalAttack(Unit attacker, Unit defender)
        {
            BattleDamageResult damage = CalculateNormalAttackDamage(attacker, defender);
            ApplyDamage(defender, damage);
            return damage;
        }
    }
}
