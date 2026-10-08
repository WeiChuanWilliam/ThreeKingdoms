using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;

namespace ThreeKindoms.Core.Units
{
    /// <summary>部隊（<see cref="Unit"/>）與地圖格的連動。</summary>
    public sealed class UnitLocationBinding
    {
        Unit _unit;
        LocationGrid _locationGrid;
        MapLocation _currentLocation;

        /// <summary>綁定的部隊本體。</summary>
        public Unit Unit => _unit;

        /// <summary>目前所在六角格座標。</summary>
        public HexCoord Position { get; set; }

        /// <summary>目前佔據的地圖格實例。</summary>
        public MapLocation CurrentLocation => _currentLocation;

        /// <summary>腳下格是否著火。</summary>
        public bool IsOnFire => _currentLocation != null && _currentLocation.LocationFlags.OnFire;

        /// <summary>腳下格是否有陷阱。</summary>
        public bool IsOnTrap => _currentLocation != null && _currentLocation.LocationFlags.OnTrap;

        /// <summary>腳下格是否處於防禦工事狀態。</summary>
        public bool IsOnDefence => _currentLocation != null && _currentLocation.LocationFlags.OnDefence;

        /// <summary>駐紮建築（部隊自身或腳下格建築）。</summary>
        public AbstractBuilding StationedBuilding => _unit.Building ?? _currentLocation?.Building;

        /// <summary>腳下格地形。</summary>
        public AbstractTerrain CurrentTerrain => _currentLocation?.Terrain;

        /// <summary>建立部隊與地圖格的綁定（尚未進入世界）。</summary>
        public UnitLocationBinding(Unit unit)
        {
            _unit = unit;
        }

        /// <summary>進入地圖世界：掛載格網、落點並登記至 <see cref="UnitRegistry"/>。</summary>
        /// <param name="autoGarrison">
        /// true＝腳下有據點時自動駐紮（一般進格）；
        /// false＝出征／剛產生時不駐紮，保持可立即移動。
        /// </param>
        public void BindToWorld(
            LocationGrid locationGrid,
            HexCoord startHex,
            AbstractTerrain terrainAtStart,
            bool autoGarrison = true)
        {
            _locationGrid = locationGrid;
            EnterHex(startHex, terrainAtStart, autoGarrison);
            // 出征部署：強制非駐紮，產生後即可行動並移出據點格
            if (!autoGarrison)
                StationRules.DepartStation(_unit);
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
            _unit.InheritMovementFrom(source._unit);
        }

        /// <summary>從目前腳下格／地形同步著火與環境傷害（不含續燃擲骰與灼燒結算）。</summary>
        public void SyncEnvironmentFromLocation() => ApplyFireAndMoraleFromLocation();

        /// <summary>相容舊呼叫；等同 <see cref="SyncEnvironmentFromLocation"/>。</summary>
        public void SyncFireFromLocation() => SyncEnvironmentFromLocation();

        /// <summary>離開目前六角格（更新格內佔位與交戰單位）。</summary>
        public void LeaveCurrentHex()
        {
            if (_currentLocation == null) return;
            _currentLocation.UnitMoveOut();
            if (_currentLocation.FightingUnit == _unit)
                _currentLocation.SetFightingUnit(null);
            _currentLocation = null;
        }

        /// <summary>進入指定六角格；駐紮中或格不可進入時失敗。</summary>
        /// <param name="autoGarrison">進格後是否自動駐紮（出征落點請傳 false）。</param>
        public bool EnterHex(HexCoord hex, AbstractTerrain terrainIfCreate = null, bool autoGarrison = true)
        {
            if (_locationGrid == null) return false;
            if (_unit.IsGarrison)
                return false;

            LeaveCurrentHex();

            MapLocation loc = terrainIfCreate != null
                ? _locationGrid.GetOrCreate(hex, terrainIfCreate)
                : _locationGrid.TryGet(hex, out var existing) ? existing : null;

            if (loc == null || !loc.UnitMoveIn()) return false;

            Position = hex;
            _currentLocation = loc;
            loc.SetFightingUnit(_unit);
            SyncFromLocation(loc, autoGarrison);
            return true;
        }

        /// <summary>沿路徑逐步移動；每步由腳下地形 <see cref="AbstractTerrain.CalculateMovement"/> 扣除離格行動力。駐紮中無法移動。</summary>
        public bool MoveAlongPath(HexGrid pathGrid, System.Collections.Generic.IReadOnlyList<HexCoord> path)
        {
            if (_unit.IsGarrison)
                return false;
            if (path == null || path.Count <= 1) return false;

            for (int i = 1; i < path.Count; i++)
            {
                var step = path[i];
                if (!pathGrid.CanEnter(step)) break;
                if (CurrentTerrain == null || !CurrentTerrain.CalculateMovement(_unit)) break;

                pathGrid.TryGet(step, out var cell);
                if (!EnterHex(step, TerrainRegistry.Get(cell.Terrain))) break;

                _currentLocation?.UnitMoved();
            }

            ApplyFireAndMoraleFromLocation();
            return true;
        }

        void SyncFromLocation(MapLocation loc, bool autoGarrison = true)
        {
            if (loc.Building != null)
                _unit.SetBuilding(loc.Building);

            if (autoGarrison)
                StationRules.TryAutoStation(_unit, loc);

            ApplyFireAndMoraleFromLocation();
        }

        /// <summary>離開駐紮狀態，可再次移動。</summary>
        public void DepartStation() => StationRules.DepartStation(_unit);

        void ApplyFireAndMoraleFromLocation()
        {
            if (_currentLocation == null)
            {
                _unit.SetFireEffect(0);
                return;
            }

            bool onFire = _currentLocation.LocationFlags.OnFire;
            int terrainFireEffect = _currentLocation.Terrain?.FireEffect ?? 0;
            _unit.SetFireEffect(onFire
                ? (short)(terrainFireEffect > 0 ? terrainFireEffect : 1)
                : (short)0);
            _unit.SetMoralePenalty((short)(_currentLocation.Terrain?.MoralePenalty ?? 0));
        }
    }
}
