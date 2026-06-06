using System.Collections.Generic;
using ThreeKindoms.Data.Skill;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    public sealed class Transport : Unit
    {
        public override float FoodConsumptionFactor => UnitConfigUtil.GetFoodConsumptionFactor(UnitKind.Transport);

        /// <summary>行军 mobility（参数档绝对值，非倍率）。</summary>
        public float MarchMobility => UnitConfigUtil.GetUnitMarchMobility(UnitKind.Transport);

        readonly HashSet<Skill> strategySkills = new(SkillByIdComparer.Instance);

        public override UnitKind Kind => UnitKind.Transport;

        public Transport(TransportUnitDef def)
            : base(UnitNameBuilder.Resolve(def, UnitKind.Transport), def.Belonged)
        {
            def.ApplyTo(this);
        }

        public bool ContainsStrategySkill(int skillId)
        {
            if (skillId <= 0) return false;
            foreach (Skill s in strategySkills)
            {
                if (s.SkillId == skillId) return true;
            }
            return false;
        }

        public bool AddStrategySkill(int skillId)
        {
            if (skillId <= 0) return false;
            return strategySkills.Add(SkillPool.CopyForUnit(skillId));
        }

        public bool RemoveStrategySkill(int skillId)
        {
            foreach (Skill s in strategySkills)
            {
                if (s.SkillId != skillId) continue;
                return strategySkills.Remove(s);
            }
            return false;
        }

        public override int CalculateFoodConsumption() =>
            IsAnnihilated ? 0 : System.Math.Max(1, (int)(BaseFoodByHeadCount() * FoodConsumptionFactor));

        internal void CollectEquippedSkills(System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> strategy)
        {
            foreach (Skill s in strategySkills)
            {
                strategy.Add(new Data.Persistence.SkillSaveEntry
                {
                    skillId = s.SkillId,
                    level = s.Level,
                    enabled = s.Enabled
                });
            }
        }
    }
}
