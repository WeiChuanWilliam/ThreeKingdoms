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
| `Combat` | 戰鬥 | 實際作戰；兵種六圍與戰法 |
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

## 3. 戰鬥力：父類分流＋子類兩個抽象方法

`Unit.CalculateCombatPower()` 內建判斷式：

```csharp
public int CalculateCombatPower()
{
    if (IsAnnihilated) return 0;
    return IsGarrison
        ? CalculateGarrisonCombatPower()      // 抽象：駐紮
        : CalculateNonGarrisonCombatPower();  // 抽象：非駐紮（野戰）
}
```

子類（`Legion`／`Combat`／`Transport`）各自 `override` 上述兩個 `protected abstract` 方法。

### 3.1 駐紮戰鬥力（依種類，意圖）

| 部隊 | 非駐紮 | 駐紮 |
|------|--------|------|
| **Transport** | 幾乎無戰力 | **偏提高防禦**（守輜重） |
| **Legion** | 0（不可野戰） | **高攻高防** |
| **Combat** | 一般戰鬥力公式 | 駐紮公式（可疊據點加成；目前先同野戰） |

### 3.2 實作落點

| 項目 | 位置 |
|------|------|
| 布林 | `Unit.IsGarrison` / `SetGarrison` |
| 分流 | `Unit.CalculateCombatPower()` |
| 子類覆寫 | `CalculateGarrisonCombatPower` / `CalculateNonGarrisonCombatPower` |
| 駐紮係數 | `unit.properties` / `UnitConfigUtil`（依 `UnitKind`） |
| 據點防禦％ | 可與 [`SETTLEMENT_SITES.md`](SETTLEMENT_SITES.md) 疊加 |

---

## 4. 各子類職責（摘要）

### `Legion`（兵團）

- 野戰出征編制；自帶軍糧
- 駐紮 → `IsGarrison`；戰鬥力走兵團駐紮（高攻高防）

### `Combat`（戰鬥）

- 作戰主體；兵種六圍、四槽戰法；1 主將 + 1 副將
- 駐紮 → 走戰鬥部隊駐紮戰鬥力

### `Transport`（運輸）

- 運糧運錢；僅計略戰法槽
- 駐紮 → 偏防禦強化

---

## 5. 相關文件

- [`LEGION_TERMINOLOGY.md`](LEGION_TERMINOLOGY.md) — 兵團／軍團用語  
- [`UNIT_AND_LOCATION.md`](UNIT_AND_LOCATION.md) — 地格、進出建築  
- [`SETTLEMENT_SITES.md`](SETTLEMENT_SITES.md) — 八種據點與駐紮防禦  
- [`UNIT_MANPOWER_AND_SKILLS.md`](UNIT_MANPOWER_AND_SKILLS.md) — 兵力與戰法  
- [`COMBAT_AND_CAMPAIGN_ARCHITECTURE.md`](COMBAT_AND_CAMPAIGN_ARCHITECTURE.md) — 戰役層  
