using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>
    /// 全域武將池（勢力／在野的「本尊」）。指派到部隊時請用 <see cref="CloneForUnit"/>，勿把池內實例直接塞進 Unit。
    /// </summary>
    public static class OfficerPool
    {
        static OfficerDatabase database;

        public static bool IsInitialized => database != null;

        public static void Initialize(OfficerDatabase db) => database = db;

        /// <summary>池內共享實例（會隨遊戲進度變動）。</summary>
        public static Officer GetShared(int defId) =>
            database?.GetOrCreateRuntime(defId);

        /// <summary>從池複製一份給部隊用，不影響池內資料。</summary>
        public static Officer CloneForUnit(int defId)
        {
            Officer shared = GetShared(defId);
            return shared == null ? null : shared.CloneForUnit();
        }
    }
}
