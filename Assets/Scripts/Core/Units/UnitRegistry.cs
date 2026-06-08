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

        public static IReadOnlyCollection<Unit> All => registered;

        public static int Count => registered.Count;

        public static bool Contains(Unit unit) => unit != null && registered.Contains(unit);

        public static void Register(Unit unit)
        {
            if (unit == null || unit.IsAnnihilated)
                return;
            registered.Add(unit);
        }

        public static void Unregister(Unit unit)
        {
            if (unit == null)
                return;
            registered.Remove(unit);
        }

        public static void Replace(Unit oldUnit, Unit newUnit)
        {
            if (oldUnit != null)
                registered.Remove(oldUnit);
            Register(newUnit);
        }

        public static void Clear() => registered.Clear();
    }
}
