using System;

namespace ThreeKindoms.Data.Scenario
{
    /// <summary>企劃／開發用劇本 JSON（開局狀態，不是存檔）。Unity JsonUtility 用陣列。</summary>
    [Serializable]
    public class ScenarioDocument
    {
        public string scenarioId = "opening";
        public string displayName = "";
        public ScenarioUnitEntry[] units = Array.Empty<ScenarioUnitEntry>();
    }

    [Serializable]
    public class ScenarioUnitEntry
    {
        public string spawnKey = "player";
        public string type = "combat";
        public int faction = 1;
        public int commander;
        public int escortCommander;
        public int[] vice = Array.Empty<int>();
        public int soldiers = 1000;
        public int wounded;
        public byte morale = 100;
        public byte stamina = 100;
        public int troopType;
        public string customName = "";
        public int hexQ;
        public int hexR;
    }
}
