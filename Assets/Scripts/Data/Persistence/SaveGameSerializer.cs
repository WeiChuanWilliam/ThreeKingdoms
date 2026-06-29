using System.Collections.Generic;
using System.IO;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using UnityEngine;

namespace ThreeKindoms.Data.Persistence
{
    public static class SaveGameSerializer
    {
        public static GameSaveDocument Capture(
            IEnumerable<(Unit unit, HexCoord hex)> unitsOnMap,
            string sourceScenarioId = "",
            int gameDay = 0)
        {
            var list = new List<UnitSaveEntry>();
            foreach ((Unit unit, HexCoord hex) in unitsOnMap)
            {
                if (unit != null)
                    list.Add(CaptureUnit(unit, hex));
            }

            return new GameSaveDocument
            {
                formatVersion = 1,
                sourceScenarioId = sourceScenarioId ?? "",
                gameDay = gameDay,
                units = list.ToArray()
            };
        }

        public static string ToJson(GameSaveDocument doc, bool prettyPrint = true) =>
            prettyPrint ? JsonUtility.ToJson(doc, true) : JsonUtility.ToJson(doc);

        public static GameSaveDocument FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new GameSaveDocument();
            return JsonUtility.FromJson<GameSaveDocument>(json) ?? new GameSaveDocument();
        }

        public static void WriteFile(GameSaveDocument doc, string absolutePath)
        {
            string dir = Path.GetDirectoryName(absolutePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);
            File.WriteAllText(absolutePath, ToJson(doc));
        }

        public static GameSaveDocument ReadFile(string absolutePath)
        {
            if (!File.Exists(absolutePath)) return new GameSaveDocument();
            return FromJson(File.ReadAllText(absolutePath));
        }

        public static List<(Unit unit, HexCoord hex)> RestoreUnits(GameSaveDocument doc)
        {
            var result = new List<(Unit, HexCoord)>();
            if (doc?.units == null) return result;

            foreach (UnitSaveEntry entry in doc.units)
            {
                Unit unit = RestoreUnit(entry);
                if (unit != null)
                    result.Add((unit, new HexCoord(entry.hexQ, entry.hexR)));
            }

            return result;
        }

        static UnitSaveEntry CaptureUnit(Unit unit, HexCoord hex)
        {
            var entry = new UnitSaveEntry
            {
                saveId = unit.UnitName,
                unitName = unit.UnitName,
                faction = unit.Belonged,
                hexQ = hex.Q,
                hexR = hex.R,
                soldiers = unit.Soldiers,
                wounded = unit.Wounded,
                morale = unit.Morale,
                stamina = unit.Stamina,
                isStationed = unit.IsStationed,
                commander = CaptureOfficer(unit.Commander)
            };

            var vice = new List<OfficerSaveEntry>();
            foreach (Officer v in unit.ViceOfficers)
                vice.Add(CaptureOfficer(v));
            entry.vice = vice.ToArray();

            var battle = new List<SkillSaveEntry>();
            var strategy = new List<SkillSaveEntry>();
            var mobility = new List<SkillSaveEntry>();
            var defence = new List<SkillSaveEntry>();

            switch (unit)
            {
                case Combat combat:
                    entry.type = "combat";
                    entry.troopType = (int)combat.TroopType;
                    combat.CollectEquippedSkills(battle, strategy, mobility, defence);
                    break;
                case Legion legion:
                    entry.type = "legion";
                    entry.carriedFood = legion.CarriedFood;
                    break;
                case Transport transport:
                    entry.type = "transport";
                    transport.CollectEquippedSkills(strategy);
                    break;
            }

            entry.battleSkills = battle.ToArray();
            entry.strategySkills = strategy.ToArray();
            entry.mobilitySkills = mobility.ToArray();
            entry.defenceSkills = defence.ToArray();
            return entry;
        }

        static OfficerSaveEntry CaptureOfficer(Officer o)
        {
            if (o == null) return null;
            return new OfficerSaveEntry
            {
                defId = o.RuntimeId,
                stamina = o.Stamina,
                loyalty = o.Loyalty,
                belong = o.Belong
            };
        }

        static Unit RestoreUnit(UnitSaveEntry entry)
        {
            if (entry == null) return null;

            UnitDef def = BuildDefFromSave(entry);
            Unit unit = def switch
            {
                LegionUnitDef legionDef => new Legion(legionDef),
                CombatUnitDef c => new Combat(c),
                TransportUnitDef t => new Transport(t),
                _ => null
            };

            if (unit == null) return null;
            ApplyRuntimeState(unit, entry);
            return unit;
        }

        static void ApplyRuntimeState(Unit unit, UnitSaveEntry entry)
        {
            ApplyOfficerSnapshots(unit, entry);
            unit.SetManpower(entry.soldiers, entry.wounded);
            unit.SetMorale(entry.morale);
            unit.SetStamina(entry.stamina);
            unit.SetStationed(entry.isStationed);

            if (unit is Legion legion)
                legion.SetCarriedFood(entry.carriedFood);

            if (unit is Combat combat)
            {
                RestoreSkills(entry.battleSkills, combat.AddBattleSkill);
                RestoreSkills(entry.strategySkills, combat.AddStrategySkill);
                RestoreSkills(entry.mobilitySkills, combat.AddMobilitySkill);
                RestoreSkills(entry.defenceSkills, combat.AddDefenceSkill);
            }
            else if (unit is Transport transport)
            {
                RestoreSkills(entry.strategySkills, transport.AddStrategySkill);
            }
        }

        static UnitDef BuildDefFromSave(UnitSaveEntry entry)
        {
            string type = entry.type?.ToLowerInvariant() ?? "combat";
            UnitDef def = type switch
            {
                "legion" => new LegionUnitDef(entry.faction) { Food = entry.carriedFood },
                "transport" => new TransportUnitDef(entry.faction),
                _ => new CombatUnitDef(entry.faction)
            };

            if (!string.IsNullOrWhiteSpace(entry.unitName))
                def.CustomUnitName = entry.unitName;
            if (entry.commander != null)
                def.CommanderOfficerId = entry.commander.defId;
            foreach (OfficerSaveEntry v in entry.vice ?? System.Array.Empty<OfficerSaveEntry>())
            {
                if (v != null && v.defId > 0) def.AddViceOfficer(v.defId);
            }

            if (def is CombatUnitDef combat)
                combat.TroopType = (TroopType)entry.troopType;

            return def;
        }

        static void ApplyOfficerSnapshots(Unit unit, UnitSaveEntry entry)
        {
            if (entry.commander != null)
            {
                Officer cmd = RestoreOfficer(entry.commander);
                if (cmd != null) unit.SetCommander(cmd);
            }

            unit.ClearViceOfficers();
            foreach (OfficerSaveEntry v in entry.vice ?? System.Array.Empty<OfficerSaveEntry>())
            {
                Officer copy = RestoreOfficer(v);
                if (copy != null) unit.AddViceOfficer(copy);
            }
        }

        static Officer RestoreOfficer(OfficerSaveEntry snap)
        {
            if (snap == null || snap.defId <= 0) return null;
            Officer officer = OfficerPool.Get(snap.defId);
            if (officer == null) return null;
            officer.SetStamina(snap.stamina);
            officer.SetBelong(snap.belong, snap.loyalty);
            return officer;
        }

        static void RestoreSkills(SkillSaveEntry[] skills, System.Func<int, bool> addById)
        {
            if (skills == null) return;
            foreach (SkillSaveEntry s in skills)
            {
                if (s.skillId > 0) addById(s.skillId);
            }
        }
    }
}
