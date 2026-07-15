using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>兵團（Legion）：地圖上出征的部隊編制，自帶兵糧；無護衛隊，遠征時較脆弱。</summary>
    public sealed class Legion : Unit
    {
        public override float FoodConsumptionFactor => UnitConfigUtil.GetFoodConsumptionFactor(UnitKind.Legion);
        public override float FireDamageFactor => UnitConfigUtil.GetFireDamageFactor(UnitKind.Legion);

        /// <summary>行军 mobility（参数档绝对值，非倍率）。</summary>
        public float MarchMobility => UnitConfigUtil.GetUnitMarchMobility(UnitKind.Legion);

        /// <summary>兵團攜帶的軍糧；下屬戰鬥部隊與本體耗糧皆由此扣除。</summary>
        public int CarriedFood { get; private set; }

        public override UnitKind Kind => UnitKind.Legion;

        /// <summary>兵團僅在駐紮據點內可正常作戰；野戰中不可。</summary>
        public override bool CanFightInField => IsGarrison;

        public Legion(string unitName, int factionBelonged)
            : base(unitName ?? "", factionBelonged)
        {
        }

        public void SetCarriedFood(int value) => CarriedFood = value < 0 ? 0 : value;

        /// <summary>暫定兵糧無限：每日應耗糧恒為 0。</summary>
        public override int CalculateFoodConsumption() => 0;

        /// <summary>暫定兵糧無限：不扣糧，恒成功。</summary>
        public override bool TryConsumeDailyFood() => true;

        /// <summary>暫定兵糧無限：不實際扣除 <see cref="CarriedFood"/>。</summary>
        public bool TryConsumeFood(int amount) => true;

        /// <summary>野戰：無法正常作戰，戰鬥力 0。</summary>
        protected override int CalculateNonGarrisonCombatPower() => 0;

        /// <summary>駐紮：高攻高防（接近戰鬥部隊量級）。</summary>
        protected override int CalculateGarrisonCombatPower() =>
            UnitCombatPowerRules.CalculateStationedLegionPower(this);
    }
}
