# 部隊狀態 vs 地形傷害

## 兩套概念（勿混用）

| 概念 | 用途 | 程式位置 |
|------|------|----------|
| **地形／格子環境傷害** | 著火、陷阱、毒沼、棧道等「腳下格」造成的傷害等級 | `TerrainHazardLevel`、`LocationFlags`、`UnitHazardSnapshot` |
| **部隊計略／異常狀態** | 混亂、偽報、止步、斷糧等「掛在部隊上」的負面狀態 | `UnitFlags` + `UnitNegativeStatus` 子類 |

## 地形傷害

- 等級 enum：`Data/Terrain/TerrainHazard.cs` → `TerrainHazardLevel`（0～3）
- 格子當下狀態：`LocationFlags.OnFire` / `OnTrap` + 地形 `FireEffect`
- 部隊進格時同步快照：`Unit.TerrainHazard`（`UnitHazardSnapshot`）
- 火傷公式讀快照，不寫入 `UnitFlags`

地形類型旗標（可蓋、可陷等）仍在 `TerrainFlags`；具體毒沼／棧道傷害公式尚未實作。

## 部隊狀態（UnitFlags）

`Data/Units/UnitFlags.cs` 容器，內含：

| 狀態 | 類別 | properties 字串 |
|------|------|-------------------|
| 混亂 | `UnitMessStatus` | `status.on_mess` |
| 偽報 | `UnitDeceiveStatus` | `status.on_deceive` |
| 止步 | `UnitStopStatus` | `status.on_trap` |
| 斷糧 | `UnitCutSupplyStatus` | （待補） |

讀取：`unit.Status.Mess.IsActive` 等。  
目前所有子類 **未實作**，`IsActive` 預設 **false**。

## 傷害結算管線（不變）

```
交戰／火攻／（未來）陷阱… → BattleDamageResult → Apply(unit)
計略狀態（未來）         → 另設 Result 或擴充 Apply → 寫入 UnitFlags 子狀態
```

## 已移除的錯誤設計

舊 `UnitFlags` 曾放 `Reachable`、`FlameDamage`、`TrapDamage`（实为格子特性），已迁出。
