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

        /// <summary>兵科大類（步／騎／弓等）。</summary>
        public TroopType TroopType { get; private set; }

        /// <summary>兵種表鍵值（對應 properties 兵種定義）。</summary>
        public string TroopKindKey { get; private set; }

        /// <summary>兵種表攻擊。</summary>
        public short TroopAttack { get; private set; }

        /// <summary>兵種表防禦。</summary>
        public short TroopDefense { get; private set; }

        /// <summary>兵種表機動。</summary>
        public short TroopMobility { get; private set; }

        /// <summary>兵種表擊破。</summary>
        public short TroopJipo { get; private set; }

        /// <summary>兵種表破城。</summary>
        public short TroopGongcheng { get; private set; }

        /// <summary>兵種表耐力。</summary>
        public short TroopStamina { get; private set; }

        /// <summary>兵種表攻擊距離。</summary>
        public short TroopAttackRange { get; private set; }

        /// <summary>兵種階段／世代。</summary>
        public int TroopStage { get; private set; }

        /// <summary>
        /// 最終戰鬥屬性（攻／防／擊破／破城／策略／建設）。
        /// 由 <see cref="CombatStatMath.Recalculate"/> 寫入。
        /// </summary>
        public CombatTroopStatBlock Stats { get; private set; }

        /// <summary>副將（戰鬥部隊至多一位）。</summary>
        public Officer ViceOfficer => ViceOfficers.Count > 0 ? ViceOfficers[0] : null;

        /// <summary>部隊整體統率。合成公式：<see cref="CombatStatMath.GetUnitLeadership"/> → BlendOfficerStat。</summary>
        public short UnitLeadership => CombatStatMath.GetUnitLeadership(this);

        /// <summary>部隊整體武力。合成公式：<see cref="CombatStatMath.GetUnitForce"/> → BlendOfficerStat。</summary>
        public short UnitForce => CombatStatMath.GetUnitForce(this);

        /// <summary>部隊整體智力。合成公式：<see cref="CombatStatMath.GetUnitIntelligence"/> → BlendOfficerStat。</summary>
        public short UnitIntelligence => CombatStatMath.GetUnitIntelligence(this);

        /// <summary>部隊整體政治。合成公式：<see cref="CombatStatMath.GetUnitPolicy"/> → BlendOfficerStat。</summary>
        public short UnitPolicy => CombatStatMath.GetUnitPolicy(this);

        /// <summary>部隊整體魅力。合成公式：<see cref="CombatStatMath.GetUnitCharisma"/> → BlendOfficerStat。</summary>
        public short UnitCharisma => CombatStatMath.GetUnitCharisma(this);

        /// <summary>部隊類型：戰鬥部隊。</summary>
        public override UnitKind Kind => UnitKind.Combat;

        /// <summary>所屬兵團（暫保留掛載）。</summary>
        public Legion ParentLegion { get; private set; }

        /// <summary>建立空戰鬥部隊；組隊請用 <see cref="Data.Units.UnitUtil.Create"/>。</summary>
        public Combat(string unitName, int factionBelonged)
            : base(unitName ?? "", factionBelonged)
        {
            SetGarrison(false);
        }

        /// <summary>重算最終戰鬥屬性並寫入 <see cref="Stats"/>。</summary>
        public void RecalculateStats() => CombatStatMath.Recalculate(this);

        /// <summary>由 <see cref="CombatStatMath"/> 寫入最終屬性。</summary>
        internal void ApplyStats(in CombatTroopStatBlock stats) => Stats = stats;

        /// <summary>設定唯一副將（取代既有副將）。</summary>
        public bool SetViceOfficer(Officer unitCopy)
        {
            ClearViceOfficers();
            if (unitCopy == null)
            {
                RecalculateStats();
                return true;
            }

            bool ok = base.AddViceOfficer(unitCopy);
            RecalculateStats();
            return ok;
        }

        /// <summary>從武將池設定唯一副將（id≤0 則清空）。</summary>
        public bool SetViceOfficerFromPool(int officerDefId) =>
            SetViceOfficer(officerDefId > 0 ? OfficerPool.Get(officerDefId) : null);

        /// <summary>新增副將（至多一位）。</summary>
        public override bool AddViceOfficer(Officer unitCopy)
        {
            if (ViceOfficers.Count >= 1)
                return false;
            bool ok = base.AddViceOfficer(unitCopy);
            if (ok)
                RecalculateStats();
            return ok;
        }

        /// <summary>設定主將後重算。</summary>
        public override void SetCommander(Officer officer)
        {
            base.SetCommander(officer);
            RecalculateStats();
        }

        /// <summary>從武將池設定主將後重算。</summary>
        public override void SetCommanderFromPool(int officerDefId)
        {
            base.SetCommanderFromPool(officerDefId);
            RecalculateStats();
        }

        /// <inheritdoc />
        public override void SetManpower(int totalSoldiers, int woundedCount = 0)
        {
            base.SetManpower(totalSoldiers, woundedCount);
            RecalculateStats();
        }

        /// <inheritdoc />
        public override void SetMorale(short value)
        {
            base.SetMorale(value);
            RecalculateStats();
        }

        /// <inheritdoc />
        public override void SetStamina(short value)
        {
            base.SetStamina(value);
            RecalculateStats();
        }

        /// <summary>設定兵科大類。</summary>
        public void SetTroopType(TroopType type) => TroopType = type;

        /// <summary>綁定兵種表後重算。</summary>
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
            RecalculateStats();
        }

        /// <summary>戰法暫停：恒為 0。</summary>
        public int CountEquippedSkills() => 0;

        /// <summary>掛載至所屬兵團。</summary>
        public void AttachToLegion(Legion legion) => ParentLegion = legion;

        /// <summary>暫定兵糧無限。</summary>
        public override int CalculateFoodConsumption() => 0;

        /// <summary>暫定兵糧無限。</summary>
        public override bool TryConsumeDailyFood() => true;

        /// <summary>野戰：回傳攻擊（詳見 <see cref="Stats"/>）。</summary>
        protected override int CalculateNonGarrisonCombatPower()
        {
            RecalculateStats();
            return Stats.Attack;
        }

        /// <summary>駐紮：同野戰（之後可疊據點加成）。</summary>
        protected override int CalculateGarrisonCombatPower()
        {
            RecalculateStats();
            return Stats.Attack;
        }

        /// <summary>戰法暫停：不匯出。</summary>
        internal void CollectEquippedSkills(
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> battle,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> strategy,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> mobility,
            System.Collections.Generic.List<Data.Persistence.SkillSaveEntry> defence)
        {
        }
    }
}
