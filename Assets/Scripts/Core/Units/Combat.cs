using System.Collections.Generic;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Skill;
using ThreeKindoms.Data.Units;
using ThreeKindoms.Data.Units.TroopKinds;

namespace ThreeKindoms.Core.Units
{
    public sealed class Combat : Unit, ICombatTroopStatsSource
    {
        /// <summary>戰鬥部隊每日耗糧倍率（相對於人頭基數）。</summary>
        public override float FoodConsumptionFactor => UnitConfigUtil.GetFoodConsumptionFactor(UnitKind.Combat);

        /// <summary>戰鬥部隊受火焰傷害時的部隊類型係數。</summary>
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

        /// <summary>兵科大類（步／騎／弓等）。</summary>
        public TroopType TroopType { get; private set; }

        /// <summary>兵種表鍵值（對應 properties 兵種定義）。</summary>
        public string TroopKindKey { get; private set; }

        /// <summary>兵種表攻擊（初始六圍）。</summary>
        public short TroopAttack { get; private set; }

        /// <summary>兵種表防禦（初始六圍）。</summary>
        public short TroopDefense { get; private set; }

        /// <summary>兵種表機動（初始六圍）。</summary>
        public short TroopMobility { get; private set; }

        /// <summary>兵種表破甲（初始六圍）。</summary>
        public short TroopJipo { get; private set; }

        /// <summary>兵種表攻城（初始六圍）。</summary>
        public short TroopGongcheng { get; private set; }

        /// <summary>兵種表耐力（初始六圍）。</summary>
        public short TroopStamina { get; private set; }

        /// <summary>兵種表攻擊距離（初始值）。</summary>
        public short TroopAttackRange { get; private set; }

        /// <summary>兵種階段／世代（兵種表 stage）。</summary>
        public int TroopStage { get; private set; }

        /// <summary>副將（戰鬥部隊至多一位）。</summary>
        public Officer ViceOfficer => ViceOfficers.Count > 0 ? ViceOfficers[0] : null;

        /// <summary>主將+副將合併後的技能 id（HashSet 去重）。</summary>
        public IReadOnlyCollection<int> OfficerSkillIds => officerSkillIds;

        /// <summary>主將／副將合併技能 id 的可變集合（組裝與存檔用）。</summary>
        internal HashSet<int> OfficerSkillIdSet => officerSkillIds;

        /// <summary>兵種表六圍（初始值）。</summary>
        public CombatTroopStatBlock BaseTroopStats => CombatStatMath.GetBaseTroopStats(this);

        /// <summary>武將／科技／戰法後、尚未乘地勢。</summary>
        public CombatTroopStatBlock TroopStatsAfterOfficerAndResearch =>
            CombatStatMath.GetStatsAfterOfficerAndResearch(this);

        /// <summary>最終六圍（含地勢）。</summary>
        public CombatTroopStatBlock EffectiveTroopStats => CombatStatMath.GetEffectiveTroopStats(this);

        /// <summary>最終攻擊（含武將、科技、地勢）。</summary>
        public short EffectiveAttack => CombatStatMath.GetEffectiveAttack(this);

        /// <summary>最終防禦（含武將、科技、地勢）。</summary>
        public short EffectiveDefense => CombatStatMath.GetEffectiveDefense(this);

        /// <summary>最終機動（含武將、科技、地勢）。</summary>
        public short EffectiveMobility => CombatStatMath.GetEffectiveMobility(this);

        /// <summary>最終破甲（含武將、科技、地勢）。</summary>
        public short EffectiveJipo => CombatStatMath.GetEffectiveJipo(this);

        /// <summary>最終攻城（含武將、科技、地勢）。</summary>
        public short EffectiveGongcheng => CombatStatMath.GetEffectiveGongcheng(this);

        /// <summary>最終耐力（含武將、科技、地勢）。</summary>
        public short EffectiveTroopStamina => CombatStatMath.GetEffectiveTroopStamina(this);

        /// <summary>最終攻擊距離（含武將、科技、地勢）。</summary>
        public short EffectiveAttackRange => CombatStatMath.GetEffectiveAttackRange(this);

        /// <summary>部隊智力（主將／副將相合，公式可調）。</summary>
        public short UnitIntelligence => CombatStatMath.GetUnitIntelligence(this);

        /// <summary>戰鬥力評分；等同 <see cref="CalculateCombatPower"/>。</summary>
        public int CombatPower => CalculateCombatPower();

        /// <summary>部隊類型：戰鬥部隊。</summary>
        public override UnitKind Kind => UnitKind.Combat;

        /// <summary>所屬兵團；戰鬥部隊耗糧由此兵團的 <see cref="Legion.CarriedFood"/> 扣除。</summary>
        public Legion ParentLegion { get; private set; }

        /// <summary>從表資料建立（對應 Java：{@code new Combat(combatDef)}）。</summary>
        public Combat(CombatUnitDef def)
            : base(UnitNameBuilder.Resolve(def, UnitKind.Combat), def.Belonged)
        {
            def.ApplyTo(this);
        }

