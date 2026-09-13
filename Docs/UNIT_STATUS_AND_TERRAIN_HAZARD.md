# 部隊狀態 vs 地形傷害

## 兩套概念（勿混用）

| 概念 | 用途 | 程式位置 |
|------|------|----------|
| **地形／格子環境傷害** | 著火、陷阱、毒沼、棧道等「腳下格」造成的傷害等級 | `TerrainHazardLevel`、`LocationFlags`、`UnitHazardSnapshot` |
| **部隊計略／異常狀態** | 混亂、偽報、止步、斷糧等「掛在部隊上」的負面狀態 | `UnitFlags` 剩餘天數（`byte`） |

## 地形傷害

- 等級 enum：`Data/Terrain/TerrainHazard.cs` → `TerrainHazardLevel`（0～3）
- 格子當下狀態：`LocationFlags.OnFire` / `OnTrap` + 地形 `FireEffect`
- 部隊進格時同步快照：`Unit.TerrainHazard`（`UnitHazardSnapshot`）
- 火傷公式讀快照，不寫入 `UnitFlags`

地形類型旗標（可蓋、可陷等）仍在 `TerrainFlags`；具體毒沼／棧道傷害公式尚未實作。

## 部隊狀態（UnitFlags）

`Data/Units/UnitFlags.cs`：每個 `Unit` 自帶一份，直接讀寫。  
欄位為 **剩餘天數**（`byte`，0～255；0＝無此狀態）。行動限制用 `> 0` 判斷。

| 狀態 | 欄位 | properties 字串 | 行為 |
|------|------|-------------------|------|
| 混亂 | `Mess` | `status.on_mess` | 日出減 1；到 0 自動結束 |
| 偽報 | `Deceive` | `status.on_deceive` | 同上 |
| 止步 | `Stop` | `status.on_trap` | 同上 |
| 斷糧 | `CutSupply` | （待補） | **特殊規則**（見下） |

讀寫：`unit.Status.Mess = 3;`、`if (unit.Status.Mess > 0) …`  
無 `HasAnyNegative`：需要時各自檢查欄位即可。

### 斷糧（CutSupply）特殊規則

與一般計略狀態不同：

1. **觸發**：兵團（`Legion`）被擊破 → 開始斷糧倒數（寫入 `CutSupply` 天數）
2. **到期**：倒數歸零 → **直接斷量**（清零攜糧，例如 `Legion.CarriedFood`）
3. **取消**：倒數尚未歸零期間，**只要糧食補上** → 立即 `CutSupply = 0`（取消斷糧）

日出 Tick／補糧取消／擊破觸發的具體函數尚未接上，僅資料形狀與規則定稿。

## 傷害結算管線（不變）

```
交戰／火攻／（未來）陷阱… → BattleDamageResult → Apply(unit)
計略狀態（未來）         → 寫入 UnitFlags 剩餘天數
斷糧（未來）             → 擊破設 CutSupply；日出到期斷量；補糧清 CutSupply
```

## 已移除的錯誤設計

舊 `UnitFlags` 曾放 `Reachable`、`FlameDamage`、`TrapDamage`（实为格子特性），已迁出。  
曾用 `UnitNegativeStatus`／純布林／`HasAnyNegative`，已改為剩餘天數直接讀寫。
