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
        public override bool CanFightInField => IsStationed;

        public Legion(LegionUnitDef def)
            : base(UnitNameBuilder.Resolve(def, UnitKind.Legion), def.Belonged)
        {
            def.ApplyTo(this);
        }

        public void SetCarriedFood(int value) => CarriedFood = value < 0 ? 0 : value;

        /// <summary>扣除兵糧；餘量不足回傳 false（斷糧）。</summary>
        public bool TryConsumeFood(int amount)
        {
            if (amount <= 0) return true;
            if (CarriedFood < amount) return false;
            CarriedFood -= amount;
            return true;
        }

        public override int CalculateFoodConsumption()
        {
            if (IsAnnihilated || IsStationed) return 0;
            return System.Math.Max(1, (int)(BaseFoodByHeadCount() * FoodConsumptionFactor));
        }

        public override bool TryConsumeDailyFood()
        {
            int cost = CalculateFoodConsumption();
            return TryConsumeFood(cost);
        }

        /// <summary>未駐紮時戰鬥力為 0（無法野戰）；駐紮時接近戰鬥部隊量級。</summary>
        public override int CalculateCombatPower()
        {
            if (IsAnnihilated) return 0;
            if (!IsStationed) return 0;
            return UnitCombatPowerRules.CalculateStationedLegionPower(this);
        }
    }
}
