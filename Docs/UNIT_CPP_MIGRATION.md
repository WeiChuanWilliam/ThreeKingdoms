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

**不是**地形傷害。負面狀態為剩餘天數（`byte`，0～255）：`unit.Status`（`Mess`／`Deceive`／`Stop`／`CutSupply`）。  
`CutSupply` 規則特殊（擊破倒數 → 到期斷量；補糧即取消），見 [UNIT_STATUS_AND_TERRAIN_HAZARD.md](UNIT_STATUS_AND_TERRAIN_HAZARD.md)。

腳下格火／陷阱 → `Unit.IsOnFire`／`IsOnTrap`（讀 LocationFlags）。

## 與 Spike

- 已移除 `MapUnit`、`UnitFactory`、`UnitState`；請用 `Unit` + `UnitLocationBinding` + `LocationGrid`。
- 部隊進入格時：`EnterHex` → 同步 `StationedBuilding`、`FireEffect`／士氣懲罰；著火／陷阱讀 LocationFlags。

## C++ 原檔問題

- `#include` 後無標頭
- `ActualUnit` 類別為空且僅繼承 `AbstractTerrain`

請繼續貼：`Building.h`、`ForceUnit.h`、`Map` 模板定義（若有）。
