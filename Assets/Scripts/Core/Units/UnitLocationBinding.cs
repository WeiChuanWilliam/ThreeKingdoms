using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Data.Locations;
using ThreeKindoms.Data.Units;

namespace ThreeKindoms.Core.Units
{
    /// <summary>部隊（<see cref="Unit"/>）與地圖格的連動。</summary>
    public sealed class UnitLocationBinding : IMapUnitMovement
    {
        Unit _unit;
        LocationGrid _locationGrid;
        MapLocation _currentLocation;

        public Unit Unit => _unit;
        public HexCoord Position { get; set; }
        public int MovementPointsLeft { get; set; }
        public MapLocation CurrentLocation => _currentLocation;

        public bool IsOnFire => _currentLocation != null && _currentLocation.LocationFlags.OnFire;
        public bool IsOnTrap => _currentLocation != null && _currentLocation.LocationFlags.OnTrap;
        public bool IsOnDefence => _currentLocation != null && _currentLocation.LocationFlags.OnDefence;
        public AbstractBuilding StationedBuilding => _unit.Building ?? _currentLocation?.Building;
        public AbstractTerrain CurrentTerrain => _currentLocation?.Terrain;

        public UnitLocationBinding(Unit unit)
        {
            _unit = unit;
            MovementPointsLeft = MovementRules.DefaultDailyPoints;
        }

        public void BindToWorld(LocationGrid locationGrid, HexCoord startHex, AbstractTerrain terrainAtStart)
        {
            _locationGrid = locationGrid;
            EnterHex(startHex, terrainAtStart);
            UnitRegistry.Register(_unit);
        }

        /// <summary>駐軍轉換等情境：複製另一部隊的地圖綁定狀態。</summary>
        internal void InheritWorldStateFrom(UnitLocationBinding source)
        {
            if (source == null)
                return;
            _locationGrid = source._locationGrid;
            _currentLocation = source._currentLocation;
            Position = source.Position;
            MovementPointsLeft = source.MovementPointsLeft;
        }

        public void RefillMovementAtSunrise() =>
            MovementPointsLeft = MovementRules.DefaultDailyPoints;

        /// <summary>從目前腳下格／地形同步著火與環境傷害（不含續燃擲骰與灼燒結算）。</summary>
        public void SyncEnvironmentFromLocation() => ApplyFireAndMoraleFromLocation();

        /// <summary>相容舊呼叫；等同 <see cref="SyncEnvironmentFromLocation"/>。</summary>
        public void SyncFireFromLocation() => SyncEnvironmentFromLocation();

        public void LeaveCurrentHex()
        {
            if (_currentLocation == null) return;
            _currentLocation.UnitMoveOut();
            if (_currentLocation.FightingUnit == _unit)
                _currentLocation.SetFightingUnit(null);
            _currentLocation = null;
        }

        public bool EnterHex(HexCoord hex, AbstractTerrain terrainIfCreate = null)
        {
            if (_locationGrid == null) return false;
            if (_unit is Garrison)
                return false;

            LeaveCurrentHex();

            MapLocation loc = terrainIfCreate != null
                ? _locationGrid.GetOrCreate(hex, terrainIfCreate)
                : _locationGrid.TryGet(hex, out var existing) ? existing : null;

            if (loc == null || !loc.UnitMoveIn()) return false;

            Position = hex;
            _currentLocation = loc;
            loc.SetFightingUnit(_unit);
            SyncFromLocation(loc);
            return true;
        }

        public bool MoveAlongPath(HexGrid pathGrid, System.Collections.Generic.IReadOnlyList<HexCoord> path)
        {
            if (_unit is Garrison)
                return false;
            if (path == null || path.Count <= 1) return false;
            float speed = UnitMarchSpeed.GetMarchSpeedMultiplier(_unit);
            if (speed <= 0f)
                return false;

            for (int i = 1; i < path.Count; i++)
            {
                var step = path[i];
                int cost = pathGrid.GetEnterCost(step);
                if (speed < 1f)
                    cost = System.Math.Max(1, (int)(cost / speed));
                if (MovementPointsLeft < cost) break;

                pathGrid.TryGet(step, out var cell);
                var terrain = TerrainDefinition.FromTerrainType(cell.Terrain);
                if (!EnterHex(step, terrain)) break;

                MovementPointsLeft -= cost;
                _currentLocation?.UnitMoved();
            }

            ApplyFireAndMoraleFromLocation();
            return true;
        }

        void SyncFromLocation(MapLocation loc)
        {
            if (loc.Building != null)
                _unit.SetBuilding(loc.Building);

            if (_unit is Combat combat)
                GarrisonConversion.TryAutoStation(combat, loc, ref _unit);

            ApplyFireAndMoraleFromLocation();
        }

        /// <summary>駐軍離開據點時呼叫，還原野戰部隊並可再次移動。</summary>
        public Combat DepartGarrison()
        {
            if (_unit is not Garrison garrison)
                return null;
            return GarrisonConversion.Depart(garrison, _currentLocation, ref _unit);
        }

        void ApplyFireAndMoraleFromLocation()
        {
            if (_currentLocation == null)
            {
                _unit.SetFireEffect(0);
                _unit.SetFlameDamage(HazardDamageLevel.None);
                _unit.SetTrapDamage(HazardDamageLevel.None);
                return;
            }

            ApplyHazardFromTile(
                _currentLocation.LocationFlags.OnFire,
                _currentLocation.LocationFlags.OnTrap,
                _currentLocation.Terrain?.FireEffect ?? 0);

            _unit.SetMoralePenalty((short)(_currentLocation.Terrain?.MoralePenalty ?? 0));
        }

        /// <summary>火焰／陷阱格共用傷害等級（0～3）；分開寫入 UnitFlags。</summary>
        void ApplyHazardFromTile(bool onFire, bool onTrap, int terrainFireEffect)
        {
            HazardDamageLevel level = ResolveHazardLevel(onFire || onTrap, terrainFireEffect);
            _unit.SetFlameDamage(onFire ? level : HazardDamageLevel.None);
            _unit.SetTrapDamage(onTrap ? level : HazardDamageLevel.None);
            _unit.SetFireEffect(onFire
                ? (short)(terrainFireEffect > 0 ? terrainFireEffect : 1)
                : (short)0);
        }

        static HazardDamageLevel ResolveHazardLevel(bool onHazardTile, int terrainFireEffect)
        {
            if (!onHazardTile) return HazardDamageLevel.None;
            if (terrainFireEffect >= 3) return HazardDamageLevel.Serious;
            if (terrainFireEffect >= 2) return HazardDamageLevel.Medium;
            return HazardDamageLevel.Slight;
        }
    }
}
