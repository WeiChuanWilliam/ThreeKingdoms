using ThreeKindoms.Core.Officers;
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

        // ----- 戰法四槽：暫不使用（之後接 skill.properties／戰法 class 再打開）-----
        // readonly HashSet<Skill> battleSkills = new(SkillByIdComparer.Instance);
        // readonly HashSet<Skill> strategySkills = new(SkillByIdComparer.Instance);
        // readonly HashSet<Skill> mobilitySkills = new(SkillByIdComparer.Instance);
        // readonly HashSet<Skill> defenceSkills = new(SkillByIdComparer.Instance);

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

        /// <summary>兵種表六圍（初始值）。</summary>
        public CombatTroopStatBlock BaseTroopStats => CombatStatMath.GetBaseTroopStats(this);

        /// <summary>武將／科技後、尚未乘地勢（戰法暫不納入）。</summary>
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

        /// <summary>攻擊力（結算用；見 <see cref="CombatBattleFormulas"/>）。</summary>
        public int CalculateAttack() => CombatBattleFormulas.CalculateAttack(this);

        /// <summary>防禦力（結算用；見 <see cref="CombatBattleFormulas"/>）。</summary>
        public int CalculateDefense() => CombatBattleFormulas.CalculateDefense(this);

        /// <summary>機動力（結算用）。</summary>
        public int CalculateMobility() => CombatBattleFormulas.CalculateMobility(this);

        /// <summary>破甲（結算用）。</summary>
        public int CalculateJipo() => CombatBattleFormulas.CalculateJipo(this);

        /// <summary>攻城（結算用）。</summary>
        public int CalculateGongcheng() => CombatBattleFormulas.CalculateGongcheng(this);

        /// <summary>部隊耐力（結算用）。</summary>
        public int CalculateTroopStaminaStat() => CombatBattleFormulas.CalculateTroopStamina(this);

        /// <summary>普攻攻擊距離（結算用）。</summary>
        public int CalculateAttackRange() => CombatBattleFormulas.CalculateAttackRange(this);

        /// <summary>部隊類型：戰鬥部隊。</summary>
        public override UnitKind Kind => UnitKind.Combat;

        /// <summary>所屬兵團（暫保留掛載；兵糧目前視為無限，不由此扣糧）。</summary>
        public Legion ParentLegion { get; private set; }

        /// <summary>建立空戰鬥部隊；組隊請用 <see cref="Data.Units.UnitUtil.Create"/>。</summary>
        public Combat(string unitName, int factionBelonged)
            : base(unitName ?? "", factionBelonged)
        {
            SetGarrison(false);
        }

        /// <summary>設定唯一副將（取代既有副將）。</summary>
        public bool SetViceOfficer(Officer unitCopy)
        {
            ClearViceOfficers();
            if (unitCopy == null)
                return true;

            return base.AddViceOfficer(unitCopy);
        }

        /// <summary>從武將池設定唯一副將（id≤0 則清空）。</summary>
        public bool SetViceOfficerFromPool(int officerDefId) =>
            SetViceOfficer(officerDefId > 0 ? OfficerPool.Get(officerDefId) : null);

        /// <summary>新增副將（至多一位）。</summary>
        public override bool AddViceOfficer(Officer unitCopy)
        {
            if (ViceOfficers.Count >= 1)
                return false;
            return base.AddViceOfficer(unitCopy);
        }

        /// <summary>建立戰鬥力計算用上下文（士氣、體力、六圍等；戰法暫不納入）。</summary>
        public bool TryGetCombatPowerContext(out CombatPowerContext context) =>
            CombatPowerRules.TryCreateContext(this, out context);

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

        /// <summary>戰法暫停：恒為 0。</summary>
        public int CountEquippedSkills() => 0;

        // ----- 戰法 API 暫註解 -----
        // public bool ContainsBattleSkill(int skillId) => ...
        // public bool AddBattleSkill(int skillId) => ...
        // public bool RemoveBattleSkill(int skillId) => ...
        // （strategy／mobility／defence 同）

        /// <summary>掛載至所屬兵團（耗糧由兵團糧草扣除）。</summary>
        public void AttachToLegion(Legion legion) => ParentLegion = legion;

        /// <summary>脫離兵團歸屬。</summary>
        public void DetachFromLegion() => ParentLegion = null;

        /// <summary>暫定兵糧無限：每日應耗糧恒為 0。</summary>
        public override int CalculateFoodConsumption() => 0;

        /// <summary>暫定兵糧無限：不扣糧，恒成功。</summary>
        public override bool TryConsumeDailyFood() => true;

        /// <summary>野戰戰鬥力（武將、六圍、士氣、體力與有效兵力；戰法暫不納入）。</summary>
        protected override int CalculateNonGarrisonCombatPower() =>
            CombatPowerRules.GetCombatPower(this);

        /// <summary>駐紮戰鬥力（目前同野戰公式；之後可疊據點攻防加成）。</summary>
        protected override int CalculateGarrisonCombatPower() =>
            CombatPowerRules.GetCombatPower(this);

        /// <summary>戰法暫停：不匯出裝備戰法。</summary>
        internal void CollectEquippedSkills(
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> battle,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> strategy,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> mobility,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> defence)
        {
            // 戰法暫不存檔
        }
        
        
        /// <summary>攻擊力。待填兵力／士氣等；目前 Combat 暫回傳六圍最終攻擊。</summary>
        public static int CalculateAttack(Unit unit)
        {
            if (unit is Combat combat)
                return CalculateAttack(combat);
            return 0;
        }
    }
}
