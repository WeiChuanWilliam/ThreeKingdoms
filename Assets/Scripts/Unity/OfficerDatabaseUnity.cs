using System.IO;
using ThreeKindoms.Data.Officers;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>Unity 端載入武將圖鑑與劇本 Pool（StreamingAssets 路徑）。</summary>
    public static class OfficerDatabaseUnity
    {
        public static void EnsureCatalogLoaded(
            string officersFile = "officers.json",
            string personalityFile = "personality_traits.json")
        {
            if (OfficerDatabase.IsCatalogLoaded)
                return;

            string root = Application.streamingAssetsPath;
            OfficerDatabase.LoadCatalog(
                Path.Combine(root, officersFile),
                Path.Combine(root, personalityFile));
            Debug.Log($"[OfficerDatabase] 圖鑑已載入 {OfficerDatabase.Defs.Count} 名武將定義");
        }

        /// <summary>載入劇本武將清單，從圖鑑 materialize 本局 Pool。</summary>
        public static void LoadScenarioRuntime(
            string scenarioOfficersFile = "scenario_officers/opening.json",
            string officersFile = "officers.json",
            string personalityFile = "personality_traits.json")
        {
            EnsureCatalogLoaded(officersFile, personalityFile);

            string root = Application.streamingAssetsPath;
            OfficerDatabase.MaterializeFromScenarioFile(Path.Combine(root, scenarioOfficersFile));
            OfficerDatabase.SyncAllRelations();
            Debug.Log($"[OfficerDatabase] 劇本 Pool {OfficerDatabase.RuntimeCount} 名（{scenarioOfficersFile}）");
        }

        /// <summary>相容舊 Spike：圖鑑全載進 runtime。</summary>
        public static void LoadAllIntoRuntime(
            string officersFile = "officers.json",
            string personalityFile = "personality_traits.json")
        {
            string root = Application.streamingAssetsPath;
            OfficerDatabase.LoadCatalogAndMaterializeAll(
                Path.Combine(root, officersFile),
                Path.Combine(root, personalityFile));
            OfficerDatabase.SyncAllRelations();
            Debug.Log($"[OfficerDatabase] 圖鑑 {OfficerDatabase.Defs.Count} 名，runtime 全載");
        }
    }
}
