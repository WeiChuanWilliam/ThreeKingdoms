using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>本局劇本武將池（委派 <see cref="OfficerDatabase.Runtime"/>）。部隊主將／副將直接 <see cref="Get"/> 引用，不複製。</summary>
    public static class OfficerPool
    {
        public static bool IsInitialized => OfficerDatabase.IsRuntimeLoaded;

        public static Officer Get(int defId) => GetShared(defId);

        public static Officer GetShared(int defId) => OfficerDatabase.TryGetRuntime(defId);

        public static void RemoveOfficer(Officer officer) => OfficerDatabase.RemoveFromRuntime(officer);
    }
}
