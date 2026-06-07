using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    public sealed class Legion : Unit
    {
        public override float FoodConsumptionFactor => UnitConfigUtil.GetFoodConsumptionFactor(UnitKind.Legion);
        public override float FireDamageFactor => UnitConfigUtil.GetFireDamageFactor(UnitKind.Legion);

        /// <summary>行军 mobility（参数档绝对值，非倍率）。</summary>
        public float MarchMobility => UnitConfigUtil.GetUnitMarchMobility(UnitKind.Legion);

        public Combat Escort { get; private set; }

        public override UnitKind Kind => UnitKind.Legion;

        public Legion(LegionUnitDef def)
            : base(UnitNameBuilder.Resolve(def, UnitKind.Legion), def.Belonged)
        {
            Escort = new Combat(def.CreateEscortDef());
            def.ApplyTo(this);
        }

        internal void RestoreEscort(Combat escortUnit) =>
            Escort = escortUnit ?? throw new System.ArgumentNullException(nameof(escortUnit));

        public bool IsEscortCommanderValid() =>
            Escort?.Commander != null &&
            IsOfficerAssigned(Escort.Commander.RuntimeId);

        public override int CalculateFoodConsumption()
        {
            if (IsAnnihilated) return 0;
            int baseFood = BaseFoodByHeadCount();
            int escortFood = Escort != null && !Escort.IsAnnihilated ? Escort.CalculateFoodConsumption() : 0;
            return System.Math.Max(1, (int)((baseFood + escortFood) * FoodConsumptionFactor));
        }
    }
}
