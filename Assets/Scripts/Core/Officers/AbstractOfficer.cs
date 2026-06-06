using System.Collections.Generic;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Battle;
using ThreeKindoms.Data.Items;
using ThreeKindoms.Data.Officers;

namespace ThreeKindoms.Core.Officers
{
    /// <summary>
    /// 對應 C++ Officer::AbstractOfficer。
    /// 規則與戰鬥相關方法為 abstract；欄位與 C++ protected 區塊對齊。
    /// </summary>
    public abstract class AbstractOfficer
    {
        // --- 姓名與敘述 ---
        protected string lastName = "";
        protected string firstName = "";
        protected string aliasName = "";
        protected string biography = "";

        // --- 五維（C++ attack = 武力, policy = 政治）---
        protected short attack;
        protected short intelligence;
        protected short leadership;
        protected short policy;
        protected short charisma;

        protected short stamina;

        // --- 表現/發揮值（calculatePerformance 產出）---
        protected short attackPerform;
        protected short intelligencePerform;
        protected short leadershipPerform;
        protected short policyPerform;
        protected short charismaPerform;

        protected OfficerFlag officerFlag;

        /// <summary>0 = 不屬於任何勢力</summary>
        protected short belong;

        /// <summary>忠誠；leader=-1 等規則在具體邏輯中處理</summary>
        protected short loyalty;

        protected string title = "";

        /// <summary>未歸屬時應為 0</summary>
        protected short salary;

        protected readonly List<PersonalityDef> personalities = new();
        protected short birthYear;
        protected short ageLimit;

        protected readonly List<ItemInstance> items = new();
        protected byte[] pictureBuffer = System.Array.Empty<byte>();

        protected OfficerBattleSkills battleSkills;

        // --- 公開唯讀屬性（給 UI / 系統讀，避免外部改 protected）---
        public string LastName => lastName;
        public string FirstName => firstName;
        public string AliasName => aliasName;
        public string Biography => biography;
        public string FullName => string.IsNullOrEmpty(aliasName)
            ? $"{lastName}{firstName}"
            : aliasName;

        public short Attack => attack;
        public short Intelligence => intelligence;
        public short Leadership => leadership;
        public short Policy => policy;
        public short Charisma => charisma;
        public short Stamina => stamina;

        public short AttackPerform => attackPerform;
        public short IntelligencePerform => intelligencePerform;
        public short LeadershipPerform => leadershipPerform;
        public short PolicyPerform => policyPerform;
        public short CharismaPerform => charismaPerform;

        public OfficerFlag OfficerFlag => officerFlag;
        public short Belong => belong;
        public short Loyalty => loyalty;
        public string Title => title;
        public short Salary => salary;
        public IReadOnlyList<PersonalityDef> Personalities => personalities;
        public short BirthYear => birthYear;
        public short AgeLimit => ageLimit;
        public IReadOnlyList<ItemInstance> Items => items;
        public byte[] PictureBuffer => pictureBuffer;
        public ref OfficerBattleSkills BattleSkills => ref battleSkills;

        public bool IsDead => officerFlag.Death;
        public bool IsBelonged => belong != 0 && officerFlag.Show == OfficerShowState.Belonged;

        // --- C++ 虛函式 ---
        public abstract void StaminaChange(short change);

        /// <summary>C++ healthChange(bool change)；true/false 語意由子類實作（如好轉/惡化）。</summary>
        public abstract bool HealthChange(bool change);

        public abstract bool AcceptOffer(AbstractOfficer officer);
        public abstract bool AcceptFight(AbstractOfficer officer);
        public abstract bool AcceptDebate(AbstractOfficer officer);

        public abstract bool DefendCavalrySkill(AbstractOfficer officer, Units.Unit selfUnit);
        public abstract bool DefendSpearSkill(AbstractOfficer officer, Units.Unit selfUnit);
        public abstract bool DefendArcherySkill(AbstractOfficer officer, Units.Unit selfUnit);
        public abstract bool DefendShieldSkill(AbstractOfficer officer, Units.Unit selfUnit);

        protected abstract void CalculatePerformance();
        protected abstract short CalculateRandom(AbstractOfficer officer);

        /// <summary>子類或工廠在修改五維後呼叫，刷新 *Perform 欄位。</summary>
        protected void RefreshPerformance() => CalculatePerformance();
    }
}
