using System;
using System.Collections.Generic;

namespace ThreeKindoms.Data.Units.TroopKinds
{
    /// <summary>
    /// 兵種「電話簿」：用字串 key（如 "knight.hubao"）找到對應的 <see cref="AbstractTroopKind"/> 子類實例。
    /// <para>
    /// 為什麼要 Dictionary？遊戲裡部隊只存 kindKey，戰鬥時一行 <c>Get(key)</c> 拿到虎豹騎的 class，
    /// 不必寫一大串 if/switch，也不必到處 <c>new KnightHubaoTroopKind()</c>。
    /// </para>
    /// <para>
    /// <see cref="All"/> 只是給編輯器/工具「列出全部兵種」用，平常用 <see cref="Get"/> 即可。
    /// </para>
    /// </summary>
    public static class TroopKindRegistry
    {
        static Dictionary<string, AbstractTroopKind> byKey;

        /// <summary>在 <see cref="UnitConfigUtil.Load"/> 之後呼叫，確保六圍已從 properties 填入。</summary>
        public static void EnsureBuilt() => _ = All;

        /// <summary>所有已註冊兵種：key → 該兵種的單例物件。</summary>
        public static IReadOnlyDictionary<string, AbstractTroopKind> All =>
            byKey ??= BuildAllKindsOnce();

        /// <summary>依 properties 的 kind 字串取兵種定義（含技能/相性邏輯的那個 class）。</summary>
        public static AbstractTroopKind Get(string kindKey)
        {
            if (string.IsNullOrEmpty(kindKey))
                return null;
            var map = All;
            if (map.TryGetValue(kindKey, out AbstractTroopKind kind))
                return kind;
            if (TryResolveLegacyAlias(kindKey, out string canonical) && map.TryGetValue(canonical, out kind))
                return kind;
            return null;
        }

        public static bool TryGet(string kindKey, out AbstractTroopKind kind)
        {
            kind = Get(kindKey);
            return kind != null;
        }

        static bool TryResolveLegacyAlias(string key, out string canonical)
        {
            canonical = key switch
            {
                "spear.chinzhou" => TroopKindKeys.SpearQingzhou,
                "armor.baihau" => TroopKindKeys.ArmorBaimao,
                TroopKindKeys.Infantry => TroopKindKeys.Blade,
                _ => null
            };
            return canonical != null;
        }

        /// <summary>啟動時 new 出每一個兵種 class，放進字典。</summary>
        static Dictionary<string, AbstractTroopKind> BuildAllKindsOnce()
        {
            AbstractTroopKind[] instances =
            {
                new BladeTroopKind(),
                new SpearTroopKind(),
                new SpearAdvanceTroopKind(),
                new SpearQingzhouTroopKind(),
                new SpearDajiTroopKind(),
                new ArmorTroopKind(),
                new ArmorAdvanceTroopKind(),
                new ArmorXianzhenTroopKind(),
                new ArmorBaimaoTroopKind(),
                new CavalryTroopKind(),
                new KnightTroopKind(),
                new KnightAdvanceTroopKind(),
                new KnightHubaoTroopKind(),
                new KnightXilianTroopKind(),
                new HorsemanTroopKind(),
                new HorsemanAdvanceTroopKind(),
                new HorsemanBingzhouTroopKind(),
                new HorsemanBaimaTroopKind(),
                new ArcherTroopKind(),
                new BowTroopKind(),
                new BowAdvanceTroopKind(),
                new BowWudanTroopKind(),
                new BowDanyangTroopKind(),
                new CrossbowTroopKind(),
                new CrossbowAdvanceTroopKind(),
                new CrossbowXiandengTroopKind(),
                new CrossbowZhugeTroopKind(),
                new SiegeChargerTroopKind(),
                new SiegeMushouTroopKind(),
                new SiegeElephantTroopKind(),
                new SiegeShooterTroopKind(),
                new SiegeStoneTroopKind(),
                new SiegeTowerCrossbowTroopKind(),
                new NavySmallTroopKind(),
                new NavyMediumTroopKind(),
                new NavyLargeTroopKind(),
                new NavyFinalTroopKind()
            };

            var map = new Dictionary<string, AbstractTroopKind>(StringComparer.OrdinalIgnoreCase);
            foreach (AbstractTroopKind k in instances)
                map[k.KindKey] = k;
            return map;
        }
    }
}
