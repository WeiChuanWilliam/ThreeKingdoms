# Unit 相關 C# 檔案說明（還原後）

## 你的 C++ 對應（請以這些為準調整）

| C++ | C# 檔案 |
|-----|---------|
| `Unit::AbstractUnit` | `Core/Units/Unit.cs` |
| `unitFlags` | `Data/Units/UnitFlags.cs`（部隊計略狀態；非地形傷害） |

## 先前誤刪、已還原的檔案

| 檔案 | 說明 |
|------|------|
| `Data/Units/UnitDef.cs` | **非 C++**，可選 JSON 用 |
| `Data/Units/UnitFactory.cs` | **非 C++**，可選建立部隊 |

## 曾用錯名、已不用

| 名稱 | 處理 |
|------|------|
| `MapUnit` / `ActualUnit` | 已改為 `Unit` |
| `UnitState` | Spike 舊測試類，**已刪除** |
| `UnitEffectMaps` / `TerrainEffectMaps` / `GroupUnit` | C++ 佔位表／編組，**已刪除**（從未接入公式） |
| `UnitCombatPowerRules` / `StationedCombatRules` | 舊戰力總分／駐紮加成，**已刪除**（改 `CombatStatMath`） |
| `CombatStatKind` / `CombatStatModifierHooks` / `ICombatTroopStatsSource` | 舊戰力修飾管線，**已刪除** |
| `UnitFireRules` / `UnitSunriseFireTick` | 舊火傷入口，**已刪除**（改 `UnitBurnRules` + `LocationGridSunrise`） |
| `CombatSkillAssembler` | 戰法組裝佔位，**已刪除**（戰法 API 暫停） |
| `UnitState` / `Data/Battle` / `Core/Map` | Spike 舊測試／空目錄，**已刪除** |

## 額外接上 Location 的檔案（非 C++ 標頭）

| 檔案 | 說明 |
|------|------|
| `Core/Units/UnitLocationBinding.cs` | 建築/著火/進出格 |

Spike 使用：`Unit` + `Unit.Location`（Binding）。
