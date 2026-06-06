using System.Collections.Generic;

namespace ThreeKindoms.Data.Units.TroopKinds
{
    /// <summary>所有兵種最上層。數值自 unit.properties（troop.kind.*）載入。</summary>
    public abstract class AbstractTroopKind
    {
        public abstract string KindKey { get; }

        public string DisplayName => UnitConfigUtil.GetKindDisplayName(KindKey);

        public TroopType Category { get; private set; }
        public int Stage { get; private set; }
        public string ParentKindKey { get; private set; }

        public short Attack { get; private set; }
        public short Defense { get; private set; }
        public short Mobility { get; private set; }
        public short Jipo { get; private set; }
        public short Gongcheng { get; private set; }
        public short Stamina { get; private set; }

        /// <summary>攻擊距離（格）；troop.kind.*.attack_range，未設則近戰 1。</summary>
        public short AttackRange { get; private set; }

        protected AbstractTroopKind()
        {
            if (TroopKindTree.TryGetNode(KindKey, out TroopKindNode node))
            {
                Category = node.TroopType;
                ParentKindKey = node.ParentKey;
            }
            Stage = TroopKindTree.GetStage(KindKey);
            ApplyBaseStatsFromProperties();
        }

        public virtual IReadOnlyList<int> GetExclusiveSkillIds() =>
            System.Array.Empty<int>();

        public virtual float GetDamageMultiplierAgainst(AbstractTroopKind defender)
        {
            if (defender == null) return 1f;
            return 1f;
        }

        void ApplyBaseStatsFromProperties()
        {
            if (!UnitConfigUtil.TryGetKindBaseStats(KindKey, out TroopKindBaseStats stats))
                return;

            Attack = stats.Attack;
            Defense = stats.Defense;
            Mobility = stats.Mobility;
            Jipo = stats.Jipo;
            Gongcheng = stats.Gongcheng;
            Stamina = stats.Stamina;

            AttackRange = UnitConfigUtil.TryGetKindAttackRange(KindKey, out short range)
                ? range
                : (short)1;
        }
    }
}
