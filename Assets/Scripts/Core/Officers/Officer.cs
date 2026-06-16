using System.Collections.Generic;

using ThreeKindoms.Core.Units;

using ThreeKindoms.Data.Battle;

using ThreeKindoms.Data.Officers;



namespace ThreeKindoms.Core.Officers

{

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

                Gender = OfficerGender.Male,

                IsAlive = true

            };

            stamina = OfficerConfigUtil.IsLoaded

                ? (byte)OfficerConfigUtil.GetDefaultStamina()

                : (byte)100;

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

            leadership = OfficerPerformanceRules.ClampBaseStat(lead);

            attack = OfficerPerformanceRules.ClampBaseStat(atk);

            intelligence = OfficerPerformanceRules.ClampBaseStat(intel);

            policy = OfficerPerformanceRules.ClampBaseStat(pol);

            charisma = OfficerPerformanceRules.ClampBaseStat(charm);

            if (staminaValue >= 0)

                SetStamina(staminaValue);

            else

                RefreshPerformance();

        }



        public void SetStamina(short value)

        {

            stamina = OfficerPerformanceRules.ClampStamina(value);

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



        /// <summary>

        /// 設定存活。死亡時不 RefreshPerformance；應觸發劇本 Pool 移除此武將（見 TODO）。

        /// 遊戲中武將通常只會活著；不支援死而復生。

        /// </summary>

        public void SetAlive(bool alive)

        {

            if (!alive)

            {

                officerFlag.IsAlive = false;

                // TODO: OfficerPool.RemoveOfficer(RuntimeId) 或 ScenarioOfficerPool — 從本局 Pool 移除，無需 RefreshPerformance

                return;

            }

            officerFlag.IsAlive = true;

        }



        /// <summary>所屬兵團（Legion）主將 defId；自領兵團時設為 <see cref="RuntimeId"/>。</summary>

        public void SetLegionLeader(int commanderDefId) =>

            legionLeaderId = commanderDefId < 0 ? 0 : commanderDefId;



        public void SetInjury(OfficerInjuryState state)

        {

            officerFlag.Injury = state;

            RefreshPerformance();

        }



        public void SetCompatibility(byte value) => compatibility = value;

        public void SetTroopAptitude(OfficerTroopAptitude aptitude) =>

            troopAptitude = OfficerTroopAptitude.Normalize(aptitude);

        public void SetBattleSkills(OfficerBattleSkills skills) => battleSkills = skills;



        public bool AddPersonality(int id, string key, string displayName)

        {

            if (id <= 0)

                return false;

            return personalities.Add(new PersonalityDef { Id = id, Key = key, DisplayName = displayName });

        }



        public bool RemovePersonality(int id) =>

            id > 0 && personalities.Remove(new PersonalityDef { Id = id });



        public bool AddItemId(int id) => id > 0 && itemIds.Add(id);

        public bool RemoveItemId(int id) => id > 0 && itemIds.Remove(id);



        public void SetRelations(OfficerRelations relations)

        {

            if (OfficerPool.IsInitialized)

                OfficerRelationsSync.Apply(this, relations, OfficerPool.GetShared);

            else

                ReplaceLocalRelations(relations);

        }



        public void ReplaceLocalRelations(OfficerRelations relations)

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



        internal bool TryAddBeloved(int otherId) => TryAddCapped(otherId, belovedOfficerIds, OfficerRelationsRules.MaxBeloved);

        internal bool TryAddHated(int otherId) => TryAddUnique(otherId, hatedOfficerIds);

        internal bool TryAddSwornBrother(int otherId) =>

            TryAddCapped(otherId, swornBrotherIds, OfficerRelationsRules.MaxSwornBrothers);

        internal bool TryAddSpouse(int otherId) =>

            TryAddCapped(otherId, spouseOfficerIds, OfficerRelationsRules.MaxSpouses(officerFlag.Gender));



        internal void RemoveBeloved(int otherId) => belovedOfficerIds.Remove(otherId);

        internal void RemoveHated(int otherId) => hatedOfficerIds.Remove(otherId);

        internal void RemoveSwornBrother(int otherId) => swornBrotherIds.Remove(otherId);

        internal void RemoveSpouse(int otherId) => spouseOfficerIds.Remove(otherId);



        public override bool HealthChange(bool worsen)

        {

            if (IsDead)

                return false;



            int level = (int)officerFlag.Injury;

            level += worsen ? 1 : -1;

            if (level < 0) level = 0;

            if (level > (int)OfficerInjuryState.Severe)

                level = (int)OfficerInjuryState.Severe;

            officerFlag.Injury = (OfficerInjuryState)level;

            RefreshPerformance();

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

            // TODO: 實作五維發揮值計算（統率／武力／智力／政治／魅力）。

            // 可參考 OfficerPerformanceRules；完成後由 RefreshPerformance() 呼叫。

        }



        public override int RollRandom(int minInclusive, int maxInclusive)

        {

            if (minInclusive > maxInclusive)

                (minInclusive, maxInclusive) = (maxInclusive, minInclusive);

            return System.Random.Shared.Next(minInclusive, maxInclusive + 1);

        }



        static bool TryAddCapped(int id, List<int> target, int max)

        {

            if (id <= 0 || max <= 0 || target.Contains(id))

                return false;

            if (target.Count >= max)

                return false;

            target.Add(id);

            return true;

        }



        static bool TryAddUnique(int id, List<int> target)

        {

            if (id <= 0 || target.Contains(id))

                return false;

            target.Add(id);

            return true;

        }



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

