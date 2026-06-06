using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Core.Units
{
    /// <summary>野戰部隊進駐據點時的快照，供還原為 <see cref="Combat"/>。</summary>
    public sealed class GarrisonSnapshot
    {
        public string TroopKindKey { get; }
        public TroopType TroopType { get; }
        public CombatTroopStatBlock BaseStats { get; }
        public int Soldiers { get; }
        public int Wounded { get; }
        public byte Morale { get; }
        public byte Stamina { get; }
        public int Money { get; }
        public int CommanderOfficerId { get; }
        public int[] ViceOfficerIds { get; }

        GarrisonSnapshot(
            string troopKindKey,
            TroopType troopType,
            CombatTroopStatBlock baseStats,
            int soldiers,
            int wounded,
            byte morale,
            byte stamina,
            int money,
            int commanderOfficerId,
            int[] viceOfficerIds)
        {
            TroopKindKey = troopKindKey;
            TroopType = troopType;
            BaseStats = baseStats;
            Soldiers = soldiers;
            Wounded = wounded;
            Morale = morale;
            Stamina = stamina;
            Money = money;
            CommanderOfficerId = commanderOfficerId;
            ViceOfficerIds = viceOfficerIds ?? System.Array.Empty<int>();
        }

        public static GarrisonSnapshot Capture(Combat combat)
        {
            if (combat == null) return null;
            var vice = new int[combat.ViceOfficers.Count];
            for (int i = 0; i < vice.Length; i++)
                vice[i] = combat.ViceOfficers[i].RuntimeId;

            return new GarrisonSnapshot(
                combat.TroopKindKey,
                combat.TroopType,
                CombatStatMath.GetBaseTroopStats(combat),
                combat.Soldiers,
                combat.Wounded,
                (byte)combat.Morale,
                (byte)combat.Stamina,
                combat.Money,
                combat.Commander?.RuntimeId ?? 0,
                vice);
        }

        public CombatUnitDef ToCombatDef(int factionId)
        {
            var def = new CombatUnitDef(
                factionId,
                TroopKindKey,
                Soldiers,
                Wounded,
                Morale,
                Stamina,
                Money,
                CommanderOfficerId);
            def.TroopType = TroopType;
            foreach (int id in ViceOfficerIds)
                def.AddViceOfficer(id);
            return def;
        }
    }
}
