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
    /// </summary>
    public static class CombatBattleFormulas
    {
        /// <summary>攻擊力。</summary>
        public static int CalculateAttack(Unit unit)
        {
            if (unit is Combat combat)
                return CalculateAttack(combat);
            return 0;
        }

        /// <summary>攻擊力（Combat）。</summary>
        public static int CalculateAttack(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return 0;
            // TODO: 實作攻擊力公式（兵力、士氣、體力、武將…）
            return combat.EffectiveAttack;
        }

        /// <summary>防禦力。</summary>
        public static int CalculateDefense(Unit unit)
        {
            if (unit is Combat combat)
                return CalculateDefense(combat);
            return 0;
        }

        /// <summary>防禦力（Combat）。</summary>
        public static int CalculateDefense(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return 0;
            // TODO: 實作防禦力公式
            return combat.EffectiveDefense;
        }

        /// <summary>機動力（Combat）。</summary>
        public static int CalculateMobility(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return 0;
            return combat.EffectiveMobility;
        }

        /// <summary>破甲（Combat）。</summary>
        public static int CalculateJipo(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return 0;
            return combat.EffectiveJipo;
        }

        /// <summary>攻城（Combat）。</summary>
        public static int CalculateGongcheng(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return 0;
            return combat.EffectiveGongcheng;
        }

        /// <summary>部隊耐力（Combat）。</summary>
        public static int CalculateTroopStamina(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return 0;
            return combat.EffectiveTroopStamina;
        }

        /// <summary>普攻攻擊距離（格）。</summary>
        public static int CalculateAttackRange(Combat combat)
        {
            if (combat == null || combat.IsAnnihilated)
                return 0;
            return combat.EffectiveAttackRange;
        }

        /// <summary>計算一次普攻傷害（不改狀態）。空殼。</summary>
        public static BattleDamageResult CalculateNormalAttackDamage(Unit attacker, Unit defender)
        {
            if (attacker == null || defender == null)
                return BattleDamageResult.None;
            if (attacker.IsAnnihilated || defender.IsAnnihilated)
                return BattleDamageResult.None;

            int atk = CalculateAttack(attacker);
            int def = CalculateDefense(defender);
            _ = atk;
            _ = def;
            return BattleDamageResult.None;
        }

        /// <summary>將傷害結果寫入防守方。空殼。</summary>
        public static void ApplyDamage(Unit defender, in BattleDamageResult damage)
        {
            if (defender == null || !damage.HasEffect)
                return;
            // TODO: 扣 Soldiers、加 Wounded、扣 Morale
        }

        /// <summary>一次普攻：計算＋套用。</summary>
        public static BattleDamageResult ResolveNormalAttack(Unit attacker, Unit defender)
        {
            BattleDamageResult damage = CalculateNormalAttackDamage(attacker, defender);
            ApplyDamage(defender, damage);
            return damage;
        }
    }
}
