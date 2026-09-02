using System;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 一次傷害結算結果（兵力、傷兵、士氣、體力四項調整量）。
    /// 交戰／火攻／陷阱／建築等皆產出此結果，再統一用 <see cref="Apply"/> 寫回 <see cref="Unit"/>。
    /// </summary>
    public readonly struct BattleDamageResult
    {
        /// <summary>無任何調整。</summary>
        public static BattleDamageResult None => default;

        /// <summary>可戰士兵陣亡（從部隊移除，不再變成傷兵）。</summary>
        public int SoldierDeaths { get; }

        /// <summary>可戰士兵轉為傷兵（從可戰扣、加到傷兵）。</summary>
        public int WoundedIncrease { get; }

        /// <summary>士氣下降（正值＝扣士氣）。</summary>
        public short MoraleLoss { get; }

        /// <summary>體力下降（正值＝扣體力）。</summary>
        public short StaminaLoss { get; }

        /// <summary>產生四項調整結果（負值歸零）。</summary>
        public BattleDamageResult(
            int soldierDeaths,
            int woundedIncrease,
            short moraleLoss,
            short staminaLoss)
        {
            SoldierDeaths = soldierDeaths < 0 ? 0 : soldierDeaths;
            WoundedIncrease = woundedIncrease < 0 ? 0 : woundedIncrease;
            MoraleLoss = moraleLoss < 0 ? (short)0 : moraleLoss;
            StaminaLoss = staminaLoss < 0 ? (short)0 : staminaLoss;
        }

        /// <summary>是否有任何一項調整。</summary>
        public bool HasEffect =>
            SoldierDeaths > 0 || WoundedIncrease > 0 || MoraleLoss > 0 || StaminaLoss > 0;

        /// <summary>
        /// 將結果寫回部隊。
        /// 可戰：先扣陣亡，再扣轉傷；傷兵：加上轉傷人數。
        /// 例：兵1000、陣亡100、轉傷400 → 可戰500、傷兵+400。
        /// 寫回後若可戰過低、或 <see cref="Unit.Morale"/>／<see cref="Unit.Stamina"/> ≤0，
        /// 會經 <see cref="Unit.TryRemoveIfAnnihilated"/> 移除部隊。
        /// </summary>
        public static void Apply(Unit unit, in BattleDamageResult result)
        {
            if (unit == null || !result.HasEffect)
                return;

            int fighting = unit.Soldiers;
            int deaths = Math.Min(result.SoldierDeaths, fighting);
            int remainingAfterDeaths = fighting - deaths;
            int woundedAdd = Math.Min(result.WoundedIncrease, remainingAfterDeaths);
            int newSoldiers = remainingAfterDeaths - woundedAdd;
            int newWounded = unit.Wounded + woundedAdd;
            unit.SetManpower(newSoldiers, newWounded);

            if (result.MoraleLoss > 0)
                unit.ChangeMorale((short)-result.MoraleLoss);
            if (result.StaminaLoss > 0)
                unit.ChangeStamina((short)-result.StaminaLoss);

            // 兵力／士氣／體力任一達全滅條件 → 自 UnitRegistry 移除
            unit.TryRemoveIfAnnihilated();
        }
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

        /// <summary>計算一次普攻傷害（不改狀態）。空殼；之後在此填公式並 new BattleDamageResult(...)。</summary>
        public static BattleDamageResult CalculateNormalAttackDamage(Unit attacker, Unit defender)
        {
            if (attacker == null || defender == null)
                return BattleDamageResult.None;
            if (attacker.IsAnnihilated || defender.IsAnnihilated)
                return BattleDamageResult.None;
            // TODO: 用 Stats.Attack / Stats.Defense 等算出四項，再 return new BattleDamageResult(...)
            return BattleDamageResult.None;
        }

        /// <summary>將傷害結果寫回部隊（轉呼叫 <see cref="BattleDamageResult.Apply"/>）。</summary>
        public static void ApplyDamage(Unit unit, in BattleDamageResult damage) =>
            BattleDamageResult.Apply(unit, damage);

        /// <summary>計算並套用一次普攻。</summary>
        public static BattleDamageResult ResolveNormalAttack(Unit attacker, Unit defender)
        {
            BattleDamageResult damage = CalculateNormalAttackDamage(attacker, defender);
            BattleDamageResult.Apply(defender, damage);
            return damage;
        }
    }
}
