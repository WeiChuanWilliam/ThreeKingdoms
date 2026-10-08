# 部隊狀態 vs 地形／格子

## 三層分工（勿混用）

| 層 | 用途 | 程式 |
|----|------|------|
| **AbstractTerrain + TerrainFlags** | 地形**類型**能不能：可達、可建、可陷、可燃… | `Core/Terrain/AbstractTerrain.cs`、`Data/Terrain/TerrainFlags.cs` |
| **LocationFlags** | **這一格當下**有沒有：著火、陷阱、防禦… | `Data/Locations/LocationFlags.cs` |
| **Unit 腳下布林** | 部隊讀腳下格：有沒有火／陷阱 | `Unit.IsOnFire`／`Unit.IsOnTrap`（轉讀 LocationFlags） |

```text
TerrainFlags.Fireable = true   → 這種地「可以」著火
LocationFlags.OnFire = true    → 這一格「現在」著火
unit.IsOnFire                  → 這支部隊腳下「現在」著火
```

沒有傷害等級 enum／快照；有無傷害只看布林。

## 部隊計略狀態（UnitFlags）

與地形無關：混亂／偽報／止步／斷糧為剩餘天數（`byte`）。見同檔下文或程式註解。

`Data/Units/UnitFlags.cs`：

| 狀態 | 欄位 | 行為摘要 |
|------|------|----------|
| 混亂 | `Mess` | 日出減 1 |
| 偽報 | `Deceive` | 日出減 1 |
| 止步 | `Stop` | 日出減 1 |
| 斷糧 | `CutSupply` | 擊破倒數→到期斷量；補糧即取消 |

## 傷害結算（火）

```
格子 OnFire → unit.IsOnFire
  → UnitBurnRules（讀地形 FireEffect n 等）→ BattleDamageResult.Apply
```

已移除：`UnitHazardSnapshot`、`TerrainHazardLevel`、部隊上的 FlameDamage／等級倍率。

## 已移除的錯誤設計

舊 `UnitFlags` 曾放 `Reachable`、`FlameDamage`、`TrapDamage`（实为格子／類型特性）。  
曾用 `UnitNegativeStatus`／傷害等級快照，已廢除。
