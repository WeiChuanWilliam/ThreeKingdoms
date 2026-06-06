# Unit 相關 C# 檔案說明（還原後）

## 你的 C++ 對應（請以這些為準調整）

| C++ | C# 檔案 |
|-----|---------|
| `Unit::AbstractUnit` | `Core/Units/AbstractUnit.cs` |
| `Unit::ActualUnit` | `Core/Units/ActualUnit.cs`（類別本體保持精簡） |
| `unitFlags` | `Data/Units/UnitFlags.cs` |
| 四張 `Map<short,short>` | `Core/Units/UnitEffectMaps.cs` → `AbstractUnit.effectMaps` |

## 先前誤刪、已還原的檔案

| 檔案 | 說明 |
|------|------|
| `Data/Units/UnitDef.cs` | **非 C++**，可選 JSON 用 |
| `Data/Units/UnitFactory.cs` | **非 C++**，可選建立部隊 |
| `Core/UnitState.cs` | **非 C++**，僅 Spike 舊測試用 |

## 曾用錯名、已不用

| 名稱 | 處理 |
|------|------|
| `MapUnit` | 已移除；請用 `ActualUnit` |

## 額外接上 Location 的檔案（非 C++ 標頭）

| 檔案 | 說明 |
|------|------|
| `ActualUnitLocationBinding.cs` | 建築/著火/進出格；你可併入 ActualUnit 或改寫 |

Spike 使用：`PlayerUnit`（ActualUnit）+ `PlayerUnit.Location`（Binding）。
