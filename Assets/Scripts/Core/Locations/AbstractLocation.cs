using ThreeKindoms.Core;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Locations;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>
    /// 對應 C++ Location::AbstractLocation。
    /// 地圖上一格（row/column）的狀態：地形、建築、駐軍、火計等。
    /// </summary>
    public abstract class AbstractLocation
    {
        protected ushort row;
        protected ushort column;
        protected LocationFlags locationFlags = LocationFlags.DefaultPassable;
        protected AbstractBuilding building;
        protected Unit fightingUnit;
        protected GroupUnit groupUnit;
        protected AbstractTerrain terrain;
        protected short belonged;

        /// <summary>格座標列（對應 axial r）。</summary>
        public ushort Row => row;

        /// <summary>格座標行（對應 axial q）。</summary>
        public ushort Column => column;

        /// <summary>通行、著火、駐軍等狀態旗標。</summary>
        public LocationFlags LocationFlags => locationFlags;

        /// <summary>格上建築實例（可為 null）。</summary>
        public AbstractBuilding Building => building;

        /// <summary>格上戰鬥或駐留部隊（與 <see cref="OccupyingUnit"/> 同源）。</summary>
        public Unit FightingUnit => fightingUnit;

        /// <summary>格上集結／編組單位（可為 null）。</summary>
        public GroupUnit GroupUnit => groupUnit;

        /// <summary>格地形定義。</summary>
        public AbstractTerrain Terrain => terrain;

        /// <summary>格所屬勢力 id。</summary>
        public short Belonged => belonged;

        /// <summary>與 Spike 六角格對齊：column=q，row=r。</summary>
        public HexCoord Hex => new(column, row);

        /// <summary>以列、行與地形建立 Location 基底實例。</summary>
        protected AbstractLocation(ushort row, ushort column, AbstractTerrain terrainRef)
        {
            this.row = row;
            this.column = column;
            terrain = terrainRef;
        }

        /// <summary>設定格所屬勢力。</summary>
        public void SetBelonged(short factionId) => belonged = factionId;

        /// <summary>更換格地形定義。</summary>
        public void SetTerrain(AbstractTerrain terrainRef) => terrain = terrainRef;

        /// <summary>設定或清除格上建築。</summary>
        public void SetBuilding(AbstractBuilding b) => building = b;
        /// <summary>目前佔據此格的部隊（進入時設定，離開時清空）。</summary>
        public Unit OccupyingUnit => fightingUnit;

        /// <summary>登記部隊進入此格。</summary>
        public void SetFightingUnit(Unit unit) => fightingUnit = unit;

        /// <summary>設定格上集結單位。</summary>
        public void SetGroupUnit(GroupUnit group) => groupUnit = group;

        /// <summary>覆寫格狀態旗標。</summary>
        public void SetFlags(LocationFlags flags) => locationFlags = flags;

        /// <summary>判斷目標格是否在曼哈頓距離射程內。</summary>
        public abstract bool CountRange(short inputRow, short inputColumn, short range);

        /// <summary>部隊嘗試移入：檢查可通行並標記有駐軍。</summary>
        public abstract bool UnitMoveIn();

        /// <summary>部隊移入完成後的後續處理。</summary>
        public abstract void UnitMoved();

        /// <summary>判斷是否允許部隊合流進入。</summary>
        public abstract bool UnitJoinIn();

        /// <summary>部隊合流完成後的後續處理。</summary>
        public abstract void UnitJoined();

        /// <summary>嘗試點燃此格（需地形可燃）。</summary>
        public abstract bool SetOnFire();

        /// <summary>進入防禦姿態。</summary>
        public abstract void SetOnDefence();

        /// <summary>解除防禦姿態。</summary>
        public abstract void CancelDefence();

        /// <summary>每日續燃判定（依亂數 roll）。</summary>
        public abstract void CountFire(int roll0To99);

        /// <summary>嘗試向鄰格蔓延火勢。</summary>
        public abstract bool FireExpansion();

        /// <summary>部隊離開此格並清除駐軍標記。</summary>
        public abstract void UnitMoveOut();

        /// <summary>登記對此格的攻擊部隊。</summary>
        public abstract void UnitAttack(Unit inputUnit);
    }
}
