using System.Collections.Generic;

using ThreeKindoms.Core.Units;

using ThreeKindoms.Data.Battle;

using ThreeKindoms.Data.Officers;



namespace ThreeKindoms.Core.Officers

{

    /// <summary>武將執行時具體類；透過 Set* 方法修改狀態並同步發揮值。</summary>
    public sealed class Officer : AbstractOfficer

    {

        /// <summary>本局執行時唯一 id（與定義表 defId 對應）。</summary>
        public int RuntimeId { get; }



        /// <summary>建立新武將實例並套用預設旗標、體力與壽命。</summary>
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



        /// <summary>設定姓、名與字（別號）。</summary>
        public void SetName(string last, string first, string alias = "")

        {

            lastName = last ?? "";

            firstName = first ?? "";

            aliasName = alias ?? "";

        }



        /// <summary>設定台詞語氣、語音與頭像資源。</summary>
        public void SetPresentation(string toneValue, string voiceValue, string picturePath)

        {

            tone = toneValue ?? "";

            voice = voiceValue ?? "";

            picture = picturePath ?? "";

        }



        /// <summary>設定五維基礎值；可選同時設定體力並觸發發揮值重算。</summary>
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



        /// <summary>設定體力並重算發揮值。</summary>
        public void SetStamina(short value)

        {

            stamina = OfficerPerformanceRules.ClampStamina(value);

            RefreshPerformance();

        }



        /// <summary>登用或釋放武將：設定勢力、忠誠與登場狀態。</summary>
        public void SetBelong(short factionId, short loyaltyValue = 80)

        {

            belong = factionId;

            loyalty = loyaltyValue;

            salary = factionId == 0 ? (short)0 : salary;

            officerFlag.Show = factionId == 0 ? OfficerShowState.OpenShow : OfficerShowState.Belonged;

        }



        /// <summary>change忠誠度（0～100）。</summary>
        public void ChangeLoyalty(short value)
        {
            loyalty += value;
            loyalty = Clamp0To100(loyalty);
        } 



        /// <summary>設定人物傳記文字。</summary>
        public void SetBiography(string bio) => biography = bio ?? "";

        /// <summary>設定出生年份。</summary>
        public void SetBirthYear(short year) => birthYear = year;

        /// <summary>設定預期壽命（至少 1 年）。</summary>
        public void SetLifespan(short years) => lifespan = years < 1 ? (short)1 : years;

        /// <summary>設定官職或稱號。</summary>
        public void SetTitle(string t) => title = t ?? "";

        /// <summary>以列舉設定性別。</summary>
        public void SetGender(OfficerGender gender) => officerFlag.Gender = gender;

        /// <summary>以布林值設定性別（true＝男）。</summary>
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



        /// <summary>設定傷勢並重算發揮值。</summary>
        public void SetInjury(OfficerInjuryState state)

        {

            officerFlag.Injury = state;

            RefreshPerformance();

        }



        /// <summary>設定相性基準值。</summary>
        public void SetCompatibility(byte value) => compatibility = value;

        /// <summary>設定各兵種適性並正規化。</summary>
        public void SetTroopAptitude(OfficerTroopAptitude aptitude) =>

            troopAptitude = OfficerTroopAptitude.Normalize(aptitude);

        /// <summary>覆寫戰法技能資料。</summary>
        public void SetBattleSkills(OfficerBattleSkills skills) => battleSkills = skills;



        /// <summary>新增一項個性；id 無效或重複時失敗。</summary>
        public bool AddPersonality(int id, string key, string displayName)

        {

            if (id <= 0)

                return false;

            return personalities.Add(new PersonalityDef { Id = id, Key = key, DisplayName = displayName });

        }



        /// <summary>移除指定 id 的個性。</summary>
        public bool RemovePersonality(int id) =>

            id > 0 && personalities.Remove(new PersonalityDef { Id = id });



        /// <summary>新增道具 id。</summary>
        public bool AddItemId(int id) => id > 0 && itemIds.Add(id);

        /// <summary>移除道具 id。</summary>
        public bool RemoveItemId(int id) => id > 0 && itemIds.Remove(id);



