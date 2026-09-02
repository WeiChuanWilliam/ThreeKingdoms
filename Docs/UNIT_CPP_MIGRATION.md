# 單位 Unit C++ → C# 對照

## 檔案

| C++ | C# |
|-----|-----|
| `Unit::AbstractUnit` | `Core/Units/Unit.cs` |
| `Unit::ActualUnit` | 併入 `Unit` + `UnitLocationBinding` |
| `unitFlags` | `Data/Units/UnitFlags.cs`（**部隊計略狀態**，見下） |
| `Building*` | `Core/Buildings/Building.cs` |
| `AbstractTerrain` | `Core/Terrain/AbstractTerrain.cs`（見 [TERRAIN.md](TERRAIN.md)） |

## 欄位

| C++ | C# |
|-----|-----|
| `unitName` | `unitName` / `UnitName` |
| `unitId` | `ushort unitId` |
| `fireEffect` | `FireEffect` |
| `moralePenalty` | `MoralePenalty` |
| `belonged` | `Belonged`（0=無勢力） |

## UnitFlags（部隊狀態）

**不是**地形傷害。負面狀態容器：`unit.Status`（混亂／偽報／止步／斷糧）。  
詳見 [UNIT_STATUS_AND_TERRAIN_HAZARD.md](UNIT_STATUS_AND_TERRAIN_HAZARD.md)。

腳下格火／陷阱傷害等級 → `Unit.TerrainHazard`（自 Location 同步）。

## 與 Spike

- 已移除 `MapUnit`、`UnitFactory`、`UnitState`；請用 `Unit` + `UnitLocationBinding` + `LocationGrid`。
- 部隊進入格時：`EnterHex` → 同步 `StationedBuilding`、`IsOnFire`（讀 Location）、`TerrainHazard`。

## C++ 原檔問題

- `#include` 後無標頭
- `ActualUnit` 類別為空且僅繼承 `AbstractTerrain`

請繼續貼：`Building.h`、`ForceUnit.h`、`Map` 模板定義（若有）。
