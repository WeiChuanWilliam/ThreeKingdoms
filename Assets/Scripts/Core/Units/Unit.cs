using System.Collections.Generic;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    public abstract class Unit
    {
        readonly List<Officer> viceOfficers = new();
        bool annihilated;

        public string UnitName { get; }
        public int Belonged { get; }

        UnitFlags unitFlags;
        public ref UnitFlags UnitFlags => ref unitFlags;
        public AbstractBuilding Building { get; private set; }
        public short FireEffect { get; private set; }
        public short MoralePenalty { get; private set; }
        public UnitEffectMaps EffectMaps { get; } = new();

        /// <summary>腳下格著火即視為著火。</summary>
        public bool IsOnFire => Location?.IsOnFire == true;

        public short Morale { get; private set; }
        public short Stamina { get; private set; }
        public int Money { get; private set; }

        /// <summary>主將（部隊專用複本，非 OfficerPool 本尊）。</summary>
        public Officer Commander { get; private set; }

        public int Soldiers { get; private set; }
        public int Wounded { get; private set; }

        public UnitLocationBinding Location { get; private set; }

        public IReadOnlyList<Officer> ViceOfficers => viceOfficers;

        public int EffectiveCombatStrength => UnitManpower.EffectiveCombatStrength(Soldiers, Wounded);
        public bool IsAnnihilated => annihilated || UnitManpower.IsAnnihilated(Soldiers);

        public abstract UnitKind Kind { get; }
        /// <summary>耗粮倍率（相对 <see cref="BaseFoodByHeadCount"/>）。</summary>
        public abstract float FoodConsumptionFactor { get; }
        public abstract int CalculateFoodConsumption();

        protected Unit(string name, int factionBelonged)
        {
            UnitName = name ?? "";
            Belonged = factionBelonged;
            unitFlags = new UnitFlags();
            Location = new UnitLocationBinding(this);
        }

        public bool ContainsViceOfficer(int officerDefId) =>
            officerDefId > 0 && FindVice(officerDefId) != null;

        public bool AddViceOfficer(Officer unitCopy)
        {
            if (unitCopy == null || unitCopy.RuntimeId <= 0) return false;
            if (FindVice(unitCopy.RuntimeId) != null) return false;
            viceOfficers.Add(unitCopy);
            return true;
        }

        public bool AddViceOfficerFromPool(int officerDefId)
        {
            Officer copy = OfficerPool.CloneForUnit(officerDefId);
            return copy != null && AddViceOfficer(copy);
        }

        public bool RemoveViceOfficer(int officerDefId)
        {
            Officer found = FindVice(officerDefId);
            return found != null && viceOfficers.Remove(found);
        }

        internal void ClearViceOfficers() => viceOfficers.Clear();

        public void SetCommander(Officer unitCopy) => Commander = unitCopy;

        public void SetCommanderFromPool(int officerDefId) =>
            Commander = OfficerPool.CloneForUnit(officerDefId);

        public void SetMorale(short value) => Morale = Clamp0To100(value);
        public void SetStamina(short value) => Stamina = Clamp0To100(value);
        public void SetMoney(int value) => Money = value < 0 ? 0 : value;
        public void ChangeMorale(short delta) => SetMorale((short)(Morale + delta));
        public void ChangeStamina(short delta) => SetStamina((short)(Stamina + delta));

        public void SetBuilding(AbstractBuilding b) => Building = b;
        public void SetFireEffect(short value) => FireEffect = value;
        public void SetMoralePenalty(short value) => MoralePenalty = value;

        public virtual void SetManpower(int totalSoldiers, int woundedCount = 0)
        {
            Soldiers = System.Math.Max(0, totalSoldiers);
            Wounded = System.Math.Clamp(woundedCount, 0, Soldiers);
            annihilated = UnitManpower.IsAnnihilated(Soldiers);
        }

        public void SetReachableFlag(bool reachable) => UnitFlags.Reachable = reachable;

        public void SetFlameDamage(HazardDamageLevel level) =>
            UnitFlags.FlameDamage = UnitFlags.ClampHazard(level);

        public void SetTrapDamage(HazardDamageLevel level) =>
            UnitFlags.TrapDamage = UnitFlags.ClampHazard(level);

        public HazardDamageLevel FlameDamage => UnitFlags.FlameDamage;
        public HazardDamageLevel TrapDamage => UnitFlags.TrapDamage;

        /// <summary>火焰與陷阱取較重者（格傷害規則相同時用）。</summary>
        public HazardDamageLevel EnvironmentalDamage =>
            (HazardDamageLevel)System.Math.Max((byte)FlameDamage, (byte)TrapDamage);

        public bool IsOfficerAssigned(int officerDefId) =>
            officerDefId > 0 &&
            (Commander?.RuntimeId == officerDefId || FindVice(officerDefId) != null);

        public bool IsOfficerAssigned(Officer officer) =>
            officer != null && IsOfficerAssigned(officer.RuntimeId);

        Officer FindVice(int officerDefId)
        {
            foreach (Officer v in viceOfficers)
            {
                if (v.RuntimeId == officerDefId) return v;
            }
            return null;
        }

        protected int BaseFoodByHeadCount() =>
            System.Math.Max(1, Soldiers / 100);

        protected static short Clamp0To100(short v)
        {
            if (v < 0) return 0;
            if (v > 100) return 100;
            return v;
        }
    }

    public enum UnitKind
    {
        Legion,
        Combat,
        Transport,
        /// <summary>進駐城池／港灣／砦等據點的駐軍（由 <see cref="GarrisonConversion"/> 自 <see cref="Combat"/> 轉換）。</summary>
        Garrison
    }
}