        /// <summary>替換人際關係；若 Pool 已初始化則同步雙向連結。</summary>
        public void SetRelations(OfficerRelations relations)

        {

            if (OfficerPool.IsInitialized)

                OfficerRelationsSync.Apply(this, relations, OfficerPool.GetShared);

            else

                ReplaceLocalRelations(relations);

        }



        /// <summary>僅更新本將本地人際列表（不經 Pool 雙向同步）。</summary>
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



        /// <summary>嘗試將對方加入親愛列表（含上限檢查）。</summary>
        internal bool TryAddBeloved(int otherId) => TryAddCapped(otherId, belovedOfficerIds, OfficerRelationsRules.MaxBeloved);

        /// <summary>嘗試將對方加入厭惡列表。</summary>
        internal bool TryAddHated(int otherId) => TryAddUnique(otherId, hatedOfficerIds);

        /// <summary>嘗試將對方加入義兄弟列表（含上限檢查）。</summary>
        internal bool TryAddSwornBrother(int otherId) =>

            TryAddCapped(otherId, swornBrotherIds, OfficerRelationsRules.MaxSwornBrothers);

        /// <summary>嘗試將對方加入配偶列表（含性別上限檢查）。</summary>
        internal bool TryAddSpouse(int otherId) =>

            TryAddCapped(otherId, spouseOfficerIds, OfficerRelationsRules.MaxSpouses(officerFlag.Gender));



        /// <summary>從親愛列表移除對方 id。</summary>
        internal void RemoveBeloved(int otherId) => belovedOfficerIds.Remove(otherId);

        /// <summary>從厭惡列表移除對方 id。</summary>
        internal void RemoveHated(int otherId) => hatedOfficerIds.Remove(otherId);

        /// <summary>從義兄弟列表移除對方 id。</summary>
        internal void RemoveSwornBrother(int otherId) => swornBrotherIds.Remove(otherId);

        /// <summary>從配偶列表移除對方 id。</summary>
        internal void RemoveSpouse(int otherId) => spouseOfficerIds.Remove(otherId);



        /// <inheritdoc/>
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



        /// <inheritdoc/>
        public override bool AcceptOffer(AbstractOfficer officer) => false;

        /// <inheritdoc/>
        public override bool AcceptFight(AbstractOfficer officer) => false;

        /// <inheritdoc/>
        public override bool AcceptDebate(AbstractOfficer officer) => false;



        /// <inheritdoc/>
        public override bool DefendCavalrySkill(AbstractOfficer officer, Unit selfUnit) => false;

        /// <inheritdoc/>
        public override bool DefendSpearSkill(AbstractOfficer officer, Unit selfUnit) => false;

        /// <inheritdoc/>
        public override bool DefendArcherySkill(AbstractOfficer officer, Unit selfUnit) => false;

        /// <inheritdoc/>
        public override bool DefendShieldSkill(AbstractOfficer officer, Unit selfUnit) => false;



        /// <inheritdoc/>
        protected override void CalculatePerformance()
        {
            if (!officerFlag.IsAlive)
            {
                attackPerform = 0;
                intelligencePerform = 0;
                leadershipPerform = 0;
                policyPerform = 0;
                charismaPerform = 0;
                return;
            }

            attackPerform = OfficerPerformanceRules.ComputePerform(
                attack, officerFlag.Injury, true, stamina, itemIds);
            intelligencePerform = OfficerPerformanceRules.ComputePerform(
                intelligence, officerFlag.Injury, true, stamina, itemIds);
            leadershipPerform = OfficerPerformanceRules.ComputePerform(
                leadership, officerFlag.Injury, true, stamina, itemIds);
            policyPerform = OfficerPerformanceRules.ComputePerform(
                policy, officerFlag.Injury, true, stamina, itemIds);
            charismaPerform = OfficerPerformanceRules.ComputePerform(
                charisma, officerFlag.Injury, true, stamina, itemIds);
        }



        /// <summary>以執行緒安全亂數產生整數。</summary>
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

