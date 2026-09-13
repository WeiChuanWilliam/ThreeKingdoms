using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Data.Terrain;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>
    /// 地圖上所有部隊的抽象基類。
    /// 三種部隊：<see cref="Legion"/>（兵團）、<see cref="Combat"/>（戰鬥隊）、<see cref="Transport"/>（運輸隊）。
    /// </summary>
    public abstract class Unit
    {
        readonly List<Officer> viceOfficers = new();

        /// <summary>部隊顯示名稱。</summary>
        public string UnitName { get; }

        /// <summary>所屬勢力 id。</summary>
        public int Belonged { get; }

        UnitFlags unitFlags = new();

        /// <summary>部隊計略／異常狀態剩餘天數（混亂、偽報、止步、斷糧；0＝無）。</summary>
        public UnitFlags Status => unitFlags;

        UnitHazardSnapshot terrainHazard;

        /// <summary>腳下格環境傷害快照（火／陷阱等；由 Location 同步）。</summary>
        public UnitHazardSnapshot TerrainHazard => terrainHazard;

        /// <summary>駐紮或關聯的建築據點。</summary>
        public AbstractBuilding Building { get; private set; }

        /// <summary>腳下格火焰強度（由地形／格狀態同步）。</summary>
        public short FireEffect { get; private set; }

        /// <summary>腳下格造成的士氣懲罰。</summary>
        public short MoralePenalty { get; private set; }

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

        /// <summary>可戰士兵數（不含傷兵；戰鬥力／交戰用人數直接用這個）。</summary>
        public int Soldiers { get; private set; }

        /// <summary>傷兵數（獨立計數；仍屬部隊，但不參與戰鬥計算）。</summary>
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

        /// <summary>是否已全滅（可戰過低，或士氣／體力歸零）。無額外旗標，純看狀態。</summary>
        public bool IsAnnihilated =>
            Soldiers < Data.Units.UnitConfigUtil.MinSoldiers
            || Morale <= 0
            || Stamina <= 0;

        /// <summary>部隊類型（兵團／戰鬥／運輸）。</summary>
        public abstract UnitKind Kind { get; }

        /// <summary>耗糧倍率（相對 <see cref="BaseFoodByHeadCount"/>；暫定兵糧無限時子類可忽略）。</summary>
        public abstract float FoodConsumptionFactor { get; }

        /// <summary>火焰傷害倍率（相對基礎火傷）。</summary>
        public abstract float FireDamageFactor { get; }

        /// <summary>計算本部隊每日兵糧消耗量。暫定兵糧無限 → 子類回傳 0。</summary>
        public abstract int CalculateFoodConsumption();

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
            terrainHazard = UnitHazardSnapshot.None;
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
        public virtual void SetCommander(Officer unitCopy)
        {
            if (Commander != null && !ReferenceEquals(Commander, unitCopy))
                SyncOfficerDeployed(Commander, assigned: false);
            Commander = unitCopy;
            SyncOfficerDeployed(unitCopy, assigned: true);
        }

        /// <summary>從武將池依 defId 設定主將。</summary>
        public virtual void SetCommanderFromPool(int officerDefId) =>
            SetCommander(officerDefId > 0 ? OfficerPool.Get(officerDefId) : null);

        /// <summary>
        /// 戰鬥／運輸部隊任職 → <see cref="Officer.IsDeployed"/>＝true；
        /// 兵團任職不算出戰；卸任 → false。
        /// 戰鬥部隊另綁 <see cref="Officer.SetAssignedCombat"/> 以便傷病回寫統合五維。
        /// </summary>
        void SyncOfficerDeployed(Officer officer, bool assigned)
        {
            if (officer == null)
                return;
            if (Kind == UnitKind.Combat || Kind == UnitKind.Transport)
                officer.SetDeployed(assigned);
            if (this is Combat combat)
                officer.SetAssignedCombat(assigned ? combat : null);
            // Legion：指派／卸任皆不改 IsDeployed（不算出戰）
        }

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

        /// <summary>
        /// 設定可戰士兵與傷兵（兩者獨立；傷兵不從士兵扣）。
        /// 可戰過低時自註冊表移除。
        /// </summary>
        public virtual void SetManpower(int soldiers, int woundedCount = 0)
        {
            Soldiers = System.Math.Max(0, soldiers);
            Wounded = System.Math.Max(0, woundedCount);
            if (Soldiers < Data.Units.UnitConfigUtil.MinSoldiers)
                TryRemoveIfAnnihilated();
        }

        /// <summary>設定士氣並限制在 0～100；士氣歸零則自註冊表移除。</summary>
        public virtual void SetMorale(short value)
        {
            Morale = NumericUtil.ClampToTarget(value, (short)0, (short)100);
            if (Morale <= 0)
                TryRemoveIfAnnihilated();
        }

        /// <summary>設定體力並限制在 0～100；體力歸零則自註冊表移除。</summary>
        public virtual void SetStamina(short value)
        {
            Stamina = NumericUtil.ClampToTarget(value, (short)0, (short)100);
            if (Stamina <= 0)
                TryRemoveIfAnnihilated();
        }

        /// <summary>
        /// 若 <see cref="IsAnnihilated"/> 則從 <see cref="UnitRegistry"/> 移除；否則無事。
        /// </summary>
        public void TryRemoveIfAnnihilated()
        {
            if (!IsAnnihilated)
                return;
            UnitRegistry.Unregister(this);
        }

        /// <summary>由 <see cref="UnitLocationBinding"/> 寫入腳下格環境傷害。</summary>
        internal void ApplyTerrainHazard(in UnitHazardSnapshot hazard) => terrainHazard = hazard;

        /// <summary>著火格火焰傷害等級（讀 <see cref="TerrainHazard"/>）。</summary>
        public TerrainHazardLevel FlameDamage => terrainHazard.FlameLevel;

        /// <summary>陷阱格傷害等級（讀 <see cref="TerrainHazard"/>）。</summary>
        public TerrainHazardLevel TrapDamage => terrainHazard.TrapLevel;

        /// <summary>火焰與陷阱環境傷害取較高者。</summary>
        public TerrainHazardLevel EnvironmentalDamage => terrainHazard.EnvironmentalLevel;

        /// <summary>設定是否駐紮（駐紮中不可移動）。</summary>
        public void SetGarrison(bool garrison) => IsGarrison = garrison;

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

        /// <summary>依人頭計算基礎耗糧（可戰＋傷兵；每 100 人至少 1）。</summary>
        protected int BaseFoodByHeadCount() =>
            System.Math.Max(1, (Soldiers + Wounded) / 100);

    }

    /// <summary>地圖部隊類型：兵團、戰鬥隊、運輸隊。</summary>
    public enum UnitKind
    {
        Legion,
        Combat,
        Transport
    }
}
