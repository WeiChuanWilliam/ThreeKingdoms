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

        public IReadOnlyDictionary<short, short> MobilityEffectToUnit => _mobilityToUnit;
        public IReadOnlyDictionary<short, short> AttackEffectToUnit => _attackToUnit;
        public IReadOnlyDictionary<short, short> DefenceEffectToUnit => _defenceToUnit;
        public IReadOnlyDictionary<short, bool> ReachableUnit => _reachableUnit;

        public void SetMobility(short unitId, short effect) => _mobilityToUnit[unitId] = effect;
        public void SetAttack(short unitId, short effect) => _attackToUnit[unitId] = effect;
        public void SetDefence(short unitId, short effect) => _defenceToUnit[unitId] = effect;
        public void SetReachable(short unitId, bool decision) => _reachableUnit[unitId] = decision;

        public bool IsReachableFor(short unitId) =>
            !_reachableUnit.TryGetValue(unitId, out bool allowed) || allowed;

        public void ClearAll()
        {
            _mobilityToUnit.Clear();
            _attackToUnit.Clear();
            _defenceToUnit.Clear();
            _reachableUnit.Clear();
        }
    }
}
