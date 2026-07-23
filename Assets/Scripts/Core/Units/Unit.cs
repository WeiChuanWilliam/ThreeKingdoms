using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 地圖上所有部隊的抽象基類。
    /// 設計中的四種部隊：<see cref="Legion"/>（兵團）、<see cref="Combat"/>（戰鬥隊）、<see cref="Transport"/>（運輸隊），以及本類本身。
    /// </summary>
    public abstract class Unit
    {
        readonly List<Officer> viceOfficers = new();
        bool annihilated;

        /// <summary>部隊顯示名稱。</summary>
        public string UnitName { get; }

        /// <summary>所屬勢力 id。</summary>
        public int Belonged { get; }

        UnitFlags unitFlags;

        /// <summary>部隊狀態旗標（可達、火焰／陷阱傷害等）。</summary>
        public ref UnitFlags UnitFlags => ref unitFlags;

        /// <summary>駐紮或關聯的建築據點。</summary>
        public AbstractBuilding Building { get; private set; }

        /// <summary>腳下格火焰強度（由地形／格狀態同步）。</summary>
        public short FireEffect { get; private set; }

        /// <summary>腳下格造成的士氣懲罰。</summary>
        public short MoralePenalty { get; private set; }

        /// <summary>部隊身上的效果對照表（戰法、計略等，預留）。</summary>
        public UnitEffectMaps EffectMaps { get; } = new();

        /// <summary>腳下格著火即視為著火。</summary>
        public bool IsOnFire => Location?.IsOnFire == true;

        /// <summary>士氣（0～100）。</summary>
        public short Morale { get; private set; }

        /// <summary>體力（0～100）。</summary>
        public short Stamina { get; private set; }

        /// <summary>攜帶金錢。</summary>
        public int Money { get; private set; }

        /// <summary>主將（直接引用劇本 <see cref="OfficerPool"/> 內武將）。</summary>
        public Officer Commander { get; private set; }

        /// <summary>士兵總數（含傷兵）。</summary>
        public int Soldiers { get; private set; }

        /// <summary>傷兵數。</summary>
        public int Wounded { get; private set; }

        /// <summary>部隊與地圖格的綁定（座標、行動力、進出格）。</summary>
        public UnitLocationBinding Location { get; private set; }

        /// <summary>目前地圖座標（未進入地圖時為預設值）。</summary>
        public HexCoord CurrentHex => Location.Position;

        /// <summary>腳下格；未綁定地圖時為 null。</summary>
        public MapLocation CurrentMapLocation => Location.CurrentLocation;

        /// <summary>是否已進入地圖並佔據一格。</summary>
        public bool IsOnMap => Location?.CurrentLocation != null;

        /// <summary>腳下地形；未進入地圖時為 null。</summary>
        public AbstractTerrain CurrentTerrain => Location?.CurrentTerrain;

        /// <summary>副將列表（戰鬥部隊至多一位，其他類型依設計）。</summary>
        public IReadOnlyList<Officer> ViceOfficers => viceOfficers;

        /// <summary>可作戰的有效兵力（總兵 − 傷兵）。</summary>
        public int EffectiveCombatStrength => UnitManpower.EffectiveCombatStrength(Soldiers, Wounded);

        /// <summary>是否已全滅（兵力歸零）。</summary>
        public bool IsAnnihilated => annihilated || UnitManpower.IsAnnihilated(Soldiers);

        /// <summary>部隊類型（兵團／戰鬥／運輸）。</summary>
        public abstract UnitKind Kind { get; }

        /// <summary>耗糧倍率（相對 <see cref="BaseFoodByHeadCount"/>；暫定兵糧無限時子類可忽略）。</summary>
        public abstract float FoodConsumptionFactor { get; }

        /// <summary>火焰傷害倍率（相對基礎火傷）。</summary>
        public abstract float FireDamageFactor { get; }

        /// <summary>計算本部隊每日兵糧消耗量。暫定兵糧無限 → 子類回傳 0。</summary>
        public abstract int CalculateFoodConsumption();

        /// <summary>
        /// 當前戰鬥力評分：依 <see cref="IsGarrison"/> 分流至駐紮／非駐紮抽象方法。
        /// 全滅為 0。
        /// </summary>
        public int CalculateCombatPower()
        {
            if (IsAnnihilated)
                return 0;
            return IsGarrison
                ? CalculateGarrisonCombatPower()
                : CalculateNonGarrisonCombatPower();
        }

        /// <summary>非駐紮（野戰）戰鬥力；由子類覆寫。</summary>
        protected abstract int CalculateNonGarrisonCombatPower();

        /// <summary>駐紮（garrison）戰鬥力；由子類覆寫。</summary>
        protected abstract int CalculateGarrisonCombatPower();

        /// <summary>駐紮於城池／要塞等據點時為 true；駐紮中不可移動且通常不耗糧。</summary>
        public bool IsGarrison { get; private set; }

        /// <summary>向後勤申請扣除本日兵糧；暫定兵糧無限 → 恒 true、不扣糧。</summary>
        public virtual bool TryConsumeDailyFood() => true;

        /// <summary>
        /// 是否可在野戰中正常作戰。
        /// 兵團（<see cref="Legion"/>）須 <see cref="IsGarrison"/> 後才可；戰鬥隊預設可野戰。
        /// </summary>
        public virtual bool CanFightInField => true;

        /// <summary>以名稱與勢力建立部隊，並建立地圖綁定物件。</summary>
        protected Unit(string name, int factionBelonged)
        {
            UnitName = name ?? "";
            Belonged = factionBelonged;
            unitFlags = new UnitFlags();
            Location = new UnitLocationBinding(this);
        }

        /// <summary>是否已指派該 defId 的副將。</summary>
        public bool ContainsViceOfficer(int officerDefId) =>
            officerDefId > 0 && FindVice(officerDefId) != null;

        /// <summary>加入副將（須為劇本池內有效武將）。</summary>
        public virtual bool AddViceOfficer(Officer unitCopy)
        {
            if (unitCopy == null || unitCopy.RuntimeId <= 0) return false;
            if (FindVice(unitCopy.RuntimeId) != null) return false;
            viceOfficers.Add(unitCopy);
            SyncOfficerDeployed(unitCopy, assigned: true);
            return true;
        }

        /// <summary>從武將池依 defId 加入副將。</summary>
        public bool AddViceOfficerFromPool(int officerDefId)
        {
            Officer officer = OfficerPool.Get(officerDefId);
            return officer != null && AddViceOfficer(officer);
        }

        /// <summary>移除指定 defId 的副將。</summary>
        public bool RemoveViceOfficer(int officerDefId)
        {
            Officer found = FindVice(officerDefId);
            if (found == null || !viceOfficers.Remove(found))
                return false;
            SyncOfficerDeployed(found, assigned: false);
            return true;
        }

        /// <summary>清空所有副將（存檔還原等內部用）。</summary>
        internal void ClearViceOfficers()
        {
            foreach (Officer v in viceOfficers)
                SyncOfficerDeployed(v, assigned: false);
            viceOfficers.Clear();
        }

        /// <summary>設定主將；會更新出戰狀態（Combat／Transport＝出戰）。</summary>
        public void SetCommander(Officer unitCopy)
        {
            if (Commander != null && !ReferenceEquals(Commander, unitCopy))
                SyncOfficerDeployed(Commander, assigned: false);
            Commander = unitCopy;
            SyncOfficerDeployed(unitCopy, assigned: true);
        }

        /// <summary>從武將池依 defId 設定主將。</summary>
        public void SetCommanderFromPool(int officerDefId) =>
            SetCommander(officerDefId > 0 ? OfficerPool.Get(officerDefId) : null);

        /// <summary>
        /// 戰鬥／運輸部隊任職 → <see cref="Officer.IsDeployed"/>＝true；
        /// 兵團任職不算出戰；卸任 → false。
        /// </summary>
        void SyncOfficerDeployed(Officer officer, bool assigned)
        {
            if (officer == null)
                return;
            if (Kind == UnitKind.Combat || Kind == UnitKind.Transport)
                officer.SetDeployed(assigned);
            // Legion：指派／卸任皆不改 IsDeployed（不算出戰）
        }

        /// <summary>設定士氣並限制在 0～100。</summary>
        public void SetMorale(short value) => Morale = NumericUtil.ClampToTarget(value, (short)0, (short)100);

        /// <summary>設定體力並限制在 0～100。</summary>
        public void SetStamina(short value) => Stamina = NumericUtil.ClampToTarget(value, (short)0, (short)100);

        /// <summary>設定攜帶金錢（負值視為 0）。</summary>
        public void SetMoney(int value) => Money = value < 0 ? 0 : value;

        /// <summary>增減士氣。</summary>
        public void ChangeMorale(short delta) => SetMorale((short)(Morale + delta));

        /// <summary>增減體力。</summary>
        public void ChangeStamina(short delta) => SetStamina((short)(Stamina + delta));

        /// <summary>設定駐紮或關聯建築。</summary>
        public void SetBuilding(AbstractBuilding b) => Building = b;

        /// <summary>設定腳下格火焰效果強度。</summary>
        public void SetFireEffect(short value) => FireEffect = value;

        /// <summary>設定腳下格士氣懲罰。</summary>
        public void SetMoralePenalty(short value) => MoralePenalty = value;

        /// <summary>設定士兵與傷兵；全滅時自動從註冊表移除。</summary>
        public virtual void SetManpower(int totalSoldiers, int woundedCount = 0)
        {
            Soldiers = System.Math.Max(0, totalSoldiers);
            Wounded = NumericUtil.ClampToTarget(woundedCount, 0, Soldiers);
            bool wasAnnihilated = annihilated;
            annihilated = UnitManpower.IsAnnihilated(Soldiers);
            if (!wasAnnihilated && annihilated)
                UnitRegistry.Unregister(this);
        }

        /// <summary>設定是否駐紮（駐紮中不可移動）。</summary>
        public void SetGarrison(bool garrison) => IsGarrison = garrison;

        /// <summary>設定尋路可達旗標。</summary>
        public void SetReachableFlag(bool reachable) => UnitFlags.Reachable = reachable;

        /// <summary>設定火焰環境傷害等級。</summary>
        public void SetFlameDamage(HazardDamageLevel level) =>
            UnitFlags.FlameDamage = UnitFlags.ClampHazard(level);

        /// <summary>設定陷阱環境傷害等級。</summary>
        public void SetTrapDamage(HazardDamageLevel level) =>
            UnitFlags.TrapDamage = UnitFlags.ClampHazard(level);

        /// <summary>火焰環境傷害等級。</summary>
        public HazardDamageLevel FlameDamage => UnitFlags.FlameDamage;

        /// <summary>陷阱環境傷害等級。</summary>
        public HazardDamageLevel TrapDamage => UnitFlags.TrapDamage;

        /// <summary>火焰與陷阱取較重者（格傷害規則相同時用）。</summary>
        public HazardDamageLevel EnvironmentalDamage =>
            (HazardDamageLevel)System.Math.Max((byte)FlameDamage, (byte)TrapDamage);

        /// <summary>該武將 defId 是否為本隊主將或副將。</summary>
        public bool IsOfficerAssigned(int officerDefId) =>
            officerDefId > 0 &&
            (Commander?.RuntimeId == officerDefId || FindVice(officerDefId) != null);

        /// <summary>該武將是否為本隊主將或副將。</summary>
        public bool IsOfficerAssigned(Officer officer) =>
            officer != null && IsOfficerAssigned(officer.RuntimeId);

        /// <summary>依 defId 查找副將。</summary>
        Officer FindVice(int officerDefId)
        {
            foreach (Officer v in viceOfficers)
            {
                if (v.RuntimeId == officerDefId) return v;
            }
            return null;
        }

        /// <summary>依兵力計算基礎耗糧（每 100 兵至少 1）。</summary>
        protected int BaseFoodByHeadCount() =>
            System.Math.Max(1, Soldiers / 100);

    }

    /// <summary>地圖部隊類型：兵團、戰鬥隊、運輸隊。</summary>
    public enum UnitKind
    {
        Legion,
        Combat,
        Transport
    }
}
