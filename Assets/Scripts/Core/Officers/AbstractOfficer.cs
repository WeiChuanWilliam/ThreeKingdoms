using System.Collections.Generic;

using ThreeKindoms.Core.Units;

using ThreeKindoms.Data.Battle;

using ThreeKindoms.Data.Officers;

using ThreeKindoms.Data.Units;



namespace ThreeKindoms.Core.Officers

{

    /// <summary>

    /// 武將執行時資料與行為抽象。欄位修改走 <see cref="Officer"/> 的 Set*。

    /// </summary>

    public abstract class AbstractOfficer

    {

        protected string lastName = "";

        protected string firstName = "";

        protected string aliasName = "";

        protected string biography = "";

        protected string tone = "";

        protected string voice = "";

        protected string picture = "";



        /// <summary>六圍基礎值（byte，設計區間 1～100）。</summary>

        protected byte attack;

        protected byte intelligence;

        protected byte leadership;

        protected byte policy;

        protected byte charisma;



        /// <summary>體力 0～100。</summary>

        protected byte stamina;



        protected byte attackPerform;

        protected byte intelligencePerform;

        protected byte leadershipPerform;

        protected byte policyPerform;

        protected byte charismaPerform;



        protected OfficerFlag officerFlag;



        protected short belong;

        /// <summary>所屬兵團（<see cref="Units.Legion"/>）主將 defId（Leader）；自領兵團時＝自身 id；0＝未編入兵團。</summary>
        protected int legionLeaderId;

        protected short loyalty;

        protected string title = "";

        protected short salary;



        /// <summary>個性集合；以 <see cref="PersonalityDef.Id"/> 為唯一鍵（Equals/GetHashCode）。</summary>

        protected readonly HashSet<PersonalityDef> personalities = new();



        /// <summary>道具 id 集合。</summary>

        protected readonly HashSet<int> itemIds = new();



        protected short birthYear;

        protected short lifespan;

        protected byte compatibility = 145;

        protected OfficerTroopAptitude troopAptitude = OfficerTroopAptitude.DefaultC;



        protected readonly List<int> belovedOfficerIds = new();

        protected readonly List<int> hatedOfficerIds = new();

        protected readonly List<int> swornBrotherIds = new();

        protected readonly List<int> spouseOfficerIds = new();



        protected byte[] pictureBuffer = System.Array.Empty<byte>();

        protected OfficerBattleSkills battleSkills;



        public string LastName => lastName;

        public string FirstName => firstName;

        public string AliasName => aliasName;

        public string Biography => biography;

        public string Tone => tone;

        public string Voice => voice;

        public string Picture => picture;



        public string FullName => $"{lastName}{firstName}";

        public string DisplayName => string.IsNullOrEmpty(aliasName) ? FullName : $"{FullName}（{aliasName}）";



        public byte Attack => attack;

        public byte Intelligence => intelligence;

        public byte Leadership => leadership;

        public byte Policy => policy;

        public byte Charisma => charisma;

        public byte Stamina => stamina;



        public byte AttackPerform => attackPerform;

        public byte IntelligencePerform => intelligencePerform;

        public byte LeadershipPerform => leadershipPerform;

        public byte PolicyPerform => policyPerform;

        public byte CharismaPerform => charismaPerform;



        public OfficerFlag OfficerFlag => officerFlag;

        public short Belong => belong;

        /// <summary>兵團主將（Leader）武將 defId；與 <see cref="Belong"/>（勢力）不同。</summary>
        public int LegionLeaderId => legionLeaderId;

        public short Loyalty => loyalty;

        public string Title => title;

        public short Salary => salary;



        public IReadOnlyCollection<PersonalityDef> Personalities => personalities;

        public IReadOnlyCollection<int> ItemIds => itemIds;



        public short BirthYear => birthYear;

        public short Lifespan => lifespan;

        public short DeathYear => (short)(birthYear + lifespan);

        public byte Compatibility => compatibility;

        public OfficerTroopAptitude TroopAptitude => troopAptitude;

        public TroopAptitudeGrade GetTroopAptitude(TroopType troop) => troopAptitude.Get(troop);



        public IReadOnlyList<int> BelovedOfficerIds => belovedOfficerIds;

        public IReadOnlyList<int> HatedOfficerIds => hatedOfficerIds;

        public IReadOnlyList<int> SwornBrotherIds => swornBrotherIds;

        public IReadOnlyList<int> SpouseOfficerIds => spouseOfficerIds;



        public OfficerInjuryState Injury => officerFlag.Injury;

        public bool IsAlive => officerFlag.IsAlive;

        public bool IsMale => officerFlag.Gender == OfficerGender.Male;

        public bool IsFemale => officerFlag.Gender == OfficerGender.Female;



        public byte[] PictureBuffer => pictureBuffer;

        public ref OfficerBattleSkills BattleSkills => ref battleSkills;



        public bool IsDead => !officerFlag.IsAlive;

        public bool IsBelonged => belong != 0 && officerFlag.Show == OfficerShowState.Belonged;



        public bool HasPersonalityId(int id) =>

            personalities.Contains(new PersonalityDef { Id = id });



        public abstract bool HealthChange(bool worsen);



        /// <summary>登用交涉：是否接受對方勢力招降（C++ acceptOffer）。</summary>

        public abstract bool AcceptOffer(AbstractOfficer officer);



        /// <summary>是否接受單挑（C++ acceptFight）。</summary>

        public abstract bool AcceptFight(AbstractOfficer officer);



        /// <summary>是否接受舌戰（C++ acceptDebate）。</summary>

        public abstract bool AcceptDebate(AbstractOfficer officer);



        /// <summary>是否對來襲騎兵戰法發動防禦戰法（C++ defendCavalrySkill）。</summary>

        public abstract bool DefendCavalrySkill(AbstractOfficer officer, Units.Unit selfUnit);



        /// <summary>是否對來襲槍兵戰法發動防禦戰法。</summary>

        public abstract bool DefendSpearSkill(AbstractOfficer officer, Units.Unit selfUnit);



        /// <summary>是否對來襲弓兵戰法發動防禦戰法。</summary>

        public abstract bool DefendArcherySkill(AbstractOfficer officer, Units.Unit selfUnit);



        /// <summary>是否對來襲盾／步兵系戰法發動防禦戰法。</summary>

        public abstract bool DefendShieldSkill(AbstractOfficer officer, Units.Unit selfUnit);



        protected abstract void CalculatePerformance();



        public abstract int RollRandom(int minInclusive, int maxInclusive);



        protected void RefreshPerformance() => CalculatePerformance();

    }

}


