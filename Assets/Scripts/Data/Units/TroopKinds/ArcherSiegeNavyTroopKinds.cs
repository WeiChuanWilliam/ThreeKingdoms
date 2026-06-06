namespace ThreeKindoms.Data.Units.TroopKinds
{
    public sealed class ArcherTroopKind : AbstractTroopKind
    {
        public override string KindKey => TroopKindKeys.Archer;
    }

    public sealed class BowTroopKind : AbstractBowTroopKind
    {
        public override string KindKey => TroopKindKeys.Bow;
    }

    public sealed class BowAdvanceTroopKind : AbstractBowTroopKind
    {
        public override string KindKey => TroopKindKeys.BowAdvance;
    }

    public sealed class BowWudanTroopKind : AbstractBowTroopKind
    {
        public override string KindKey => TroopKindKeys.BowWudan;
    }

    public sealed class BowDanyangTroopKind : AbstractBowTroopKind
    {
        public override string KindKey => TroopKindKeys.BowDanyang;
    }

    public sealed class CrossbowTroopKind : AbstractCrossbowTroopKind
    {
        public override string KindKey => TroopKindKeys.Crossbow;
    }

    public sealed class CrossbowAdvanceTroopKind : AbstractCrossbowTroopKind
    {
        public override string KindKey => TroopKindKeys.CrossbowAdvance;
    }

    public sealed class CrossbowXiandengTroopKind : AbstractCrossbowTroopKind
    {
        public override string KindKey => TroopKindKeys.CrossbowXiandeng;
    }

    public sealed class CrossbowZhugeTroopKind : AbstractCrossbowTroopKind
    {
        public override string KindKey => TroopKindKeys.CrossbowZhuge;
    }

    public sealed class SiegeChargerTroopKind : AbstractSiegeChargerTroopKind
    {
        public override string KindKey => TroopKindKeys.SiegeCharger;
    }

    public sealed class SiegeMushouTroopKind : AbstractSiegeChargerTroopKind
    {
        public override string KindKey => TroopKindKeys.SiegeMushou;
    }

    public sealed class SiegeElephantTroopKind : AbstractSiegeChargerTroopKind
    {
        public override string KindKey => TroopKindKeys.SiegeElephant;
    }

    public sealed class SiegeShooterTroopKind : AbstractSiegeShooterTroopKind
    {
        public override string KindKey => TroopKindKeys.SiegeShooter;
    }

    public sealed class SiegeStoneTroopKind : AbstractSiegeShooterTroopKind
    {
        public override string KindKey => TroopKindKeys.SiegeStone;
    }

    public sealed class SiegeTowerCrossbowTroopKind : AbstractSiegeShooterTroopKind
    {
        public override string KindKey => TroopKindKeys.SiegeTowerCrossbow;
    }

    public sealed class NavySmallTroopKind : AbstractNavyTroopKind
    {
        public override string KindKey => TroopKindKeys.NavySmall;
    }

    public sealed class NavyMediumTroopKind : AbstractNavyTroopKind
    {
        public override string KindKey => TroopKindKeys.NavyMedium;
    }

    public sealed class NavyLargeTroopKind : AbstractNavyTroopKind
    {
        public override string KindKey => TroopKindKeys.NavyLarge;
    }

    public sealed class NavyFinalTroopKind : AbstractNavyTroopKind
    {
        public override string KindKey => TroopKindKeys.NavyFinal;
    }
}