        /// <summary>指派主將並重新彙整武將技能。</summary>
        public new void SetCommander(Officer unitCopy)
        {
            base.SetCommander(unitCopy);
            RefreshSkillsFromOfficers();
        }

        /// <summary>從武將池指派主將並重新彙整武將技能。</summary>
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

        /// <summary>從武將池設定唯一副將（id≤0 則清空）。</summary>
        public bool SetViceOfficerFromPool(int officerDefId) =>
            SetViceOfficer(officerDefId > 0 ? OfficerPool.Get(officerDefId) : null);

        /// <summary>新增副將（至多一位）；成功後重新彙整武將技能。</summary>
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

        /// <summary>清空主將／副將合併後的技能 id 集合。</summary>
        internal void ClearOfficerSkillIds() => officerSkillIds.Clear();

        /// <summary>清空四類裝備戰法技能集合（重新組裝前）。</summary>
        internal void ClearEquippedSkillSets()
        {
            battleSkills.Clear();
            strategySkills.Clear();
            mobilitySkills.Clear();
            defenceSkills.Clear();
        }

        /// <summary>設定兵科大類。</summary>
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

        /// <summary>已裝備戰法技能總數（四槽合計）。</summary>
        public int CountEquippedSkills() =>
            battleSkills.Count + strategySkills.Count + mobilitySkills.Count + defenceSkills.Count;

        /// <summary>是否已裝備指定 id 的戰鬥戰法。</summary>
        public bool ContainsBattleSkill(int skillId) => FindSkill(battleSkills, skillId) != null;

        /// <summary>是否已裝備指定 id 的計略戰法。</summary>
        public bool ContainsStrategySkill(int skillId) => FindSkill(strategySkills, skillId) != null;

        /// <summary>是否已裝備指定 id 的機動戰法。</summary>
        public bool ContainsMobilitySkill(int skillId) => FindSkill(mobilitySkills, skillId) != null;

        /// <summary>是否已裝備指定 id 的防禦戰法。</summary>
        public bool ContainsDefenceSkill(int skillId) => FindSkill(defenceSkills, skillId) != null;

        /// <summary>主將或副將是否擁有指定技能 id。</summary>
        public bool ContainsOfficerSkill(int skillId) => skillId > 0 && officerSkillIds.Contains(skillId);

        /// <summary>取得已裝備的戰鬥戰法實例。</summary>
        public Skill? GetBattleSkill(int skillId) => FindSkill(battleSkills, skillId);

        /// <summary>取得已裝備的計略戰法實例。</summary>
        public Skill? GetStrategySkill(int skillId) => FindSkill(strategySkills, skillId);

        /// <summary>裝備戰鬥戰法（複製技能池副本至部隊）。</summary>
        public bool AddBattleSkill(int skillId) => AddSkillCopy(battleSkills, skillId);

        /// <summary>卸下戰鬥戰法。</summary>
        public bool RemoveBattleSkill(int skillId) => RemoveSkillById(battleSkills, skillId);

        /// <summary>裝備計略戰法。</summary>
        public bool AddStrategySkill(int skillId) => AddSkillCopy(strategySkills, skillId);

        /// <summary>卸下計略戰法。</summary>
        public bool RemoveStrategySkill(int skillId) => RemoveSkillById(strategySkills, skillId);

        /// <summary>裝備機動戰法。</summary>
        public bool AddMobilitySkill(int skillId) => AddSkillCopy(mobilitySkills, skillId);

        /// <summary>卸下機動戰法。</summary>
        public bool RemoveMobilitySkill(int skillId) => RemoveSkillById(mobilitySkills, skillId);

        /// <summary>裝備防禦戰法。</summary>
        public bool AddDefenceSkill(int skillId) => AddSkillCopy(defenceSkills, skillId);

        /// <summary>卸下防禦戰法。</summary>
        public bool RemoveDefenceSkill(int skillId) => RemoveSkillById(defenceSkills, skillId);

        /// <summary>掛載至所屬兵團（耗糧由兵團糧草扣除）。</summary>
        public void AttachToLegion(Legion legion) => ParentLegion = legion;

        /// <summary>脫離兵團歸屬。</summary>
        public void DetachFromLegion() => ParentLegion = null;

        /// <summary>計算本日應耗糧數（殲滅或駐紮時為 0）。</summary>
        public override int CalculateFoodConsumption()
        {
            if (IsAnnihilated || IsStationed) return 0;
            return System.Math.Max(1, (int)(BaseFoodByHeadCount() * FoodConsumptionFactor));
        }

        /// <summary>計算本日耗糧並向 <see cref="ParentLegion"/> 申請扣除；無兵團或糧盡回傳 false。</summary>
        public override bool TryConsumeDailyFood()
        {
            int cost = CalculateFoodConsumption();
            if (cost <= 0) return true;
            if (ParentLegion == null) return false;
            return ParentLegion.TryConsumeFood(cost);
        }

        /// <summary>依武將、技能、六圍、士氣、體力與有效兵力計算戰鬥力。</summary>
        public override int CalculateCombatPower() => CombatPowerRules.GetCombatPower(this);

        /// <summary>匯出四槽裝備戰法至存檔條目清單。</summary>
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
