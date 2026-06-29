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

        /// <summary>對目標部隊的機動修正表（唯讀）。</summary>
        public IReadOnlyDictionary<short, short> MobilityEffectToUnit => _mobilityToUnit;

        /// <summary>對目標部隊的攻擊修正表（唯讀）。</summary>
        public IReadOnlyDictionary<short, short> AttackEffectToUnit => _attackToUnit;

        /// <summary>對目標部隊的防禦修正表（唯讀）。</summary>
        public IReadOnlyDictionary<short, short> DefenceEffectToUnit => _defenceToUnit;

        /// <summary>可達／交戰範圍相關的部隊 id 表（唯讀）。</summary>
        public IReadOnlyDictionary<short, short> ReachableUnit => _reachableUnit;

        /// <summary>設定對指定部隊的機動修正值。</summary>
        public void SetMobility(short targetUnitId, short value) => _mobilityToUnit[targetUnitId] = value;

        /// <summary>設定對指定部隊的攻擊修正值。</summary>
        public void SetAttack(short targetUnitId, short value) => _attackToUnit[targetUnitId] = value;

        /// <summary>設定對指定部隊的防禦修正值。</summary>
        public void SetDefence(short targetUnitId, short value) => _defenceToUnit[targetUnitId] = value;

        /// <summary>設定可達／交戰範圍內的部隊條目。</summary>
        public void SetReachable(short targetUnitId, short value) => _reachableUnit[targetUnitId] = value;

        /// <summary>查詢對指定部隊的機動修正值。</summary>
        public bool TryGetMobility(short targetUnitId, out short value) =>
            _mobilityToUnit.TryGetValue(targetUnitId, out value);

        /// <summary>清空全部效果表。</summary>
        public void ClearAll()
        {
            _mobilityToUnit.Clear();
            _attackToUnit.Clear();
            _defenceToUnit.Clear();
            _reachableUnit.Clear();
        }
    }
}
