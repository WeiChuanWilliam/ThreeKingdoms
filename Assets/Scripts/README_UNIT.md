# 單位 Unit C++ → C# 對照

## 檔案

| C++ | C# |
|-----|-----|
| `Unit::AbstractUnit` | `Core/Units/AbstractUnit.cs` |
| `Unit::ActualUnit` | `Core/Units/ActualUnit.cs`（綁定 `LocationGrid`，讀著火/建築） |
| `unitFlags` | `Data/Units/UnitFlags.cs` |
| `Map<short,short>` ×4 | `Core/Units/UnitEffectMaps.cs` |
| `Building*` | `Core/Buildings/Building.cs` |
| `ForceUnit*` | `Core/Units/ForceUnit.cs` |
| `AbstractTerrain` | `Core/Terrain/AbstractTerrain.cs`（見 README_TERRAIN.md） |

## 欄位

| C++ | C# |
|-----|-----|
| `unitName` | `unitName` / `UnitName` |
| `unitId` | `ushort unitId` |
| `reachable` | `UnitFlags.Reachable` |
| `fireEffect` | `FireEffect` |
| `moralePenalty` | `MoralePenalty` |
| `belonged` | `Belonged`（0=無勢力） |

## 與 Spike

- 已移除 `MapUnit`、`UnitFactory`、`UnitState`；請用 `ActualUnit` + `LocationGrid`。
- 部隊進入格時：`EnterHex` → 同步 `StationedBuilding`、`IsOnFire`（讀 Location）。
- `Officer.Defend*Skill(..., AbstractUnit selfUnit)` 已對應 `Core.Units.AbstractUnit`。

## C++ 原檔問題

- `#include` 後無標頭
- `ActualUnit` 類別為空且僅繼承 `AbstractTerrain`

請繼續貼：`Building.h`、`ForceUnit.h`、`Map` 模板定義（若有）。
