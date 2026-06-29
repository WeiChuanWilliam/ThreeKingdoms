using System;

namespace ThreeKindoms.Data.Persistence
{
    /// <summary>玩家存檔 JSON（執行中狀態快照，不是劇本）。</summary>
    [Serializable]
    public class GameSaveDocument
    {
        public int formatVersion = 1;
        public string sourceScenarioId = "";
        public int gameDay;
        public UnitSaveEntry[] units = Array.Empty<UnitSaveEntry>();
    }

    [Serializable]
    public class UnitSaveEntry
    {
        public string saveId = "";
        public string type = "combat";
        public string unitName = "";
        public int faction;
        public int hexQ;
        public int hexR;

        public int soldiers;
        public int wounded;
        public short morale;
        public short stamina;
        public bool isStationed;

        public OfficerSaveEntry commander;
        public OfficerSaveEntry[] vice = Array.Empty<OfficerSaveEntry>();

        public int troopType;
        public SkillSaveEntry[] battleSkills = Array.Empty<SkillSaveEntry>();
        public SkillSaveEntry[] strategySkills = Array.Empty<SkillSaveEntry>();
        public SkillSaveEntry[] mobilitySkills = Array.Empty<SkillSaveEntry>();
        public SkillSaveEntry[] defenceSkills = Array.Empty<SkillSaveEntry>();

        /// <summary>Legion 專用：攜帶軍糧。</summary>
        public int carriedFood;
    }

    [Serializable]
    public class OfficerSaveEntry
    {
        public int defId;
        public short stamina;
        public short loyalty;
        public short belong;
    }

    [Serializable]
    public class SkillSaveEntry
    {
        public int skillId;
        public byte level;
        public bool enabled = true;
    }
}
