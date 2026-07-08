using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>本局劇本武將池（委派 <see cref="OfficerDatabase.Officers"/>）。</summary>
    public static class OfficerPool
    {
        public static bool IsInitialized => OfficerDatabase.IsLoaded;

        public static Officer Get(int defId) => GetShared(defId);

        public static Officer GetShared(int defId) => OfficerDatabase.TryGet(defId);

        public static void RemoveOfficer(Officer officer) => OfficerDatabase.Remove(officer);
    }
}
