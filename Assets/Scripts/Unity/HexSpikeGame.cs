using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Buildings;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Scenario;
using ThreeKindoms.Data.Units;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    public sealed class HexSpikeGame
    {
        public HexGrid Grid { get; }
        public LocationGrid Locations { get; }
        public GameClock Clock { get; } = new();

        /// <summary>玩家部隊（父類型 Unit，執行時為 <see cref="Legion"/> 等子類）。</summary>
        public Unit PlayerUnit { get; }

        public UnitLocationBinding PlayerLocation => PlayerUnit?.Location;
        public string LastMessage { get; private set; } = "";

        readonly Dictionary<HexCoord, GameObject> _cellVisuals = new();

        public HexSpikeGame(HexGrid grid, HexCoord start)
        {
            Grid = grid;
            Locations = BuildLocationGrid(grid);

            List<ScenarioSpawnedUnit> placed = ScenarioPlacementBridge.LoadAndPlace(
                "scenario_start.properties", Locations, Grid);

            PlayerUnit = placed.Count > 0
                ? placed.Find(s => s.SpawnKey == "player" || s.SpawnKey == "spawn.player")?.Unit
                  ?? placed[0].Unit
                : CreateFallbackPlayerUnit(start);

            if (placed.Count == 0)
                ScenarioPlacementBridge.PlaceUnit(PlayerUnit, Locations, Grid, start);

            var demoCity = new HexCoord(5, 5);
            if (Locations.TryGet(demoCity, out var demoLoc))
                demoLoc.SetBuilding(new Building(100, "襄陽", Core.Buildings.SettlementSiteKind.City));

            Clock.AdvanceToNextSunrise();
        }

        static Unit CreateFallbackPlayerUnit(HexCoord start)
        {
            var def = new Data.Units.LegionUnitDef(1)
            {
                CommanderOfficerId = 1,
                EscortCommanderOfficerId = 2,
                Soldiers = 1000,
                Wounded = 500
            };
            def.AddViceOfficer(3);
            return new Legion(def);
        }

        static LocationGrid BuildLocationGrid(HexGrid grid)
        {
            var locGrid = new LocationGrid();
            foreach (var hex in grid.AllCoords)
            {
                grid.TryGet(hex, out var cell);
                locGrid.GetOrCreate(hex, TerrainDefinition.FromTerrainType(cell.Terrain));
            }
            return locGrid;
        }

        public void RegisterCellVisual(HexCoord c, GameObject go) => _cellVisuals[c] = go;

        public void SelectUnit()
        {
            var loc = PlayerLocation;
            var u = PlayerUnit;
            var b = loc.StationedBuilding;
            string buildingInfo = b != null ? $" 建築[{b.Name}]" : "";
            string fireInfo = loc.IsOnFire ? $" [{UnitConfigUtil.StatusOnFire}]" : "";
            if (loc.IsOnTrap) fireInfo += $" [{UnitConfigUtil.StatusOnTrap}]";
            if (u.TrapDamage != HazardDamageLevel.None)
                fireInfo += $" {UnitConfigUtil.StatusOnTrap}{u.TrapDamage}";
            if (u.FlameDamage != HazardDamageLevel.None)
                fireInfo += $" {UnitConfigUtil.StatusOnFire}{u.FlameDamage}";
            string annihilated = u.IsAnnihilated ? " [團滅]" : "";
            LastMessage = $"[{u.Kind}] {u.UnitName} 士氣{u.Morale} 體力{u.Stamina} 兵{u.Soldiers}(傷{u.Wounded}) 戰力{u.EffectiveCombatStrength}{annihilated} @ {loc.Position} 糧/日{u.CalculateFoodConsumption()}{buildingInfo}{fireInfo}";
        }

        public void TryMoveTo(HexCoord target)
        {
            var find = HexPathfinder.FindPath(Grid, PlayerLocation.Position, target, PlayerLocation.MovementPointsLeft);
            if (!find.Success)
            {
                LastMessage = find.FailureReason;
                return;
            }

            PlayerLocation.MoveAlongPath(Grid, find.Path);
            LastMessage = $"移動至 {PlayerLocation.Position}，剩餘行動力 {PlayerLocation.MovementPointsLeft}";
            if (PlayerLocation.IsOnFire)
                LastMessage += "（腳下著火）";
        }

        public void PreviewPath(HexCoord target, out List<HexCoord> path, out int totalCost)
        {
            path = new List<HexCoord>();
            totalCost = 0;
            var result = HexPathfinder.FindPath(Grid, PlayerLocation.Position, target, PlayerLocation.MovementPointsLeft);
            if (!result.Success)
            {
                LastMessage = result.FailureReason;
                return;
            }
            path = new List<HexCoord>(result.Path);
            totalCost = result.TotalCost;
            LastMessage = $"預覽：{path.Count - 1} 步，需 {totalCost} 點";
        }

        public void NextDaySunrise()
        {
            Clock.AdvanceToNextSunrise();
            var rng = new System.Random();
            Locations.TickFireAtSunrise(rng);
            Locations.TickAllUnitsFireAtSunrise();
            PlayerLocation.RefillMovementAtSunrise();
            string fireNote = PlayerUnit.IsOnFire ? $" {UnitConfigUtil.StatusOnFire}" : "";
            LastMessage = $"第 {Clock.TotalDay} 天日出 行動力 {PlayerLocation.MovementPointsLeft}{fireNote}";
        }

        public Color GetTerrainColor(HexCoord c)
        {
            if (!Grid.TryGet(c, out var cell))
                return Color.gray;
            if (Locations.IsBurning(c))
                return new Color(0.9f, 0.35f, 0.1f);
            return cell.Terrain switch
            {
                TerrainType.Road => new Color(0.85f, 0.75f, 0.45f),
                TerrainType.Hill => new Color(0.45f, 0.55f, 0.35f),
                TerrainType.Mountain => new Color(0.4f, 0.4f, 0.45f),
                TerrainType.Water => new Color(0.2f, 0.4f, 0.75f),
                TerrainType.Forest => new Color(0.15f, 0.45f, 0.2f),
                _ => new Color(0.35f, 0.65f, 0.3f)
            };
        }
    }
}
