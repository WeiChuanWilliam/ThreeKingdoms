using System;
using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Data.Units
{
    /// <summary>
    /// 部隊產生入口。
    /// 頂層 <see cref="Create"/> 先收齊組隊資料並解析名稱，再依 <see cref="UnitKind"/> 分流到具體產生函數。
    /// 目前僅實作 Combat；Legion／Transport 尚未實作。
    /// </summary>
    public static class UnitUtil
    {
        public const byte DefaultMorale = 100;
        public const byte DefaultStamina = 100;
        public const int DefaultMoney = 0;
        public const int DefaultSoldiers = 5000;

        /// <summary>
        /// 頂層組隊：吃齊資料 → 解析部隊名 → 依 <paramref name="kind"/> 產生對應部隊。
        /// </summary>
        /// <param name="kind">部隊種類（Combat／Legion／Transport）。</param>
        /// <param name="factionId">
        /// 勢力 id（寫入 <see cref="Unit.Belonged"/>）。
        /// 約定：＝執掌該勢力的武將 defId（與 <see cref="Officer.Belong"/> 相同語意；劉備軍＝1）。
        /// </param>
        /// <param name="commanderOfficerId">主將武將 defId（OfficerPool）。</param>
        /// <param name="troopKindKey">兵種鍵（如 blade）；Combat 必填語意，缺則用預設步兵種。</param>
        /// <param name="viceOfficerIds">副將 id 陣列；Combat 只取第一個有效 id。</param>
        public static Unit Create(
            UnitKind kind,
            int factionId,
            int commanderOfficerId,
            string troopKindKey = null,
            IReadOnlyList<int> viceOfficerIds = null,
            int soldiers = DefaultSoldiers,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            string customUnitName = null)
        {
            string unitName = ResolveUnitName(customUnitName, commanderOfficerId, troopKindKey, kind);

            return kind switch
            {
                UnitKind.Combat => CreateCombat(
                    unitName,
                    factionId,
                    troopKindKey,
                    soldiers,
                    commanderOfficerId,
                    viceOfficerIds,
                    wounded,
                    morale,
                    stamina,
                    money),
                UnitKind.Legion => throw new NotSupportedException("兵團產生尚未實作（目前專注 Combat）。"),
                UnitKind.Transport => throw new NotSupportedException("運輸產生尚未實作（目前專注 Combat）。"),
                _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
            };
        }

        /// <summary>
        /// 頂層組隊並部署到地圖（不自動駐紮，可立即移動）。
        /// </summary>
        public static Unit Deploy(
            UnitKind kind,
            int factionId,
            int commanderOfficerId,
            LocationGrid grid,
            HexCoord spawnHex,
            AbstractTerrain terrainAtSpawn,
            string troopKindKey = null,
            IReadOnlyList<int> viceOfficerIds = null,
            int soldiers = DefaultSoldiers,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            string customUnitName = null)
        {
            if (grid == null)
                throw new ArgumentNullException(nameof(grid));

            Unit unit = Create(
                kind,
                factionId,
                commanderOfficerId,
                troopKindKey,
                viceOfficerIds,
                soldiers,
                wounded,
                morale,
                stamina,
                money,
                customUnitName);

            unit.Location.BindToWorld(grid, spawnHex, terrainAtSpawn, autoGarrison: false);
            return unit;
        }

        /// <summary>便捷：產生 Combat（等同 <see cref="Create"/>(UnitKind.Combat, …)）。</summary>
        public static Combat CreateCombat(
            int factionId,
            string troopKindKey,
            int soldiers,
            int commanderOfficerId,
            IReadOnlyList<int> viceOfficerIds = null,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            string customUnitName = null) =>
            (Combat)Create(
                UnitKind.Combat,
                factionId,
                commanderOfficerId,
                troopKindKey,
                viceOfficerIds,
                soldiers,
                wounded,
                morale,
                stamina,
                money,
                customUnitName);

        /// <summary>便捷：部署 Combat。</summary>
        public static Combat DeployCombat(
            int factionId,
            string troopKindKey,
            int soldiers,
            int commanderOfficerId,
            IReadOnlyList<int> viceOfficerIds,
            LocationGrid grid,
            HexCoord spawnHex,
            AbstractTerrain terrainAtSpawn,
            int wounded = 0,
            byte morale = DefaultMorale,
            byte stamina = DefaultStamina,
            int money = DefaultMoney,
            string customUnitName = null) =>
            (Combat)Deploy(
                UnitKind.Combat,
                factionId,
                commanderOfficerId,
                grid,
                spawnHex,
                terrainAtSpawn,
                troopKindKey,
                viceOfficerIds,
                soldiers,
                wounded,
                morale,
                stamina,
                money,
                customUnitName);

        /// <summary>
        /// 部隊顯示名：自訂名優先；否則
        /// <c>主將名 + 兵種顯示名 + properties 後綴</c>
        ///（Combat 後綴＝<c>suffix.company</c>，預設「隊」→ 例：劉備刀兵隊）。
        /// </summary>
        public static string ResolveUnitName(
            string customUnitName,
            int commanderOfficerId,
            string troopKindKey,
            UnitKind kind)
        {
            if (!string.IsNullOrWhiteSpace(customUnitName))
                return customUnitName.Trim();

            string commanderName = GetCommanderDisplayName(commanderOfficerId);
            string troopLabel = string.IsNullOrWhiteSpace(troopKindKey)
                ? ""
                : UnitConfigUtil.GetKindDisplayName(troopKindKey);
            string suffix = UnitNamingSettings.GetSuffix(kind);

            if (!string.IsNullOrEmpty(commanderName) && !string.IsNullOrEmpty(troopLabel))
                return commanderName + troopLabel + suffix;
            if (!string.IsNullOrEmpty(commanderName))
                return commanderName + suffix;
            if (!string.IsNullOrEmpty(troopLabel))
                return troopLabel + suffix;
            return UnitConfigUtil.FallbackUnitName + suffix;
        }

        /// <summary>Combat 只帶一位副將：取陣列中第一個 &gt; 0 的 id。</summary>
        public static int PickPrimaryViceId(IReadOnlyList<int> viceOfficerIds)
        {
            if (viceOfficerIds == null)
                return 0;
            for (int i = 0; i < viceOfficerIds.Count; i++)
            {
                if (viceOfficerIds[i] > 0)
                    return viceOfficerIds[i];
            }
            return 0;
        }

        /// <summary>實際組裝 Combat（由頂層 <see cref="Create"/> 呼叫）。</summary>
        static Combat CreateCombat(
            string unitName,
            int factionId,
            string troopKindKey,
            int soldiers,
            int commanderOfficerId,
            IReadOnlyList<int> viceOfficerIds,
            int wounded,
            byte morale,
            byte stamina,
            int money)
        {
            var combat = new Combat(unitName, factionId);
            combat.SetGarrison(false);
            combat.SetManpower(soldiers, wounded);
            combat.SetMorale(morale);
            combat.SetStamina(stamina);
            combat.SetMoney(money);

            BindTroopKind(combat, troopKindKey);

            if (commanderOfficerId > 0)
                combat.SetCommanderFromPool(commanderOfficerId);
            else
                combat.SetCommander(null);

            int viceId = PickPrimaryViceId(viceOfficerIds);
            if (viceId > 0)
                combat.SetViceOfficerFromPool(viceId);

            return combat;
        }

        static void BindTroopKind(Combat combat, string troopKindKey)
        {
            string kindKey = troopKindKey;
            if (string.IsNullOrWhiteSpace(kindKey))
                kindKey = UnitConfigUtil.GetDefaultInfantryKindKey();

            if (!string.IsNullOrWhiteSpace(kindKey) &&
                TroopKindRegistry.TryGet(kindKey, out AbstractTroopKind kind))
                combat.BindTroopKind(kind);
        }

        static string GetCommanderDisplayName(int commanderDefId)
        {
            if (commanderDefId <= 0)
                return "";
            Officer shared = OfficerPool.GetShared(commanderDefId);
            if (shared == null)
                return $"{UnitConfigUtil.FallbackOfficerPrefix}{commanderDefId}";
            return shared.FullName;
        }
    }
}
