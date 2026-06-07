using System.Collections.Generic;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Skill;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Core.Units
{
    public sealed class Combat : Unit, ICombatTroopStatsSource
    {
        public override float FoodConsumptionFactor => UnitConfigUtil.GetFoodConsumptionFactor(UnitKind.Combat);
        public override float FireDamageFactor => UnitConfigUtil.GetFireDamageFactor(UnitKind.Combat);

        /// <summary>行军倍率（仅战斗部队；× <see cref="TroopMobility"/> 见 <see cref="UnitMarchSpeed"/>）。</summary>
        public float MarchSpeedFactor => UnitConfigUtil.GetCombatMarchSpeedFactor();

        /// <summary>行军用：参数倍率 × 兵种表 mobility。</summary>
        public float MarchMobilityRating => MarchSpeedFactor * TroopMobility;

        readonly HashSet<Skill> battleSkills = new(SkillByIdComparer.Instance);
        readonly HashSet<Skill> strategySkills = new(SkillByIdComparer.Instance);
        readonly HashSet<Skill> mobilitySkills = new(SkillByIdComparer.Instance);
        readonly HashSet<Skill> defenceSkills = new(SkillByIdComparer.Instance);
        readonly HashSet<int> officerSkillIds = new();

        public TroopType TroopType { get; private set; }
        public string TroopKindKey { get; private set; }
        public short TroopAttack { get; private set; }
        public short TroopDefense { get; private set; }
        public short TroopMobility { get; private set; }
        public short TroopJipo { get; private set; }
        public short TroopGongcheng { get; private set; }
        public short TroopStamina { get; private set; }
        public short TroopAttackRange { get; private set; }
        public int TroopStage { get; private set; }

        /// <summary>副將（戰鬥部隊至多一位）。</summary>
        public Officer ViceOfficer => ViceOfficers.Count > 0 ? ViceOfficers[0] : null;

        /// <summary>主將+副將合併後的技能 id（HashSet 去重）。</summary>
        public IReadOnlyCollection<int> OfficerSkillIds => officerSkillIds;

        internal HashSet<int> OfficerSkillIdSet => officerSkillIds;

        /// <summary>兵種表六圍（初始值）。</summary>
        public CombatTroopStatBlock BaseTroopStats => CombatStatMath.GetBaseTroopStats(this);

        /// <summary>武將／科技／戰法後、尚未乘地勢。</summary>
        public CombatTroopStatBlock TroopStatsAfterOfficerAndResearch =>
            CombatStatMath.GetStatsAfterOfficerAndResearch(this);

        /// <summary>最終六圍（含地勢）。</summary>
        public CombatTroopStatBlock EffectiveTroopStats => CombatStatMath.GetEffectiveTroopStats(this);

        public short EffectiveAttack => CombatStatMath.GetEffectiveAttack(this);
        public short EffectiveDefense => CombatStatMath.GetEffectiveDefense(this);
        public short EffectiveMobility => CombatStatMath.GetEffectiveMobility(this);
        public short EffectiveJipo => CombatStatMath.GetEffectiveJipo(this);
        public short EffectiveGongcheng => CombatStatMath.GetEffectiveGongcheng(this);
        public short EffectiveTroopStamina => CombatStatMath.GetEffectiveTroopStamina(this);
        public short EffectiveAttackRange => CombatStatMath.GetEffectiveAttackRange(this);

        /// <summary>部隊智力（主將／副將相合，公式可調）。</summary>
        public short UnitIntelligence => CombatStatMath.GetUnitIntelligence(this);

        /// <summary>戰鬥力評分（武將能力、技能組、士氣、體力、六圍、有效兵力）。</summary>
        public int CombatPower => CombatPowerRules.GetCombatPower(this);

        public override UnitKind Kind => UnitKind.Combat;

        /// <summary>從表資料建立（對應 Java：{@code new Combat(combatDef)}）。</summary>
        public Combat(CombatUnitDef def)
            : base(UnitNameBuilder.Resolve(def, UnitKind.Combat), def.Belonged)
        {
            def.ApplyTo(this);
        }

        public new void SetCommander(Officer unitCopy)
        {
            base.SetCommander(unitCopy);
            RefreshSkillsFromOfficers();
        }

        public new void SetCommanderFromPool(int officerDefId)
        {
            base.SetCommanderFromPool(officerDefId);
            RefreshSkillsFromOfficers();
        }

        /// <summary>設定唯一副將（取代既有副將）。</summary>
        public bool SetViceOfficer(Officer unitCopy)
        {
            ClearViceOfficers();
            if (unitCopy == null)
            {
                RefreshSkillsFromOfficers();
                return true;
            }

            bool ok = base.AddViceOfficer(unitCopy);
            RefreshSkillsFromOfficers();
            return ok;
        }

        public bool SetViceOfficerFromPool(int officerDefId) =>
            SetViceOfficer(officerDefId > 0 ? OfficerPool.CloneForUnit(officerDefId) : null);

        public new bool AddViceOfficer(Officer unitCopy)
        {
            if (ViceOfficers.Count >= 1)
                return false;
            bool ok = base.AddViceOfficer(unitCopy);
            if (ok)
                RefreshSkillsFromOfficers();
            return ok;
        }

        /// <summary>從主將／副將重新彙整技能 id 至 <see cref="OfficerSkillIds"/>。</summary>
        public void RefreshSkillsFromOfficers() => CombatSkillAssembler.RefreshEquippedSkills(this);

        /// <summary>建立戰鬥力計算用上下文（含武將能力、技能組、士氣、體力）。</summary>
        public bool TryGetCombatPowerContext(out CombatPowerContext context) =>
            CombatPowerRules.TryCreateContext(this, out context);

        internal void ClearOfficerSkillIds() => officerSkillIds.Clear();

        internal void ClearEquippedSkillSets()
        {
            battleSkills.Clear();
            strategySkills.Clear();
            mobilitySkills.Clear();
            defenceSkills.Clear();
        }

        public void SetTroopType(TroopType type) => TroopType = type;

        /// <summary>六圍與攻擊距離取自兵種表（properties → <see cref="AbstractTroopKind"/>）。</summary>
        public void BindTroopKind(AbstractTroopKind kind)
        {
            if (kind == null) return;
            TroopKindKey = kind.KindKey;
            TroopType = kind.Category;
            TroopAttack = kind.Attack;
            TroopDefense = kind.Defense;
            TroopMobility = kind.Mobility;
            TroopJipo = kind.Jipo;
            TroopGongcheng = kind.Gongcheng;
            TroopStamina = kind.Stamina;
            TroopAttackRange = kind.AttackRange;
            TroopStage = kind.Stage;
        }

        public int CountEquippedSkills() =>
            battleSkills.Count + strategySkills.Count + mobilitySkills.Count + defenceSkills.Count;

        public bool ContainsBattleSkill(int skillId) => FindSkill(battleSkills, skillId) != null;
        public bool ContainsStrategySkill(int skillId) => FindSkill(strategySkills, skillId) != null;
        public bool ContainsMobilitySkill(int skillId) => FindSkill(mobilitySkills, skillId) != null;
        public bool ContainsDefenceSkill(int skillId) => FindSkill(defenceSkills, skillId) != null;
        public bool ContainsOfficerSkill(int skillId) => skillId > 0 && officerSkillIds.Contains(skillId);

        public Skill? GetBattleSkill(int skillId) => FindSkill(battleSkills, skillId);
        public Skill? GetStrategySkill(int skillId) => FindSkill(strategySkills, skillId);

        public bool AddBattleSkill(int skillId) => AddSkillCopy(battleSkills, skillId);
        public bool RemoveBattleSkill(int skillId) => RemoveSkillById(battleSkills, skillId);
        public bool AddStrategySkill(int skillId) => AddSkillCopy(strategySkills, skillId);
        public bool RemoveStrategySkill(int skillId) => RemoveSkillById(strategySkills, skillId);
        public bool AddMobilitySkill(int skillId) => AddSkillCopy(mobilitySkills, skillId);
        public bool RemoveMobilitySkill(int skillId) => RemoveSkillById(mobilitySkills, skillId);
        public bool AddDefenceSkill(int skillId) => AddSkillCopy(defenceSkills, skillId);
        public bool RemoveDefenceSkill(int skillId) => RemoveSkillById(defenceSkills, skillId);

        public override int CalculateFoodConsumption() =>
            System.Math.Max(1, (int)(BaseFoodByHeadCount() * FoodConsumptionFactor));

        internal void CollectEquippedSkills(
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> battle,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> strategy,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> mobility,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> defence)
        {
            CollectSkillSet(battleSkills, battle);
            CollectSkillSet(strategySkills, strategy);
            CollectSkillSet(mobilitySkills, mobility);
            CollectSkillSet(defenceSkills, defence);
        }

        static void CollectSkillSet(
            HashSet<Skill> set,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> target)
        {
            foreach (Skill s in set)
            {
                target.Add(new Data.Persistence.SkillSaveEntry
                {
                    skillId = s.SkillId,
                    level = s.Level,
                    enabled = s.Enabled
                });
            }
        }

        static bool AddSkillCopy(HashSet<Skill> set, int skillId)
        {
            if (skillId <= 0) return false;
            return set.Add(SkillPool.CopyForUnit(skillId));
        }

        static bool RemoveSkillById(HashSet<Skill> set, int skillId)
        {
            Skill? found = FindSkill(set, skillId);
            return found.HasValue && set.Remove(found.Value);
        }

        static Skill? FindSkill(HashSet<Skill> set, int skillId)
        {
            if (skillId <= 0) return null;
            foreach (Skill s in set)
            {
                if (s.SkillId == skillId) return s;
            }
            return null;
        }
    }
}
