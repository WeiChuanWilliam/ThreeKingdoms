using ThreeKindoms.Data.Officers;



namespace ThreeKindoms.Core.Officers

{

    /// <summary>本局劇本武將池。部隊主將／副將直接 <see cref="Get"/> 引用，不複製。</summary>

    public static class OfficerPool

    {

        static OfficerDatabase database;



        /// <summary>本局武將池是否已載入資料庫。</summary>
        public static bool IsInitialized => database != null;



        /// <summary>以劇本武將資料庫初始化本局 Pool。</summary>
        public static void Initialize(OfficerDatabase db) => database = db;



        /// <summary>池內 Singleton（會隨遊戲進度變動）。</summary>

        public static Officer Get(int defId) => GetShared(defId);



        /// <summary>取得或建立執行時武將 Singleton（供關係同步等內部用途）。</summary>
        public static Officer GetShared(int defId) =>

            database?.GetOrCreateRuntime(defId);



        // TODO: RemoveOfficer(defId) — SetAlive(false) 時由劇本 Pool 呼叫，移出本局可玩武將。
        public static void RemoveOfficer(Officer officer)
        {
            
            
        }
        
    }

}

