# 地形 Terrain C++ → C# 對照

## 命名空間

| C++ | C# |
|-----|-----|
| `Terrain::AbstractTerrain` | `ThreeKindoms.Core.Terrain.AbstractTerrain` |

（舊的 `Core/Map/AbstractTerrain.cs` 已移除，避免重名。）

## 檔案

| 內容 | 路徑 |
|------|------|
| 抽象地形 | `Core/Terrain/AbstractTerrain.cs` |
| 具體定義 | `Core/Terrain/TerrainDefinition.cs` |
| 旗標 | `Data/Terrain/TerrainFlags.cs` |
| 環境傷害等級 | `Data/Terrain/TerrainHazard.cs` → `TerrainHazardLevel` |
| **參數表（主）** | `StreamingAssets/chinese/terrain.properties` → `TerrainConfigUtil` |
| JSON（Unity 舊路徑） | `Data/Terrain/TerrainDef.cs`、`TerrainDatabase.cs`、`terrains.json` |

## 欄位

| C++ | C# |
|-----|-----|
| `terrainName` / `terrain_id()` | `TerrainName` / `TerrainId` |
| `TerrainFlags` 位元 | `TerrainFlags` struct + `Pack`/`Unpack` |
| `fireEffect` / `moralePenalty` | `int`（與 C++ 一致） |

## 地形傷害（非 UnitFlags）

著火、陷阱、毒沼、棧道等環境傷害屬於**地形／格子**：

- 類型能力：`TerrainFlags`（可火、可陷…）
- 當格狀態：`LocationFlags.OnFire` / `OnTrap`
- 傷害等級：`TerrainHazardLevel` → 進格時同步到 `Unit.TerrainHazard`

詳見 [UNIT_STATUS_AND_TERRAIN_HAZARD.md](UNIT_STATUS_AND_TERRAIN_HAZARD.md)。

## 與六角 Spike 的關係

- `CellData.Terrain`（enum）→ `TerrainDefinition.FromTerrainType(...).SuggestedEnterCost` 作為 `enterCost`
- 之後可在 `CellData` 加 `int terrainDefId` 指向表資料

## 與 Unit 的差異

地形是**類型定義**（平原／山地等），不是地圖上的部隊。部隊用 `Unit` + `LocationGrid`。

載入範例（建議）：

```csharp
TerrainConfigUtil.Load(Path.Combine(streamingAssets, "chinese/terrain.properties"));
TerrainConfigUtil.TryGetDefinitionById(0, out TerrainDefinition plain);
```

Unity 舊路徑（仍可用）：

```csharp
var terrains = TerrainDatabase.LoadFromStreamingAssets();
var plain = terrains.Get(0);
```
