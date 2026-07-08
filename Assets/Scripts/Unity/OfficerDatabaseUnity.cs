using System.IO;
using ThreeKindoms.Data.Officers;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>Unity 端自 StreamingAssets 載入本劇本武將池。</summary>
    public static class OfficerDatabaseUnity
    {
        /// <summary>載入 officers.json 內全部武將至單一 Pool。</summary>
        public static void LoadScenario(
            string officersFile = "officers.json",
            string personalityFile = "personality_traits.json")
        {
            if (OfficerDatabase.IsLoaded)
                return;

            string root = Application.streamingAssetsPath;
            OfficerDatabase.Load(
                Path.Combine(root, officersFile),
                Path.Combine(root, personalityFile));
            Debug.Log($"[OfficerDatabase] 劇本武將池 {OfficerDatabase.Count} 名（{officersFile}）");
        }
    }
}
