using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>
    /// 本局武將存取：執行時 <see cref="Officer"/> 與表資料 <see cref="OfficerDef"/> 分池，defId 一致。
    /// </summary>
    public static class OfficerPool
    {
        public static bool IsInitialized => OfficerDatabase.IsLoaded;

        /// <summary>執行時武將（局內修改此池）。</summary>
        public static Officer Get(int defId) => GetShared(defId);

        public static Officer GetShared(int defId) => OfficerDatabase.TryGet(defId);

        /// <summary>表資料（六維原始值等；唯讀圖鑑）。</summary>
        public static OfficerDef GetDef(int defId) => OfficerDatabase.TryGetDef(defId);

        public static void RemoveOfficer(Officer officer) => OfficerDatabase.Remove(officer);
    }
}
