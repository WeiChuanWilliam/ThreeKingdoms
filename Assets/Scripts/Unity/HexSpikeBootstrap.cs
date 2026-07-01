using System.Collections.Generic;
using ThreeKindoms.Core;
using UnityEngine;

namespace ThreeKindoms.UnityBridge
{
    /// <summary>
    /// 掛在場景空物件上即可 Play 測試。
    /// 左鍵點格：有選取則移動；Shift+左鍵：只預覽路徑。
    /// R：下一天日出。Esc：取消選取。
    /// </summary>
    public class HexSpikeBootstrap : MonoBehaviour
    {
        [Header("Hex 顯示")]
        [SerializeField] float hexSize = 0.55f;
        [SerializeField] float cellSpriteScale = 1.05f;

        HexSpikeGame _game;
        GameObject _unitMarker;
        bool _unitSelected;
        LineRenderer _pathLine;
        HexCoord? _hover;

        void Start()
        {
            EnsureMainCamera();
            if (!ThreeKindoms.Data.Units.UnitConfigUtil.Load(GamePaths.ChineseUnitProperties))
                Log("未載入 chinese/unit.properties，使用程式內建預設值");
            ThreeKindoms.Data.Units.TroopKinds.TroopKindRegistry.EnsureBuilt();

            ThreeKindoms.Data.Officers.OfficerConfigUtil.LoadDefault(Application.streamingAssetsPath);
            OfficerDatabaseUnity.LoadScenarioRuntime();
            ThreeKindoms.Data.Skill.SkillPool.Register(101);
            ThreeKindoms.Data.Skill.SkillPool.Register(102);
            var sample = ThreeKindoms.Data.Officers.OfficerDatabase.TryGetRuntime(1);
            if (sample != null)
                Log($"武將表：{sample.FullName} 武{sample.Attack} 智{sample.Intelligence}");

            var grid = HexMapJsonFile.LoadOrCreateDefault();
            HexCoord start = FindStartCoord(grid);
            _game = new HexSpikeGame(grid, start);

            BuildMapVisuals();
            CreateUnitMarker();
            CreatePathLine();

            _game.SelectUnit();
            _unitSelected = true;
            Log(_game.LastMessage);
        }

        static void EnsureMainCamera()
        {
            var cam = Camera.main;
            if (cam == null)
            {
                var go = new GameObject("Main Camera");
                cam = go.AddComponent<Camera>();
                go.tag = "MainCamera";
            }
            cam.orthographic = true;
            cam.orthographicSize = 6f;
            cam.backgroundColor = new Color(0.12f, 0.14f, 0.18f);
            cam.transform.position = new Vector3(6f, 5f, -10f);
            cam.nearClipPlane = 0.1f;
            cam.farClipPlane = 100f;
        }

        static HexCoord FindStartCoord(HexGrid grid)
        {
            foreach (var c in grid.AllCoords)
                return c;
            return new HexCoord(0, 0);
        }

        void BuildMapVisuals()
        {
            var parent = new GameObject("HexCells").transform;
            parent.SetParent(transform);

            foreach (var c in _game.Grid.AllCoords)
            {
                var go = new GameObject($"Hex_{c.Q}_{c.R}");
                go.transform.SetParent(parent);
                go.transform.position = HexLayout.ToWorld(c, hexSize);

                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = CreateHexSprite();
                sr.color = _game.GetTerrainColor(c);
                sr.sortingOrder = 0;
                go.transform.localScale = Vector3.one * cellSpriteScale;

                go.AddComponent<HexCellClick>().Init(c, this);
                _game.RegisterCellVisual(c, go);
            }
        }

        void CreateUnitMarker()
        {
            _unitMarker = new GameObject("Unit");
            _unitMarker.transform.SetParent(transform);
            var sr = _unitMarker.AddComponent<SpriteRenderer>();
            sr.sprite = CreateUnitSprite();
            sr.color = new Color(0.9f, 0.2f, 0.15f);
            sr.sortingOrder = 10;
            _unitMarker.transform.localScale = Vector3.one * 0.35f;
            SyncUnitPosition();
        }

        void CreatePathLine()
        {
            var go = new GameObject("PathLine");
            go.transform.SetParent(transform);
            _pathLine = go.AddComponent<LineRenderer>();
            _pathLine.positionCount = 0;
            _pathLine.startWidth = _pathLine.endWidth = 0.08f;
            _pathLine.material = new Material(Shader.Find("Sprites/Default"));
            _pathLine.startColor = _pathLine.endColor = new Color(1f, 0.9f, 0.2f, 0.9f);
            _pathLine.sortingOrder = 5;
            _pathLine.useWorldSpace = true;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                _game.NextDaySunrise();
                ClearPathLine();
                Log(_game.LastMessage);
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                _unitSelected = false;
                ClearPathLine();
                Log("取消選取");
            }
        }

