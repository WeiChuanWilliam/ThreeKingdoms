using System.Collections.Generic;

namespace ThreeKindoms.Core.Terrain
{
    /// <summary>
    /// 地形對單位的效果表（對應 C++ Terrain::AbstractTerrain 內四張 Map）。
    /// reachableUnit 為 unitId → bool（與 Unit 側 short 表不同）。
    /// </summary>
    public sealed class TerrainEffectMaps
    {
        readonly Dictionary<short, short> _mobilityToUnit = new();
        readonly Dictionary<short, short> _attackToUnit = new();
        readonly Dictionary<short, short> _defenceToUnit = new();
        readonly Dictionary<short, bool> _reachableUnit = new();

        /// <summary>兵種 id → 移動力修正。</summary>
        public IReadOnlyDictionary<short, short> MobilityEffectToUnit => _mobilityToUnit;

        /// <summary>兵種 id → 攻擊修正。</summary>
        public IReadOnlyDictionary<short, short> AttackEffectToUnit => _attackToUnit;

        /// <summary>兵種 id → 防禦修正。</summary>
        public IReadOnlyDictionary<short, short> DefenceEffectToUnit => _defenceToUnit;

        /// <summary>兵種 id → 是否可進入此地形的覆寫。</summary>
        public IReadOnlyDictionary<short, bool> ReachableUnit => _reachableUnit;

        /// <summary>登記某兵種在此地形的移動修正。</summary>
        public void SetMobility(short unitId, short effect) => _mobilityToUnit[unitId] = effect;

        /// <summary>登記某兵種在此地形的攻擊修正。</summary>
        public void SetAttack(short unitId, short effect) => _attackToUnit[unitId] = effect;

        /// <summary>登記某兵種在此地形的防禦修正。</summary>
        public void SetDefence(short unitId, short effect) => _defenceToUnit[unitId] = effect;

        /// <summary>登記某兵種是否可進入此地形的覆寫。</summary>
        public void SetReachable(short unitId, bool decision) => _reachableUnit[unitId] = decision;

        /// <summary>查詢兵種是否可進入；未登記則視為允許。</summary>
        public bool IsReachableFor(short unitId) =>
            !_reachableUnit.TryGetValue(unitId, out bool allowed) || allowed;

        /// <summary>清空所有兵種效果表。</summary>
        public void ClearAll()
        {
            _mobilityToUnit.Clear();
            _attackToUnit.Clear();
            _defenceToUnit.Clear();
            _reachableUnit.Clear();
        }
    }
}
