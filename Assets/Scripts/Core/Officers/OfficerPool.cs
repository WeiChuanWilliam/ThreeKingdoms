using ThreeKindoms.Data.Officers;



namespace ThreeKindoms.Core.Officers

{

    /// <summary>本局劇本武將池。部隊主將／副將直接 <see cref="Get"/> 引用，不複製。</summary>

    public static class OfficerPool

    {

        static OfficerDatabase database;



        public static bool IsInitialized => database != null;



        public static void Initialize(OfficerDatabase db) => database = db;



        /// <summary>池內 Singleton（會隨遊戲進度變動）。</summary>

        public static Officer Get(int defId) => GetShared(defId);



        public static Officer GetShared(int defId) =>

            database?.GetOrCreateRuntime(defId);



        // TODO: RemoveOfficer(defId) — SetAlive(false) 時由劇本 Pool 呼叫，移出本局可玩武將。

    }

}

