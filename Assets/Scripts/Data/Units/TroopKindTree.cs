using System.Collections.Generic;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Data.Units
{
    /// <summary>
    /// 兵種「升級樹」：只記錄父子關係與水軍下一階，不讀 properties。
    /// <para><b>TroopType 定義在：</b> <see cref="TroopType"/>（TroopType.cs），不是本檔。</para>
    /// <para><b>中文名稱讀檔：</b> <see cref="UnitConfigUtil.Load"/> + <see cref="UnityBridge.GamePaths.ChineseUnitProperties"/>，與本檔無關。</para>
    /// <para><b>樹的結構：</b> 在 <see cref="Build"/> 裡用 <see cref="Add"/> / <see cref="AddLine"/> 手寫登記（目前未從 unit.properties 載入）。</para>
    /// </summary>
    public static class TroopKindTree
    {
        static readonly Dictionary<string, TroopKindNode> nodes = Build();

        public static bool TryGetNode(string kindKey, out TroopKindNode node) =>
            nodes.TryGetValue(kindKey ?? "", out node);

        public static int GetStage(string kindKey)
        {
            if (!TryGetNode(kindKey, out TroopKindNode node))
                return 1;
            int stage = 1;
            string parent = node.ParentKey;
            while (!string.IsNullOrEmpty(parent))
            {
                stage++;
                if (!TryGetNode(parent, out TroopKindNode p))
                    break;
                parent = p.ParentKey;
            }
            return stage;
        }

        public static IReadOnlyList<string> GetChildren(string parentKey)
        {
            var list = new List<string>();
            foreach (var kv in nodes)
            {
                if (kv.Value.ParentKey == parentKey)
                    list.Add(kv.Key);
            }
            list.Sort();
            return list;
        }

        public static string GetNavyNext(string kindKey) =>
            TryGetNode(kindKey, out TroopKindNode n) ? n.NextLinear : null;

        /// <summary>建立整棵樹（硬編碼）。若要改分叉請改此方法或改為讀 properties。</summary>
        static Dictionary<string, TroopKindNode> Build()
        {
            var map = new Dictionary<string, TroopKindNode>();

            // --- 步：根 blade（刀兵），兩條 4 階線（槍 / 甲）---
            Add(map, TroopKindKeys.Blade, TroopType.Infantry, parent: null);
            AddLine(map, TroopType.Infantry, root: TroopKindKeys.Blade, branch: "spear", advance: "spear.advance", leaf1: "spear.qingzhou", leaf2: "spear.daji");
            AddLine(map, TroopType.Infantry, root: TroopKindKeys.Blade, branch: "armor", advance: "armor.advance", leaf1: "armor.xianzhen", leaf2: "armor.baimao");

            // --- 騎 ---
            Add(map, "cavalry", TroopType.Cavalry, parent: null);
            AddLine(map, TroopType.Cavalry, root: "cavalry", branch: "knight", advance: "knight.advance", leaf1: "knight.hubao", leaf2: "knight.xilian");
            AddLine(map, TroopType.Cavalry, root: "cavalry", branch: "horseman", advance: "horseman.advance", leaf1: "horseman.bingzhou", leaf2: "horseman.baima");

            // --- 弓 ---
            Add(map, "archer", TroopType.Archer, parent: null);
            AddLine(map, TroopType.Archer, root: "archer", branch: "bow", advance: "bow.advance", leaf1: "bow.wudan", leaf2: "bow.danyang");
            AddLine(map, TroopType.Archer, root: "archer", branch: "crossbow", advance: "crossbow.advance", leaf1: "crossbow.xiandeng", leaf2: "crossbow.zhuge");

            // --- 器械：兩條幹（各 2 子，沒有 advance 層）---
            Add(map, "siege.charger", TroopType.Siege, parent: null);
            Add(map, "siege.mushou", TroopType.Siege, parent: "siege.charger");
            Add(map, "siege.elephant", TroopType.Siege, parent: "siege.charger");
            Add(map, "siege.shooter", TroopType.Siege, parent: null);
            Add(map, "siege.stone", TroopType.Siege, parent: "siege.shooter");
            Add(map, "siege.crossbow", TroopType.Siege, parent: "siege.shooter");

            // --- 水軍：一條線，用 NextLinear ---
            Add(map, "navy.small", TroopType.Navy, parent: null, nextLinear: "navy.medium");
            Add(map, "navy.medium", TroopType.Navy, parent: "navy.small", nextLinear: "navy.large");
            Add(map, "navy.large", TroopType.Navy, parent: "navy.medium", nextLinear: "navy.final");
            Add(map, "navy.final", TroopType.Navy, parent: "navy.large", nextLinear: null);

            return map;
        }

        /// <summary>登記一個節點：key 的父節點是 parent，大類是 type。</summary>
        static void Add(Dictionary<string, TroopKindNode> map, string key, TroopType type, string parent, string nextLinear = null) =>
            map[key] = new TroopKindNode(key, type, parent, nextLinear);

        /// <summary>
        /// 登記「4 階 Y 線」：root → branch → advance → 兩個葉子（兄弟）。
        /// 例：infantry → spear → spear.advance → qingzhou / daji
        /// </summary>
        static void AddLine(Dictionary<string, TroopKindNode> map, TroopType type,
            string root, string branch, string advance, string leaf1, string leaf2)
        {
            Add(map, branch, type, parent: root);
            Add(map, advance, type, parent: branch);
            Add(map, leaf1, type, parent: advance);
            Add(map, leaf2, type, parent: advance);
        }
    }

    /// <summary>樹上一筆資料（不是兵種 class）。</summary>
    public readonly struct TroopKindNode
    {
        public string Key { get; }
        public TroopType TroopType { get; }
        public string ParentKey { get; }
        public string NextLinear { get; }

        public TroopKindNode(string key, TroopType type, string parentKey, string nextLinear)
        {
            Key = key;
            TroopType = type;
            ParentKey = parentKey;
            NextLinear = nextLinear;
        }
    }
}
