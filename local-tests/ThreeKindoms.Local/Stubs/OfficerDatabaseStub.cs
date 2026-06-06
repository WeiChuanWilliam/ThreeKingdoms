using ThreeKindoms.Core.Officers;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>本機測試占位；Unity 使用 <see cref="OfficerDatabase"/> 真實載入。</summary>
    public sealed class OfficerDatabase
    {
        public Officer GetOrCreateRuntime(int defId) => null;

        public static OfficerDatabase LoadFromStreamingAssets(string fileName = "officers.json") =>
            new OfficerDatabase();
    }
}
