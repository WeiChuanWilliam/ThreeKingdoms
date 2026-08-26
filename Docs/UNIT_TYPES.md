# Unit 繼承結構（父類 + 三子類）

**狀態**：設計定案（本文件為準）。

用語：`Legion`＝**兵團**（不是「軍團」）。後方城市委任代管另稱「軍團」，見 [`LEGION_TERMINOLOGY.md`](LEGION_TERMINOLOGY.md)。

---

## 1. 四個類別（父類 + 三種部隊）

```text
Unit（父類，abstract）
├── Legion      兵團
├── Combat      戰鬥部隊
└── Transport   運輸部隊
```

對應 C++：`Unit::AbstractUnit` → C# **`Unit`**。  
地圖上直接使用上述三種子類；變數型別可宣告為 `Unit`。

| `UnitKind` | 中文 | 角色 |
|------------|------|------|
| `Legion` | 兵團 | 出征編制；自帶糧薪；隸屬武將／戰鬥部隊 |
| `Combat` | 戰鬥 | 實際作戰；`Combat.Stats`（攻防等）與兵種表 `Troop*` |
| `Transport` | 運輸 | 運補；計略戰法 |

**沒有**獨立的「駐紮部隊」子類。駐紮是父類上的**狀態**（見 §2）。

`UnitLocationBinding` 只負責格子連動，不是部隊種類。

---

## 2. 駐紮＝`Unit.IsGarrison` 布林

- 欄位：`Unit.IsGarrison`（進建築／可駐紮據點 → `true`；離營出征 → `false`）
- 設定：`Unit.SetGarrison(bool)`
- **所有部隊**進入建築／據點一律進入駐紮狀態（三種 `UnitKind` 皆同）
- 駐紮中通常不可野戰移動；耗糧等細節見 [`SETTLEMENT_SITES.md`](SETTLEMENT_SITES.md)、[`UNIT_AND_LOCATION.md`](UNIT_AND_LOCATION.md)

```text
Combat / Legion / Transport
        │ 進入建築 → IsGarrison = true
        │ 出征離營 → IsGarrison = false
        ▼
   仍是同一種子類（不換類）
```

---

## 3. 戰鬥力：`CombatStatMath`（尚未設計公式）

部隊能力只走 `CombatStatMath`：

1. 類型：`Combat`／`Transport`／`Legion`
2. 駐紮：僅 Combat、Legion 再分；Transport 不看駐紮

目前只有**非駐紮 Combat** 有暫代公式；其餘回 1。交戰規則在 `CombatBattleFormulas`（空殼）。

### 3.1 駐紮意圖（公式未寫）

| 部隊 | 非駐紮 | 駐紮 |
|------|--------|------|
| **Transport** | 幾乎無戰力（待設計） | 不走駐紮分支 |
| **Legion** | 待設計 | 待設計 |
| **Combat** | `CalculateCombatField`（暫代） | 待設計 |

### 3.2 實作落點

| 項目 | 位置 |
|------|------|
| 布林 | `Unit.IsGarrison` / `SetGarrison` |
| 能力計算 | `CombatStatMath` → `Combat.Stats` |
| 交戰 | `CombatBattleFormulas` |
| 據點類型 | [`SETTLEMENT_SITES.md`](SETTLEMENT_SITES.md)（加成公式未接） |

---

## 4. 各子類職責（摘要）

### `Legion`（兵團）

- 野戰出征編制；自帶軍糧
- 駐紮 → `IsGarrison`；能力走 `CombatStatMath` 兵團分支（公式未寫）

### `Combat`（戰鬥）

- 作戰主體；最終屬性 `Stats`（`CombatTroopStatBlock`）、兵種表 `Troop*`；1 主將 + 1 副將
- 駐紮 → `IsGarrison`；`CombatStatMath.CalculateCombatGarrison`（公式未寫）

### `Transport`（運輸）

- 運糧運錢；不看駐紮（`CombatStatMath.CalculateTransport`，公式未寫）

---

## 5. 相關文件

- [`LEGION_TERMINOLOGY.md`](LEGION_TERMINOLOGY.md) — 兵團／軍團用語  
- [`UNIT_AND_LOCATION.md`](UNIT_AND_LOCATION.md) — 地格、進出建築  
- [`SETTLEMENT_SITES.md`](SETTLEMENT_SITES.md) — 八種據點與駐紮防禦  
- [`UNIT_MANPOWER_AND_SKILLS.md`](UNIT_MANPOWER_AND_SKILLS.md) — 兵力與戰法  
- [`COMBAT_AND_CAMPAIGN_ARCHITECTURE.md`](COMBAT_AND_CAMPAIGN_ARCHITECTURE.md) — 戰役層  
