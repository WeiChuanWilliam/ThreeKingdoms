namespace ThreeKindoms.Data.Units.TroopKinds
{
    public sealed class InfantryTroopKind : AbstractTroopKind
    {
        public override string KindKey => TroopKindKeys.Infantry;
    }

    public sealed class SpearTroopKind : AbstractSpearTroopKind
    {
        public override string KindKey => TroopKindKeys.Spear;
    }

    public sealed class SpearAdvanceTroopKind : AbstractSpearTroopKind
    {
        public override string KindKey => TroopKindKeys.SpearAdvance;
    }

    public sealed class SpearQingzhouTroopKind : AbstractSpearTroopKind
    {
        public override string KindKey => TroopKindKeys.SpearQingzhou;
    }

    public sealed class SpearDajiTroopKind : AbstractSpearTroopKind
    {
        public override string KindKey => TroopKindKeys.SpearDaji;
    }

    public sealed class ArmorTroopKind : AbstractArmorTroopKind
    {
        public override string KindKey => TroopKindKeys.Armor;
    }

    public sealed class ArmorAdvanceTroopKind : AbstractArmorTroopKind
    {
        public override string KindKey => TroopKindKeys.ArmorAdvance;
    }

    public sealed class ArmorXianzhenTroopKind : AbstractArmorTroopKind
    {
        public override string KindKey => TroopKindKeys.ArmorXianzhen;
    }

    public sealed class ArmorBaimaoTroopKind : AbstractArmorTroopKind
    {
        public override string KindKey => TroopKindKeys.ArmorBaimao;
    }

    public sealed class CavalryTroopKind : AbstractTroopKind
    {
        public override string KindKey => TroopKindKeys.Cavalry;
    }

    public sealed class KnightTroopKind : AbstractKnightTroopKind
    {
        public override string KindKey => TroopKindKeys.Knight;
    }

    public sealed class KnightAdvanceTroopKind : AbstractKnightTroopKind
    {
        public override string KindKey => TroopKindKeys.KnightAdvance;
    }

    public sealed class KnightHubaoTroopKind : AbstractKnightTroopKind
    {
        public override string KindKey => TroopKindKeys.KnightHubao;
    }

    public sealed class KnightXilianTroopKind : AbstractKnightTroopKind
    {
        public override string KindKey => TroopKindKeys.KnightXilian;
    }

    public sealed class HorsemanTroopKind : AbstractHorsemanTroopKind
    {
        public override string KindKey => TroopKindKeys.Horseman;
    }

    public sealed class HorsemanAdvanceTroopKind : AbstractHorsemanTroopKind
    {
        public override string KindKey => TroopKindKeys.HorsemanAdvance;
    }

    public sealed class HorsemanBingzhouTroopKind : AbstractHorsemanTroopKind
    {
        public override string KindKey => TroopKindKeys.HorsemanBingzhou;
    }

    public sealed class HorsemanBaimaTroopKind : AbstractHorsemanTroopKind
    {
        public override string KindKey => TroopKindKeys.HorsemanBaima;

        public override float GetDamageMultiplierAgainst(AbstractTroopKind defender)
        {
            if (defender?.Category == TroopType.Archer)
                return 1.2f;
            return base.GetDamageMultiplierAgainst(defender);
        }
    }
}
