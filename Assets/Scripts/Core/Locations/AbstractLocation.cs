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

        public ushort Row => row;
        public ushort Column => column;
        public LocationFlags LocationFlags => locationFlags;
        public AbstractBuilding Building => building;
        public Unit FightingUnit => fightingUnit;
        public GroupUnit GroupUnit => groupUnit;
        public AbstractTerrain Terrain => terrain;
        public short Belonged => belonged;

        /// <summary>與 Spike 六角格對齊：column=q，row=r。</summary>
        public HexCoord Hex => new(column, row);

        protected AbstractLocation(ushort row, ushort column, AbstractTerrain terrainRef)
        {
            this.row = row;
            this.column = column;
            terrain = terrainRef;
        }

        public void SetBelonged(short factionId) => belonged = factionId;
        public void SetTerrain(AbstractTerrain terrainRef) => terrain = terrainRef;
        public void SetBuilding(AbstractBuilding b) => building = b;
        /// <summary>目前佔據此格的部隊（進入時設定，離開時清空）。</summary>
        public Unit OccupyingUnit => fightingUnit;

        public void SetFightingUnit(Unit unit) => fightingUnit = unit;
        public void SetGroupUnit(GroupUnit group) => groupUnit = group;
        public void SetFlags(LocationFlags flags) => locationFlags = flags;

        public abstract bool CountRange(short inputRow, short inputColumn, short range);
        public abstract bool UnitMoveIn();
        public abstract void UnitMoved();
        public abstract bool UnitJoinIn();
        public abstract void UnitJoined();
        public abstract bool SetOnFire();
        public abstract void SetOnDefence();
        public abstract void CancelDefence();
        public abstract void CountFire(int roll0To99);
        public abstract bool FireExpansion();
        public abstract void UnitMoveOut();
        public abstract void UnitAttack(Unit inputUnit);
    }
}
