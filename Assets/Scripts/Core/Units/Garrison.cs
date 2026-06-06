using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Core.Units
{
    /// <summary>駐紮於城池／港灣／砦等據點的部隊（由 <see cref="GarrisonConversion"/> 自 <see cref="Combat"/> 轉入）。</summary>
    public sealed class Garrison : Unit, ICombatTroopStatsSource
    {
        public override float FoodConsumptionFactor => 0f;

        readonly GarrisonSnapshot snapshot;
        readonly SettlementSiteKind siteKind;

        public string TroopKindKey { get; private set; }
        public TroopType TroopType { get; private set; }
        public short TroopAttack { get; private set; }
        public short TroopDefense { get; private set; }
        public short TroopMobility { get; private set; }
        public short TroopJipo { get; private set; }
        public short TroopGongcheng { get; private set; }
        public short TroopStamina { get; private set; }
        public short TroopAttackRange { get; private set; }

        public SettlementSiteKind SiteKind => siteKind;
        public string SiteLabel => GarrisonRules.GetSiteLabel(siteKind);
        public GarrisonSnapshot Snapshot => snapshot;

        public CombatTroopStatBlock BaseTroopStats => CombatStatMath.GetBaseTroopStats(this);
        public CombatTroopStatBlock EffectiveTroopStats => CombatStatMath.GetEffectiveTroopStats(this, this);

        public short EffectiveAttack => CombatStatMath.GetEffectiveAttack(this);
        public short EffectiveDefense => CombatStatMath.GetEffectiveDefense(this);
        public short EffectiveMobility => CombatStatMath.GetEffectiveMobility(this);
        public short EffectiveJipo => CombatStatMath.GetEffectiveJipo(this);
        public short EffectiveGongcheng => CombatStatMath.GetEffectiveGongcheng(this);
        public short EffectiveTroopStamina => CombatStatMath.GetEffectiveTroopStamina(this);
        public short EffectiveAttackRange => CombatStatMath.GetEffectiveAttackRange(this);
        public short UnitIntelligence => CombatStatMath.GetUnitIntelligence(this);

        public override UnitKind Kind => UnitKind.Garrison;

        public Garrison(GarrisonUnitDef def)
            : base(def.UnitDisplayName, def.Belonged)
        {
            snapshot = def.Snapshot;
            siteKind = def.SiteKind;
            def.ApplyTo(this);
        }

        public static Garrison FromCombat(Combat combat, AbstractBuilding site)
        {
            var def = GarrisonUnitDef.FromCombat(combat, site);
            return new Garrison(def);
        }

        public Combat ToFieldCombat() => new Combat(snapshot.ToCombatDef(Belonged));

        internal void ApplyTroopStats(GarrisonSnapshot snap)
        {
            TroopKindKey = snap.TroopKindKey;
            TroopType = snap.TroopType;
            TroopAttack = snap.BaseStats.Attack;
            TroopDefense = snap.BaseStats.Defense;
            TroopMobility = snap.BaseStats.Mobility;
            TroopJipo = snap.BaseStats.Jipo;
            TroopGongcheng = snap.BaseStats.Gongcheng;
            TroopStamina = snap.BaseStats.Stamina;
            TroopAttackRange = snap.BaseStats.AttackRange;
        }

        public override int CalculateFoodConsumption() => 0;
    }
}
