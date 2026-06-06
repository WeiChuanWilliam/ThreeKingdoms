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

        public MapLocation(ushort row, ushort column, AbstractTerrain terrainRef)
            : base(row, column, terrainRef) { }

        public MapLocation(HexCoord hex, AbstractTerrain terrainRef)
            : base((ushort)hex.R, (ushort)hex.Q, terrainRef) { }

        public override bool CountRange(short inputRow, short inputColumn, short range)
        {
            int dq = System.Math.Abs(inputRow - row);
            int dr = System.Math.Abs(inputColumn - column);
            return dq + dr <= range;
        }

        public override bool UnitMoveIn()
        {
            if (!locationFlags.Passable)
                return false;
            locationFlags.OnUnit = true;
            return true;
        }

        public override void UnitMoved() { }

        public override bool UnitJoinIn() => locationFlags.Joinable;

        public override void UnitJoined() { }

        internal void AttachGrid(LocationGrid grid) => _grid = grid;

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

        public bool SetOnTrap()
        {
            locationFlags.OnTrap = true;
            return true;
        }

        public void ClearTrap() => locationFlags.OnTrap = false;

        public override void SetOnDefence() => locationFlags.OnDefence = true;

        public override void CancelDefence() => locationFlags.OnDefence = false;

        public void Extinguish()
        {
            if (!locationFlags.OnFire)
                return;
            locationFlags.OnFire = false;
            _grid?.UnregisterBurning(Hex);
        }

        public override void CountFire(int roll0To99)
        {
            LocationFireRules.TickDailyBurnAtSunrise(this, roll0To99);
        }

        public override bool FireExpansion()
        {
            if (!locationFlags.OnFire)
                return false;
            LocationFireRules.TrySpreadFireToAdjacentTiles(this, grid: null, rng: null);
            return locationFlags.OnFire;
        }

        public bool FireExpansion(LocationGrid grid, Random rng)
        {
            if (!locationFlags.OnFire)
                return false;
            LocationFireRules.TrySpreadFireToAdjacentTiles(this, grid, rng);
            return locationFlags.OnFire;
        }

        public override void UnitMoveOut()
        {
            locationFlags.OnUnit = false;
            if (fightingUnit != null)
                fightingUnit = null;
        }

        public override void UnitAttack(Unit inputUnit)
        {
            fightingUnit = inputUnit;
        }
    }
}
