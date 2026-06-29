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



        /// <summary>姓。</summary>
        public string LastName => lastName;

        /// <summary>名。</summary>
        public string FirstName => firstName;

        /// <summary>字／別號。</summary>
        public string AliasName => aliasName;

        /// <summary>人物傳記文字。</summary>
        public string Biography => biography;

        /// <summary>語氣／台詞風格標識。</summary>
        public string Tone => tone;

        /// <summary>語音資源標識。</summary>
        public string Voice => voice;

        /// <summary>頭像圖檔路徑或資源名。</summary>
        public string Picture => picture;



        /// <summary>姓名合併顯示（姓＋名）。</summary>
        public string FullName => $"{lastName}{firstName}";

        /// <summary>介面顯示用全名；有字則附帶括號別號。</summary>
        public string DisplayName => string.IsNullOrEmpty(aliasName) ? FullName : $"{FullName}（{aliasName}）";



        /// <summary>武力基礎值。</summary>
        public byte Attack => attack;

        /// <summary>智力基礎值。</summary>
        public byte Intelligence => intelligence;

        /// <summary>統率基礎值。</summary>
        public byte Leadership => leadership;

        /// <summary>政治基礎值。</summary>
        public byte Policy => policy;

        /// <summary>魅力基礎值。</summary>
        public byte Charisma => charisma;

        /// <summary>體力（0～100）。</summary>
        public byte Stamina => stamina;



        /// <summary>武力發揮值（傷勢、體力、道具等修正後）。</summary>
        public byte AttackPerform => attackPerform;

        /// <summary>智力發揮值（傷勢、體力、道具等修正後）。</summary>
        public byte IntelligencePerform => intelligencePerform;

        /// <summary>統率發揮值（傷勢、體力、道具等修正後）。</summary>
        public byte LeadershipPerform => leadershipPerform;

        /// <summary>政治發揮值（傷勢、體力、道具等修正後）。</summary>
        public byte PolicyPerform => policyPerform;

        /// <summary>魅力發揮值（傷勢、體力、道具等修正後）。</summary>
        public byte CharismaPerform => charismaPerform;



        /// <summary>旗標集合：性別、傷勢、存活、登場狀態等。</summary>
        public OfficerFlag OfficerFlag => officerFlag;

        /// <summary>所屬勢力 id；0 表示在野。</summary>
        public short Belong => belong;

        /// <summary>兵團主將（Leader）武將 defId；與 <see cref="Belong"/>（勢力）不同。</summary>
        public int LegionLeaderId => legionLeaderId;

        /// <summary>對所屬勢力的忠誠度（0～100）。</summary>
        public short Loyalty => loyalty;

        /// <summary>官職或稱號文字。</summary>
        public string Title => title;

        /// <summary>俸祿數值。</summary>
        public short Salary => salary;



        /// <summary>已裝備或持有的個性定義集合。</summary>
        public IReadOnlyCollection<PersonalityDef> Personalities => personalities;

        /// <summary>已裝備道具 id 集合。</summary>
        public IReadOnlyCollection<int> ItemIds => itemIds;



        /// <summary>出生年份。</summary>
        public short BirthYear => birthYear;

        /// <summary>預期壽命（年）。</summary>
        public short Lifespan => lifespan;

        /// <summary>預計卒年（出生年＋壽命）。</summary>
        public short DeathYear => (short)(birthYear + lifespan);

        /// <summary>相性基準值（與他將互動用）。</summary>
        public byte Compatibility => compatibility;

        /// <summary>各兵種適性等級集合。</summary>
        public OfficerTroopAptitude TroopAptitude => troopAptitude;

        /// <summary>查詢指定兵種的適性等級。</summary>
        public TroopAptitudeGrade GetTroopAptitude(TroopType troop) => troopAptitude.Get(troop);



        /// <summary>親愛武將 id 列表。</summary>
        public IReadOnlyList<int> BelovedOfficerIds => belovedOfficerIds;

        /// <summary>厭惡武將 id 列表。</summary>
        public IReadOnlyList<int> HatedOfficerIds => hatedOfficerIds;

        /// <summary>義兄弟武將 id 列表。</summary>
        public IReadOnlyList<int> SwornBrotherIds => swornBrotherIds;

        /// <summary>配偶武將 id 列表。</summary>
        public IReadOnlyList<int> SpouseOfficerIds => spouseOfficerIds;



        /// <summary>目前傷勢等級。</summary>
        public OfficerInjuryState Injury => officerFlag.Injury;

        /// <summary>是否仍存活。</summary>
        public bool IsAlive => officerFlag.IsAlive;

        /// <summary>是否為男性。</summary>
        public bool IsMale => officerFlag.Gender == OfficerGender.Male;

        /// <summary>是否為女性。</summary>
        public bool IsFemale => officerFlag.Gender == OfficerGender.Female;



        /// <summary>頭像二進位快取（載入用）。</summary>
        public byte[] PictureBuffer => pictureBuffer;

        /// <summary>戰法技能資料（可變 ref 存取）。</summary>
        public ref OfficerBattleSkills BattleSkills => ref battleSkills;



        /// <summary>是否已死亡（<see cref="IsAlive"/> 的反義）。</summary>
        public bool IsDead => !officerFlag.IsAlive;

        /// <summary>是否已登用並隸屬某勢力。</summary>
        public bool IsBelonged => belong != 0 && officerFlag.Show == OfficerShowState.Belonged;



        /// <summary>是否持有指定 id 的個性。</summary>
        public bool HasPersonalityId(int id) =>

            personalities.Contains(new PersonalityDef { Id = id });



        /// <summary>變更傷勢等級（惡化或好轉一階）。</summary>
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



        /// <summary>依當前狀態重算五維發揮值。</summary>
        protected abstract void CalculatePerformance();



        /// <summary>產生含上下界的隨機整數（個性／劇本判定用）。</summary>
        public abstract int RollRandom(int minInclusive, int maxInclusive);



        /// <summary>觸發發揮值重算（屬性變更後呼叫）。</summary>
        protected void RefreshPerformance() => CalculatePerformance();

    }

}


