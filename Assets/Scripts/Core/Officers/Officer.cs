using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Battle;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>
    /// 可玩的武將實作（Spike / 單機預設）。之後可再拆 PlayerOfficer / AiOfficer。
    /// </summary>
    public sealed class Officer : AbstractOfficer
    {
        public int RuntimeId { get; }

        public Officer(int runtimeId)
        {
            RuntimeId = runtimeId;
            officerFlag = new OfficerFlag
            {
                Health = HealthLevel.Fine,
                Death = false,
                Show = OfficerShowState.NoShow,
                Leader = false,
                Gender = OfficerGender.Male
            };
        }

        public void SetName(string last, string first, string alias = "")
        {
            lastName = last ?? "";
            firstName = first ?? "";
            aliasName = alias ?? "";
        }

        public void SetStats(short atk, short intel, short lead, short pol, short charm)
        {
            attack = atk;
            intelligence = intel;
            leadership = lead;
            policy = pol;
            charisma = charm;
            RefreshPerformance();
        }

        public void SetBelong(short factionId, short loyaltyValue = 100)
        {
            belong = factionId;
            loyalty = loyaltyValue;
            salary = factionId == 0 ? (short)0 : salary;
            officerFlag.Show = factionId == 0 ? OfficerShowState.OpenShow : OfficerShowState.Belonged;
        }

        public void SetBiography(string bio) => biography = bio ?? "";
        public void SetBirthYear(short year) => birthYear = year;
        public void SetAgeLimit(short limit) => ageLimit = limit;
        public void SetTitle(string t) => title = t ?? "";
        public void SetGender(OfficerGender gender) => officerFlag.Gender = gender;
        public void SetBattleSkills(OfficerBattleSkills skills) => battleSkills = skills;

        public void AddPersonality(int id, string key, string displayName) =>
            personalities.Add(new PersonalityDef { Id = id, Key = key, DisplayName = displayName });

        /// <summary>指派部隊用：複製當前數值，之後部隊內改動不影響 <see cref="OfficerPool"/>。</summary>
        public Officer CloneForUnit()
        {
            var copy = new Officer(RuntimeId);
            copy.SetName(LastName, FirstName, AliasName);
            copy.SetBiography(Biography);
            copy.SetStats(Attack, Intelligence, Leadership, Policy, Charisma);
            copy.SetBelong(Belong, Loyalty);
            copy.SetBirthYear(BirthYear);
            copy.SetAgeLimit(AgeLimit);
            copy.SetTitle(Title);
            copy.SetGender(officerFlag.Gender);
            copy.SetBattleSkills(battleSkills);
            copy.officerFlag = officerFlag;
            copy.stamina = stamina;
            copy.salary = salary;
            foreach (var p in personalities)
                copy.personalities.Add(p);
            foreach (var item in items)
                copy.items.Add(item);
            copy.RefreshPerformance();
            return copy;
        }

        public override void StaminaChange(short change)
        {
            int next = stamina + change;
            if (next < 0) next = 0;
            if (next > short.MaxValue) next = short.MaxValue;
            stamina = (short)next;
        }

        public override bool HealthChange(bool change)
        {
            if (officerFlag.Death)
                return false;

            int level = (int)officerFlag.Health;
            level += change ? -1 : 1;
            if (level < 0) level = 0;
            if (level > 3) level = 3;
            officerFlag.Health = (HealthLevel)level;
            if (level >= 3 && !change)
                officerFlag.Death = true;
            return true;
        }

        public override bool AcceptOffer(AbstractOfficer officer) => false;
        public override bool AcceptFight(AbstractOfficer officer) => false;
        public override bool AcceptDebate(AbstractOfficer officer) => false;

        public override bool DefendCavalrySkill(AbstractOfficer officer, Units.Unit selfUnit) => false;
        public override bool DefendSpearSkill(AbstractOfficer officer, Units.Unit selfUnit) => false;
        public override bool DefendArcherySkill(AbstractOfficer officer, Units.Unit selfUnit) => false;
        public override bool DefendShieldSkill(AbstractOfficer officer, Units.Unit selfUnit) => false;

        protected override void CalculatePerformance()
        {
            // Spike：先等同基礎值；之後可接裝備、性格、狀態修正
            attackPerform = attack;
            intelligencePerform = intelligence;
            leadershipPerform = leadership;
            policyPerform = policy;
            charismaPerform = charisma;
        }

        protected override short CalculateRandom(AbstractOfficer officer) =>
            (short)(leadershipPerform + attackPerform + intelligencePerform);
    }
}
