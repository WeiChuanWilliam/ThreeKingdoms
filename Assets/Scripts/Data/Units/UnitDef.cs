using System.Collections.Generic;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Data.Units
{
    /// <summary>
    /// 組隊畫面／劇情指令產生的「建軍草稿」（記憶體物件，不讀檔）。
    /// 預設士氣／體力／金錢在建構子參數；存檔／劇情仍可事後改屬性。
    /// </summary>
    public abstract class UnitDef
    {
        public const byte DefaultMorale = 100;
        public const byte DefaultStamina = 100;
        public const int DefaultMoney = 0;

        readonly HashSet<int> viceOfficerIds = new();

        public int Belonged { get; }

        public string CustomUnitName { get; set; }
        public int CommanderOfficerId { get; set; }
        public int Soldiers { get; set; }
        public int Wounded { get; set; }
        public byte Morale { get; set; }
        public byte Stamina { get; set; }
        public int Money { get; set; }

        protected UnitDef(
            int belonged,
            int soldiers = 0,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            int commanderOfficerId = 0,
            string customUnitName = null)
        {
            Belonged = belonged;
            Soldiers = soldiers;
            Wounded = wounded;
            Morale = morale;
            Stamina = stamina;
            Money = money;
            CommanderOfficerId = commanderOfficerId;
            CustomUnitName = customUnitName;
        }

        public bool AddViceOfficer(int id) => id > 0 && viceOfficerIds.Add(id);
        public bool RemoveViceOfficer(int id) => viceOfficerIds.Remove(id);

        internal void ApplyCommonTo(Unit unit)
        {
            unit.SetManpower(Soldiers, Wounded);
            unit.SetMorale(Morale);
            unit.SetStamina(Stamina);
            unit.SetMoney(Money);
            if (CommanderOfficerId > 0)
                unit.SetCommanderFromPool(CommanderOfficerId);
            foreach (int id in viceOfficerIds)
                unit.AddViceOfficerFromPool(id);
        }
    }

    public sealed class CombatUnitDef : UnitDef
    {
        public const int DefaultSoldiers = 5000;

        /// <summary>副將（戰鬥部隊至多一位）。</summary>
        public int ViceOfficerId { get; set; }

        readonly HashSet<int> battleSkillIds = new();
        readonly HashSet<int> strategySkillIds = new();
        readonly HashSet<int> mobilitySkillIds = new();
        readonly HashSet<int> defenceSkillIds = new();

        public TroopType TroopType { get; set; }
        public string TroopKindKey { get; set; }

        public CombatUnitDef(
            int belonged,
            string troopKindKey = null,
            int soldiers = DefaultSoldiers,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            int commanderOfficerId = 0,
            string customUnitName = null)
            : base(belonged, soldiers, wounded, morale, stamina, money, commanderOfficerId, customUnitName)
        {
            TroopKindKey = troopKindKey;
            if (!string.IsNullOrWhiteSpace(troopKindKey) &&
                TroopKindRegistry.TryGet(troopKindKey, out AbstractTroopKind kind))
                TroopType = kind.Category;
        }

        public bool AddBattleSkill(int id) => id > 0 && battleSkillIds.Add(id);
        public bool RemoveBattleSkill(int id) => battleSkillIds.Remove(id);
        public bool AddStrategySkill(int id) => id > 0 && strategySkillIds.Add(id);
        public bool RemoveStrategySkill(int id) => strategySkillIds.Remove(id);
        public bool AddMobilitySkill(int id) => id > 0 && mobilitySkillIds.Add(id);
        public bool RemoveMobilitySkill(int id) => mobilitySkillIds.Remove(id);
        public bool AddDefenceSkill(int id) => id > 0 && defenceSkillIds.Add(id);
        public bool RemoveDefenceSkill(int id) => defenceSkillIds.Remove(id);

        internal void ApplyTo(Combat unit)
        {
            unit.SetManpower(Soldiers, Wounded);
            unit.SetMorale(Morale);
            unit.SetStamina(Stamina);
            unit.SetMoney(Money);
            if (CommanderOfficerId > 0)
                unit.SetCommanderFromPool(CommanderOfficerId);
            else
                unit.SetCommander(null);

            if (ViceOfficerId > 0)
                unit.SetViceOfficerFromPool(ViceOfficerId);

            string kindKey = TroopKindKey;
            if (string.IsNullOrWhiteSpace(kindKey) && TroopType == TroopType.Infantry)
                kindKey = UnitConfigUtil.GetDefaultInfantryKindKey();

            if (!string.IsNullOrWhiteSpace(kindKey) &&
                TroopKindRegistry.TryGet(kindKey, out AbstractTroopKind kind))
                unit.BindTroopKind(kind);
            else
                unit.SetTroopType(TroopType);

            unit.RefreshSkillsFromOfficers();

            foreach (int id in battleSkillIds) unit.AddBattleSkill(id);
            foreach (int id in strategySkillIds) unit.AddStrategySkill(id);
            foreach (int id in mobilitySkillIds) unit.AddMobilitySkill(id);
            foreach (int id in defenceSkillIds) unit.AddDefenceSkill(id);
        }

        /// <summary>部隊名＝properties 顯示名＋「隊」（如 鬥艦隊、工兵隊）。</summary>
        public static CombatUnitDef FromTroopKind(
            int factionId,
            string troopKindKey,
            int soldiers = DefaultSoldiers,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            int commanderOfficerId = 0)
        {
            string unitName = UnitConfigUtil.GetKindDisplayName(troopKindKey) +
                              UnitNamingSettings.GetSuffix(UnitKind.Combat);
            return new CombatUnitDef(
                factionId,
                troopKindKey,
                soldiers,
                morale: morale,
                stamina: stamina,
                money: money,
                commanderOfficerId: commanderOfficerId,
                customUnitName: unitName);
        }
    }

    public sealed class GarrisonUnitDef : UnitDef
    {
        public GarrisonSnapshot Snapshot { get; }
        public SettlementSiteKind SiteKind { get; }
        public string UnitDisplayName { get; }
        public AbstractBuilding StationSite { get; }

        GarrisonUnitDef(
            int belonged,
            GarrisonSnapshot snapshot,
            AbstractBuilding stationSite,
            string unitDisplayName)
            : base(
                belonged,
                snapshot.Soldiers,
                snapshot.Wounded,
                snapshot.Morale,
                snapshot.Stamina,
                snapshot.Money,
                snapshot.CommanderOfficerId,
                unitDisplayName)
        {
            Snapshot = snapshot;
            StationSite = stationSite;
            SiteKind = stationSite?.SiteKind ?? SettlementSiteKind.None;
            UnitDisplayName = unitDisplayName;
            foreach (int id in snapshot.ViceOfficerIds)
                AddViceOfficer(id);
        }

        public static GarrisonUnitDef FromCombat(Combat combat, AbstractBuilding stationSite)
        {
            GarrisonSnapshot snap = GarrisonSnapshot.Capture(combat);
            string suffix = UnitNamingSettings.GetSuffix(UnitKind.Garrison);
            string siteName = stationSite?.Name ?? UnitConfigUtil.FallbackUnitName;
            string display = siteName + suffix;
            return new GarrisonUnitDef(combat.Belonged, snap, stationSite, display);
        }

        internal void ApplyTo(Garrison unit)
        {
            ApplyCommonTo(unit);
            unit.ApplyTroopStats(Snapshot);
            if (StationSite != null)
                unit.SetBuilding(StationSite);
        }
    }

    public sealed class LegionUnitDef : UnitDef
    {
        public int EscortCommanderOfficerId { get; set; }
        public float EscortSoldierRatio { get; set; } = 0.5f;
        public int Food { get; set; }

        public LegionUnitDef(
            int belonged,
            int soldiers = 0,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            int commanderOfficerId = 0,
            string customUnitName = null)
            : base(belonged, soldiers, wounded, morale, stamina, money, commanderOfficerId, customUnitName)
        {
        }

        internal void ApplyTo(Legion unit) => ApplyCommonTo(unit);

        internal CombatUnitDef CreateEscortDef()
        {
            int escortCmd = EscortCommanderOfficerId > 0 ? EscortCommanderOfficerId : CommanderOfficerId;
            int escortSoldiers = System.Math.Max(
                UnitManpower.MinSoldiers,
                (int)(Soldiers * EscortSoldierRatio));

            return new CombatUnitDef(Belonged, soldiers: escortSoldiers, morale: Morale, stamina: Stamina, money: Money, commanderOfficerId: escortCmd)
            {
                TroopType = TroopType.Cavalry
            };
        }
    }

    public sealed class TransportUnitDef : UnitDef
    {
        readonly HashSet<int> strategySkillIds = new();

        public TransportUnitDef(
            int belonged,
            int soldiers = 0,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            int commanderOfficerId = 0,
            string customUnitName = null)
            : base(belonged, soldiers, wounded, morale, stamina, money, commanderOfficerId, customUnitName)
        {
        }

        public bool AddStrategySkill(int id) => id > 0 && strategySkillIds.Add(id);
        public bool RemoveStrategySkill(int id) => strategySkillIds.Remove(id);

        internal void ApplyTo(Transport unit)
        {
            ApplyCommonTo(unit);
            foreach (int id in strategySkillIds)
                unit.AddStrategySkill(id);
        }
    }
}
