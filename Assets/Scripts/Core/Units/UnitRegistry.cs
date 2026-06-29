using System.Collections.Generic;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 全場部隊索引：日出與每日結算只遍歷此集合，不掃描整張地圖。
    /// 進入地圖（<see cref="UnitLocationBinding.BindToWorld"/>）時登記；殲滅或離開世界時移除。
    /// </summary>
    public static class UnitRegistry
    {
        static readonly HashSet<Unit> registered = new();

        /// <summary>目前已登記在場的全部部隊。</summary>
        public static IReadOnlyCollection<Unit> All => registered;

        /// <summary>已登記部隊數量。</summary>
        public static int Count => registered.Count;

        /// <summary>部隊是否已登記在場。</summary>
        public static bool Contains(Unit unit) => unit != null && registered.Contains(unit);

        /// <summary>部隊進入地圖時登記（殲滅或 null 則略過）。</summary>
        public static void Register(Unit unit)
        {
            if (unit == null || unit.IsAnnihilated)
                return;
            registered.Add(unit);
        }

        /// <summary>部隊離開世界或殲滅時移除登記。</summary>
        public static void Unregister(Unit unit)
        {
            if (unit == null)
                return;
            registered.Remove(unit);
        }

        /// <summary>駐軍轉換等：以新部隊取代舊部隊的登記。</summary>
        public static void Replace(Unit oldUnit, Unit newUnit)
        {
            if (oldUnit != null)
                registered.Remove(oldUnit);
            Register(newUnit);
        }

        /// <summary>清空全部登記（新局或重載）。</summary>
        public static void Clear() => registered.Clear();
    }
}
