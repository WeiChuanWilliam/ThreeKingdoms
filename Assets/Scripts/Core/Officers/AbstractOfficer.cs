using System.Collections.Generic;

using ThreeKindoms.Core.Units;

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

        /// <summary>
        /// 暫定＝勢力 id（<see cref="belong"/>）；勢力 id＝執掌該勢力的武將 defId。
        /// 日後可與 belong 分離，改表「所屬兵團主將」。
        /// </summary>
        protected int legionLeaderId;

        /// <summary>是否出戰：在戰鬥／運輸部隊任職為 true；僅在兵團或待命為 false。</summary>
        protected bool isDeployed;

        protected short loyalty;

        protected string title = "";

        protected short salary;



        /// <summary>個性集合；以 <see cref="PersonalityDef.Id"/> 為唯一鍵（Equals/GetHashCode）。</summary>

        protected readonly HashSet<PersonalityDef> personalities = new();



        /// <summary>道具 id 集合。</summary>

        protected readonly HashSet<int> itemIds = new();



        protected short birthYear;

        protected short deathYear;

        protected byte compatibility = 145;

        protected OfficerTroopAptitude troopAptitude = OfficerTroopAptitude.DefaultC;



        protected byte[] pictureBuffer = System.Array.Empty<byte>();



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

        /// <summary>戰鬥用武力：有發揮值用發揮值，否則用基礎值。</summary>
        public byte EffectiveAttack => PickPerform(attackPerform, attack);

        /// <summary>戰鬥用智力。</summary>
        public byte EffectiveIntelligence => PickPerform(intelligencePerform, intelligence);

        /// <summary>戰鬥用統率。</summary>
        public byte EffectiveLeadership => PickPerform(leadershipPerform, leadership);

        /// <summary>戰鬥用政治。</summary>
        public byte EffectivePolicy => PickPerform(policyPerform, policy);

        /// <summary>戰鬥用魅力。</summary>
        public byte EffectiveCharisma => PickPerform(charismaPerform, charisma);

        /// <summary>戰鬥力相關三維合計（武＋智＋統，皆用 Effective*）。</summary>
        public int CombatRelevantSum =>
            EffectiveAttack + EffectiveIntelligence + EffectiveLeadership;

        /// <summary>
        /// 主將權重 2、副將權重 1 合算後的戰鬥三維合計。
        /// 僅副將時回傳副將；皆無則 0。
        /// </summary>
        public static int BlendedCombatRelevantSum(Officer commander, Officer vice)
        {
            if (commander == null)
                return vice?.CombatRelevantSum ?? 0;
            if (vice == null)
                return commander.CombatRelevantSum;

            int atk = (commander.EffectiveAttack * 2 + vice.EffectiveAttack) / 3;
            int intel = (commander.EffectiveIntelligence * 2 + vice.EffectiveIntelligence) / 3;
            int lead = (commander.EffectiveLeadership * 2 + vice.EffectiveLeadership) / 3;
            return atk + intel + lead;
        }

        static byte PickPerform(byte perform, byte baseStat) =>
            perform > 0 ? perform : baseStat;



        /// <summary>旗標集合：性別、傷勢、存活、登場狀態等。</summary>
        public OfficerFlag OfficerFlag => officerFlag;

        /// <summary>
        /// 所屬勢力 id；0＝在野。
        /// 約定：勢力 id＝執掌該勢力的武將 defId（例：劉備軍＝1）。
        /// </summary>
        public short Belong => belong;

        /// <summary>
        /// 暫定與 <see cref="Belong"/> 相同（勢力領袖武將 id）。
        /// 日後可改為「所屬兵團主將 defId」。
        /// </summary>
        public int LegionLeaderId => legionLeaderId;

        /// <summary>
        /// 是否出戰：任職於 <see cref="Units.Combat"/>／<see cref="Units.Transport"/> 為 true；
        /// 僅在兵團（<see cref="Units.Legion"/>）或城內待命為 false。
        /// </summary>
        public bool IsDeployed => isDeployed;

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

        /// <summary>死亡年份；0 表示未定。</summary>
        public short DeathYear => deathYear;

        /// <summary>享年（死亡年－出生年）；資料不全時為 0。</summary>
        public short AgeAtDeath =>
            deathYear > birthYear && birthYear > 0 ? (short)(deathYear - birthYear) : (short)0;

        /// <summary>相性基準值（與他將互動用）。</summary>
        public byte Compatibility => compatibility;

        /// <summary>各兵種適性等級集合。</summary>
        public OfficerTroopAptitude TroopAptitude => troopAptitude;

        /// <summary>查詢指定兵種的適性等級。</summary>
        public TroopAptitudeGrade GetTroopAptitude(TroopType troop) => troopAptitude.Get(troop);



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
        public abstract int RollRandom(int minInclusive, int maxInclusive, double increament);



        /// <summary>觸發發揮值重算（屬性變更後呼叫）。</summary>
        protected void RefreshPerformance() => CalculatePerformance();

    }

}


