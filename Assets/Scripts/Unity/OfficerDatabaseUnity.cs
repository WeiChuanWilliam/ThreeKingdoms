using System.IO;
using ThreeKindoms.Data.Officers;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>Unity 端載入武將表（StreamingAssets 路徑）。</summary>
    public static class OfficerDatabaseUnity
    {
        public static OfficerDatabase LoadFromStreamingAssets(
            string officersFile = "officers.json",
            string personalityFile = "personality_traits.json")
        {
            string root = Application.streamingAssetsPath;
            string officersPath = Path.Combine(root, officersFile);
            string personalityPath = Path.Combine(root, personalityFile);
            var db = OfficerDatabase.LoadFromFile(officersPath, personalityPath);
            Debug.Log($"[OfficerDatabase] 已載入 {db.Defs.Count} 筆武將定義");
            return db;
        }
    }
}