        public void OnCellClicked(HexCoord c, bool previewOnly)
        {
            if (!_unitSelected)
            {
                _unitSelected = true;
                _game.SelectUnit();
            }

            if (previewOnly)
            {
                _game.PreviewPath(c, out var path, out _);
                DrawPath(path);
                Log(_game.LastMessage);
                return;
            }

            _game.TryMoveTo(c);
            ClearPathLine();
            SyncUnitPosition();
            RefreshTerrainColors();
            Log(_game.LastMessage);
        }

        public void OnCellHover(HexCoord c)
        {
            _hover = c;
        }

        void SyncUnitPosition()
        {
            _unitMarker.transform.position = HexLayout.ToWorld(_game.PlayerLocation.Position, hexSize) + Vector3.back * 0.1f;
        }

        void RefreshTerrainColors()
        {
            foreach (var c in _game.Grid.AllCoords)
            {
                if (_game.Grid.TryGet(c, out _) == false) continue;
                // visuals stored in game - skip full refresh for spike
            }
        }

        void DrawPath(List<HexCoord> path)
        {
            if (path == null || path.Count == 0)
            {
                ClearPathLine();
                return;
            }
            _pathLine.positionCount = path.Count;
            for (int i = 0; i < path.Count; i++)
                _pathLine.SetPosition(i, HexLayout.ToWorld(path[i], hexSize) + Vector3.back * 0.05f);
        }

        void ClearPathLine() => _pathLine.positionCount = 0;

        static void Log(string msg) => Debug.Log($"[ThreeKindoms] {msg}");

        static Sprite CreateHexSprite()
        {
            const int res = 32;
            var tex = new Texture2D(res, res);
            var pivot = new Vector2(0.5f, 0.5f);
            float cx = res * 0.5f, cy = res * 0.5f, radius = res * 0.45f;
            for (int y = 0; y < res; y++)
            for (int x = 0; x < res; x++)
            {
                bool inside = PointInHex(x, y, cx, cy, radius);
                tex.SetPixel(x, y, inside ? Color.white : Color.clear);
            }
            tex.Apply();
            tex.filterMode = FilterMode.Bilinear;
            return Sprite.Create(tex, new Rect(0, 0, res, res), pivot, res);
        }

        static bool PointInHex(int px, int py, float cx, float cy, float radius)
        {
            float dx = Mathf.Abs(px - cx);
            float dy = Mathf.Abs(py - cy);
            return dy <= radius * 0.866f && dx * 0.5f + dy * 0.866f <= radius;
        }

        static Sprite CreateUnitSprite()
        {
            var tex = new Texture2D(8, 8);
            for (int i = 0; i < 64; i++) tex.SetPixel(i % 8, i / 8, Color.white);
            tex.Apply();
            return Sprite.Create(tex, new Rect(0, 0, 8, 8), new Vector2(0.5f, 0.5f), 8);
        }

        void OnGUI()
        {
            var core = _game.PlayerUnit;
            var loc = _game.PlayerLocation;
            var clock = _game.Clock;
            GUILayout.BeginArea(new Rect(10, 10, 420, 200), GUI.skin.box);
            GUILayout.Label("ThreeKindoms — Hex Spike");
            GUILayout.Label($"第 {clock.TotalDay} 天（旬內第 {clock.DayInXun} 天）");
            GUILayout.Label($"[{core.Kind}] {core.UnitName} 士氣{core.Morale} 體力{core.Stamina}");
            GUILayout.Label($"兵{core.Soldiers} 傷{core.Wounded} 戰力{core.EffectiveCombatStrength}" + (core.IsAnnihilated ? " 團滅" : ""));
            GUILayout.Label($"@ {loc.Position}  行動力 {loc.MovementPointsLeft}/15  糧/日{core.CalculateFoodConsumption()}");
            if (loc.IsOnFire) GUILayout.Label("狀態：腳下格著火");
            if (loc.StationedBuilding != null) GUILayout.Label($"建築：{loc.StationedBuilding.Name}");
            if (_hover.HasValue)
                GUILayout.Label($"指向: {_hover.Value}");
            GUILayout.Label(_game.LastMessage);
            GUILayout.Space(6);
            GUILayout.Label("左鍵：移動 | Shift+左鍵：預覽路徑 | R：下一天 | Esc：取消選取");
            if (GUILayout.Button("下一天（日出 +15）"))
            {
                _game.NextDaySunrise();
                ClearPathLine();
            }
            GUILayout.EndArea();
        }
    }
}
