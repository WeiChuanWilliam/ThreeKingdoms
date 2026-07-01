using System;
using System.Collections.Generic;
using ThreeKindoms.Core;
using ThreeKindoms.Core.Locations;
using ThreeKindoms.Core.Terrain;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>
    /// Unity 六角地圖試玩殼層。
    /// <para>SKELETON：僅保留欄位與方法簽名；完整遊戲流程待 Unit／地圖／劇本定稿後接線。</para>
    /// </summary>
    public sealed class HexSpikeGame
    {
        public HexGrid Grid { get; }
        public LocationGrid Locations { get; }
        public GameClock Clock { get; } = new();
        public Unit PlayerUnit { get; }
        public UnitLocationBinding PlayerLocation => PlayerUnit?.Location;
        public string LastMessage { get; private set; } = "";

        readonly Dictionary<HexCoord, GameObject> _cellVisuals = new();

        public HexSpikeGame(HexGrid grid, HexCoord start)
        {
            Grid = grid;
            Locations = BuildLocationGrid(grid);
            PlayerUnit = CreateFallbackPlayerUnit(start);

            if (grid.TryGet(start, out var cell))
                PlayerUnit.Location.BindToWorld(Locations, start, TerrainDefinition.FromTerrainType(cell.Terrain));
        }

        static Unit CreateFallbackPlayerUnit(HexCoord start)
        {
            var def = new LegionUnitDef(1)
            {
                CommanderOfficerId = 1,
                Soldiers = 1000,
                Wounded = 500,
                Food = 3000
            };
            def.AddViceOfficer(3);
            return new Legion(def);
        }

        static LocationGrid BuildLocationGrid(HexGrid grid)
        {
            var locGrid = new LocationGrid();
            foreach (var hex in grid.AllCoords)
            {
                if (grid.TryGet(hex, out var cell))
                    locGrid.GetOrCreate(hex, TerrainDefinition.FromTerrainType(cell.Terrain));
            }
            return locGrid;
        }

        /// <summary>登記格子的視覺物件（Unity 用）。</summary>
        public void RegisterCellVisual(HexCoord c, GameObject go) => _cellVisuals[c] = go;

        /// <summary>更新選取部隊的狀態訊息。</summary>
        public void SelectUnit()
        {
            if (PlayerUnit == null)
            {
                LastMessage = "無玩家部隊";
                return;
            }
            LastMessage = $"[{PlayerUnit.Kind}] {PlayerUnit.UnitName} @ {PlayerLocation?.Position}";
        }

        /// <summary>嘗試移動至目標格。</summary>
        public void TryMoveTo(HexCoord target) => LastMessage = "移動：尚未實作";

        /// <summary>預覽路徑與行動力消耗。</summary>
        public void PreviewPath(HexCoord target, out List<HexCoord> path, out int totalCost)
        {
            path = new List<HexCoord>();
            totalCost = 0;
            LastMessage = "路徑預覽：尚未實作";
        }

        /// <summary>推進至下一個日出並結算。</summary>
        public void NextDaySunrise()
        {
            Clock.AdvanceToNextSunrise();
            LastMessage = $"第 {Clock.TotalDay} 天日出（結算尚未實作）";
        }

        /// <summary>依地形與著火狀態回傳格色。</summary>
        public Color GetTerrainColor(HexCoord c) => Color.gray;
    }
}
