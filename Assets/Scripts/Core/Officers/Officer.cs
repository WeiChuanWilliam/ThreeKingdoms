using System.Collections.Generic;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Battle;
using ThreeKindoms.Data.Items;
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
                Injury = OfficerInjuryState.Normal,
                Show = OfficerShowState.NoShow,
                Leader = false,
                Gender = OfficerGender.Male
            };
            stamina = OfficerConfigUtil.IsLoaded
                ? OfficerConfigUtil.GetDefaultStamina()
                : (short)100;
            compatibility = OfficerConfigUtil.IsLoaded
                ? OfficerConfigUtil.GetDefaultCompatibility()
                : (byte)145;
            troopAptitude = OfficerTroopAptitude.DefaultC;
            lifespan = OfficerConfigUtil.IsLoaded
                ? OfficerConfigUtil.GetDefaultLifespan()
                : (short)60;
        }

        public void SetName(string last, string first, string alias = "")
        {
            lastName = last ?? "";
            firstName = first ?? "";
            aliasName = alias ?? "";
        }

        public void SetPresentation(string toneValue, string voiceValue, string picturePath)
        {
            tone = toneValue ?? "";
            voice = voiceValue ?? "";
            picture = picturePath ?? "";
        }

        public void SetStats(short lead, short atk, short intel, short pol, short charm, short staminaValue = -1)
        {
            leadership = lead;
            attack = atk;
            intelligence = intel;
            policy = pol;
            charisma = charm;
            if (staminaValue >= 0)
                stamina = staminaValue;
            RefreshPerformance();
        }

        public void SetBelong(short factionId, short loyaltyValue = 100)
        {
            belong = factionId;
            loyalty = loyaltyValue;
            salary = factionId == 0 ? (short)0 : salary;
            officerFlag.Show = factionId == 0 ? OfficerShowState.OpenShow : OfficerShowState.Belonged;
        }

        public void SetLoyalty(short value) => loyalty = Clamp0To100(value);

        public void SetBiography(string bio) => biography = bio ?? "";
        public void SetBirthYear(short year) => birthYear = year;
        public void SetLifespan(short years) => lifespan = years < 1 ? (short)1 : years;
        public void SetTitle(string t) => title = t ?? "";
        public void SetGender(OfficerGender gender) => officerFlag.Gender = gender;
        public void SetGender(bool isMale) =>
            officerFlag.Gender = isMale ? OfficerGender.Male : OfficerGender.Female;

        public void SetInjury(OfficerInjuryState state) =>
            officerFlag.Injury = state;

        public void SetCompatibility(byte value) => compatibility = value;
        public void SetTroopAptitude(OfficerTroopAptitude aptitude) =>
            troopAptitude = OfficerTroopAptitude.Normalize(aptitude);
        public void SetBattleSkills(OfficerBattleSkills skills) => battleSkills = skills;

        public void AddPersonality(int id, string key, string displayName) =>
            personalities.Add(new PersonalityDef { Id = id, Key = key, DisplayName = displayName });

        public void AddPersonalityId(int id)
        {
            if (id > 0)
                personalityIds.Add(id);
        }

        public void ClearPersonalities()
        {
            personalities.Clear();
            personalityIds.Clear();
        }

        public void AddItemId(int id)
        {
            if (id > 0)
                itemIds.Add(id);
        }

        public void ClearItemIds() => itemIds.Clear();

        public void SetRelations(OfficerRelations relations)
        {
            belovedOfficerIds.Clear();
            hatedOfficerIds.Clear();
            swornBrotherIds.Clear();
            spouseOfficerIds.Clear();
            if (relations == null)
                return;

            CopyCapped(relations.BelovedOfficerIds, belovedOfficerIds, OfficerRelationsRules.MaxBeloved);
            CopyIds(relations.HatedOfficerIds, hatedOfficerIds);
            CopyCapped(relations.SwornBrotherIds, swornBrotherIds, OfficerRelationsRules.MaxSwornBrothers);
            CopyCapped(relations.SpouseOfficerIds, spouseOfficerIds,
                OfficerRelationsRules.MaxSpouses(officerFlag.Gender));
        }

        /// <summary>指派部隊用：複製當前數值，之後部隊內改動不影響 <see cref="OfficerPool"/>。</summary>
        public Officer CloneForUnit()
        {
            var copy = new Officer(RuntimeId);
            copy.SetName(LastName, FirstName, AliasName);
            copy.SetPresentation(Tone, Voice, Picture);
            copy.SetBiography(Biography);
            copy.SetStats(Leadership, Attack, Intelligence, Policy, Charisma, Stamina);
            copy.SetBelong(Belong, Loyalty);
            copy.SetBirthYear(BirthYear);
            copy.SetLifespan(Lifespan);
            copy.SetTitle(Title);
            copy.SetGender(officerFlag.Gender);
            copy.SetInjury(Injury);
            copy.SetCompatibility(Compatibility);
            copy.SetTroopAptitude(TroopAptitude);
            copy.SetBattleSkills(battleSkills);
            copy.officerFlag = officerFlag;
            copy.salary = salary;
            foreach (PersonalityDef p in personalities)
                copy.personalities.Add(p);
            foreach (int pid in personalityIds)
                copy.personalityIds.Add(pid);
            foreach (int iid in itemIds)
                copy.itemIds.Add(iid);
            foreach (int id in belovedOfficerIds)
                copy.belovedOfficerIds.Add(id);
            foreach (int id in hatedOfficerIds)
                copy.hatedOfficerIds.Add(id);
            foreach (int id in swornBrotherIds)
                copy.swornBrotherIds.Add(id);
            foreach (int id in spouseOfficerIds)
                copy.spouseOfficerIds.Add(id);
            foreach (ItemInstance item in items)
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

        public override bool HealthChange(bool worsen)
        {
            if (IsDead)
                return false;

            int level = (int)officerFlag.Injury;
            level += worsen ? 1 : -1;
            if (level < 0) level = 0;
            if (level > 3) level = 3;
            officerFlag.Injury = (OfficerInjuryState)level;
            return true;
        }

        public override bool AcceptOffer(AbstractOfficer officer) => false;
        public override bool AcceptFight(AbstractOfficer officer) => false;
        public override bool AcceptDebate(AbstractOfficer officer) => false;

        public override bool DefendCavalrySkill(AbstractOfficer officer, Unit selfUnit) => false;
        public override bool DefendSpearSkill(AbstractOfficer officer, Unit selfUnit) => false;
        public override bool DefendArcherySkill(AbstractOfficer officer, Unit selfUnit) => false;
        public override bool DefendShieldSkill(AbstractOfficer officer, Unit selfUnit) => false;

        protected override void CalculatePerformance()
        {
            attackPerform = attack;
            intelligencePerform = intelligence;
            leadershipPerform = leadership;
            policyPerform = policy;
            charismaPerform = charisma;
        }

        protected override short CalculateRandom(AbstractOfficer officer) =>
            (short)(leadershipPerform + attackPerform + intelligencePerform);

        static void CopyIds(int[] source, List<int> target)
        {
            if (source == null)
                return;
            var seen = new HashSet<int>();
            foreach (int id in source)
            {
                if (id <= 0 || !seen.Add(id))
                    continue;
                target.Add(id);
            }
        }

        static void CopyCapped(int[] source, List<int> target, int max)
        {
            if (source == null || max <= 0)
                return;
            var seen = new HashSet<int>();
            int count = 0;
            foreach (int id in source)
            {
                if (id <= 0 || !seen.Add(id))
                    continue;
                target.Add(id);
                count++;
                if (count >= max)
                    break;
            }
        }

        static short Clamp0To100(short v)
        {
            if (v < 0) return 0;
            if (v > 100) return 100;
            return v;
        }
    }
}
