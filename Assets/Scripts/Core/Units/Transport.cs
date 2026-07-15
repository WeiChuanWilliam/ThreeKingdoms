using System.Collections.Generic;
using ThreeKindoms.Data.Skill;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    public sealed class Transport : Unit
    {
        /// <summary>運輸隊每日耗糧倍率（相對於人頭基數）。</summary>
        public override float FoodConsumptionFactor => UnitConfigUtil.GetFoodConsumptionFactor(UnitKind.Transport);

        /// <summary>運輸隊受火焰傷害時的部隊類型係數。</summary>
        public override float FireDamageFactor => UnitConfigUtil.GetFireDamageFactor(UnitKind.Transport);

        /// <summary>行军 mobility（参数档绝对值，非倍率）。</summary>
        public float MarchMobility => UnitConfigUtil.GetUnitMarchMobility(UnitKind.Transport);

        readonly HashSet<Skill> strategySkills = new(SkillByIdComparer.Instance);

        /// <summary>部隊類型：運輸隊。</summary>
        public override UnitKind Kind => UnitKind.Transport;

        /// <summary>建立空運輸隊（組隊欄位請用 Set*）。</summary>
        public Transport(string unitName, int factionBelonged)
            : base(unitName ?? "", factionBelonged)
        {
        }

        /// <summary>是否已裝備指定 id 的計略戰法。</summary>
        public bool ContainsStrategySkill(int skillId)
        {
            if (skillId <= 0) return false;
            foreach (Skill s in strategySkills)
            {
                if (s.SkillId == skillId) return true;
            }
            return false;
        }

        /// <summary>裝備計略戰法（複製技能池副本至部隊）。</summary>
        public bool AddStrategySkill(int skillId)
        {
            if (skillId <= 0) return false;
            return strategySkills.Add(SkillPool.CopyForUnit(skillId));
        }

        /// <summary>卸下計略戰法。</summary>
        public bool RemoveStrategySkill(int skillId)
        {
            foreach (Skill s in strategySkills)
            {
                if (s.SkillId != skillId) continue;
                return strategySkills.Remove(s);
            }
            return false;
        }

        /// <summary>暫定兵糧無限：每日應耗糧恒為 0。</summary>
        public override int CalculateFoodConsumption() => 0;

        /// <summary>匯出裝備計略戰法至存檔條目清單。</summary>
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

        /// <summary>野戰：幾乎無作戰能力。</summary>
        protected override int CalculateNonGarrisonCombatPower() =>
            UnitCombatPowerRules.CalculateTransportPower(this);

        /// <summary>駐紮：偏提高防禦（守輜重）。</summary>
        protected override int CalculateGarrisonCombatPower() =>
            UnitCombatPowerRules.CalculateGarrisonTransportPower(this);
    }
}
