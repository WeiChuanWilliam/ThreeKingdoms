using System.Collections.Generic;

namespace ThreeKindoms.Core.Units
{
    /// <summary>對應 C++ Map&lt;short, short&gt; 效果表（目標 id → 修正值）。</summary>
    public sealed class UnitEffectMaps
    {
        readonly Dictionary<short, short> _mobilityToUnit = new();
        readonly Dictionary<short, short> _attackToUnit = new();
        readonly Dictionary<short, short> _defenceToUnit = new();
        readonly Dictionary<short, short> _reachableUnit = new();

        public IReadOnlyDictionary<short, short> MobilityEffectToUnit => _mobilityToUnit;
        public IReadOnlyDictionary<short, short> AttackEffectToUnit => _attackToUnit;
        public IReadOnlyDictionary<short, short> DefenceEffectToUnit => _defenceToUnit;
        public IReadOnlyDictionary<short, short> ReachableUnit => _reachableUnit;

        public void SetMobility(short targetUnitId, short value) => _mobilityToUnit[targetUnitId] = value;
        public void SetAttack(short targetUnitId, short value) => _attackToUnit[targetUnitId] = value;
        public void SetDefence(short targetUnitId, short value) => _defenceToUnit[targetUnitId] = value;
        public void SetReachable(short targetUnitId, short value) => _reachableUnit[targetUnitId] = value;

        public bool TryGetMobility(short targetUnitId, out short value) =>
            _mobilityToUnit.TryGetValue(targetUnitId, out value);

        public void ClearAll()
        {
            _mobilityToUnit.Clear();
            _attackToUnit.Clear();
            _defenceToUnit.Clear();
            _reachableUnit.Clear();
        }
    }
}
