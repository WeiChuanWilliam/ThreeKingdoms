using System;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Locations;

namespace ThreeKindoms.Core.Locations
{
    /// <summary>可玩的地圖格實作（Spike / 單機預設）。</summary>
    public sealed class MapLocation : AbstractLocation
    {
        LocationGrid _grid;

        /// <summary>以列、行與地形建立地圖格。</summary>
        public MapLocation(ushort row, ushort column, AbstractTerrain terrainRef)
            : base(row, column, terrainRef) { }

        /// <summary>以六角座標與地形建立地圖格。</summary>
        public MapLocation(HexCoord hex, AbstractTerrain terrainRef)
            : base((ushort)hex.R, (ushort)hex.Q, terrainRef) { }

        /// <inheritdoc/>
        public override bool CountRange(short inputRow, short inputColumn, short range)
        {
            int dq = System.Math.Abs(inputRow - row);
            int dr = System.Math.Abs(inputColumn - column);
            return dq + dr <= range;
        }

        /// <inheritdoc/>
        public override bool UnitMoveIn()
        {
            if (!locationFlags.Passable)
                return false;
            locationFlags.OnUnit = true;
            return true;
        }

        /// <inheritdoc/>
        public override void UnitMoved() { }

        /// <inheritdoc/>
        public override bool UnitJoinIn() => locationFlags.Joinable;

        /// <inheritdoc/>
        public override void UnitJoined() { }

        /// <summary>綁定所屬 <see cref="LocationGrid"/> 以維護著火索引。</summary>
        internal void AttachGrid(LocationGrid grid) => _grid = grid;

        /// <inheritdoc/>
        public override bool SetOnFire()
        {
            if (terrain == null || !terrain.TerrainFlags.Fireable)
                return false;
            if (locationFlags.OnFire)
                return true;
            locationFlags.OnFire = true;
            _grid?.RegisterBurning(Hex);
            return true;
        }

        /// <summary>在此格設置陷阱。</summary>
        public bool SetOnTrap()
        {
            locationFlags.OnTrap = true;
            return true;
        }

        /// <summary>清除此格陷阱狀態。</summary>
        public void ClearTrap() => locationFlags.OnTrap = false;

        /// <inheritdoc/>
        public override void SetOnDefence() => locationFlags.OnDefence = true;

        /// <inheritdoc/>
        public override void CancelDefence() => locationFlags.OnDefence = false;

        /// <summary>熄滅此格火焰並更新著火索引。</summary>
        public void Extinguish()
        {
            if (!locationFlags.OnFire)
                return;
            locationFlags.OnFire = false;
            _grid?.UnregisterBurning(Hex);
        }

        /// <inheritdoc/>
        public override void CountFire(int roll0To99)
        {
            LocationFireRules.TickDailyBurnAtSunrise(this, roll0To99);
        }

        /// <inheritdoc/>
        public override bool FireExpansion()
        {
            if (!locationFlags.OnFire)
                return false;
            LocationFireRules.TrySpreadFireToAdjacentTiles(this, grid: null, rng: null);
            return locationFlags.OnFire;
        }

        /// <summary>以網格與亂數嘗試向鄰格蔓延火勢。</summary>
        public bool FireExpansion(LocationGrid grid, Random rng)
        {
            if (!locationFlags.OnFire)
                return false;
            LocationFireRules.TrySpreadFireToAdjacentTiles(this, grid, rng);
            return locationFlags.OnFire;
        }

        /// <inheritdoc/>
        public override void UnitMoveOut()
        {
            locationFlags.OnUnit = false;
            if (fightingUnit != null)
                fightingUnit = null;
        }

        /// <inheritdoc/>
        public override void UnitAttack(Unit inputUnit)
        {
            fightingUnit = inputUnit;
        }
    }
}
