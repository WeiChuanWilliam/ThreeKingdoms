# 部隊 Unit 與地格 Location（請先讀這份）

## 你問的：UnitFactory / MapUnit 是什麼？

| 名稱 | 來源 | 現在 |
|------|------|------|
| **UnitFactory** | 我加的 Spike 範例（從 JSON 造部隊） | **已刪除**（你沒設計 JSON 部隊表） |
| **MapUnit** | 我把 C++ `ActualUnit` 暫時改的名字 | **已刪除**，改回 **`ActualUnit`** |
| **UnitState** | 最早六角移動測試用的小類 | **已刪除**，併入 `ActualUnit` |

你的 C++ 裡真正的地圖部隊 = **`Unit::AbstractUnit` / `ActualUnit`**。  
C# 對應：

- 規則與欄位 → `AbstractUnit`
- 地圖上會動的部隊 → **`ActualUnit`**

## 部隊要連到建築、著火 —— 怎麼接？

**不在部隊上憑空寫 `onFire`，而是腳下那一格 `MapLocation`。**

```text
ActualUnit
  ├─ BindToWorld(LocationGrid, 起始 hex, 地形)
  ├─ EnterHex / LeaveCurrentHex  → 呼叫 Location.UnitMoveIn / Out
  ├─ CurrentLocation             → 目前 MapLocation
  ├─ IsOnFire / IsOnDefence      → 讀 Location.LocationFlags
  ├─ StationedBuilding           → 部隊.building 或 腳下格.Building
  └─ FireEffect / MoralePenalty  → 著火時從 Location.Terrain 同步
```

建築放在 **格子上**（與你 C++ `Location::building` 一致）：

```csharp
locationGrid.TryGet(hex, out var loc);
loc.SetBuilding(new Building(id, "襄陽"));
// 部隊走進該 hex 後，PlayerUnit.StationedBuilding 會指向同一建築
```

著火在 **格子上**（與 `LocationFlag.onFire` 一致）：

```csharp
loc.SetOnFire();  // 需地形 Fireable
// 部隊在該格時 ActualUnit.IsOnFire == true
```

## 戰役／戰鬥架構（修訂）

- **取消**：補給線切斷斷糧、戰術格山谷／之石、獨立「駐紮部隊」子類
- **駐紮（定案）**：`Unit.IsGarrison` 布林；三種部隊進建築一律駐紮；`CalculateCombatPower` 依狀態呼叫兩個抽象方法（見 [`UNIT_TYPES.md`](UNIT_TYPES.md)）
- **新增**：戰略 **Adjacent** 壓制、兵團自帶糧薪、戰術 **Facing** 向量  
→ 詳見 [`COMBAT_AND_CAMPAIGN_ARCHITECTURE.md`](COMBAT_AND_CAMPAIGN_ARCHITECTURE.md)

### 駐紮狀態

| 項目 | 位置 |
|------|------|
| 部隊種類 | `Legion` / `Combat` / `Transport`（無第四種子類） |
| 布林 | `Unit.IsGarrison` / `Unit.SetGarrison(bool)` |
| 進據點自動駐紮 | `StationRules.TryAutoStation` ← `UnitLocationBinding` 進格時 |
| 離開駐紮 | `UnitLocationBinding.DepartStation()` |
| 駐紮中不可移動 | `UnitLocationBinding` 檢查 `IsGarrison` |
| 戰鬥力分流 | `CalculateCombatPower` → `CalculateGarrisonCombatPower` / `CalculateNonGarrisonCombatPower` |
| 據點類型 | [`SETTLEMENT_SITES.md`](SETTLEMENT_SITES.md) |

## 你還沒設計、仍是佔位的

- BattleSkill、Items、Personality 詳細規則
- Building 只有 id+名稱（城池系統未做）
- ForceUnit 編制（未設計）
- `GroupUnit`：舊 C++ 地格多隊佔位，**不是**三種部隊類型之一；見 `GroupUnit.cs`
- 駐紮戰鬥力係數表（運輸偏防、兵團高攻高防、戰鬥部隊駐紮強化）待參數化
- StrategicNodeGraph（據點 adjacent）

## 調整程式時建議只改這幾個檔

- `Core/Units/AbstractUnit.cs` — 你的 C++ 欄位
- `Core/Units/ActualUnit.cs` — 與 Location 連動邏輯
- `Core/Locations/MapLocation.cs` — 進出格、起火
- `Core/Locations/LocationGrid.cs` — 整圖索引
