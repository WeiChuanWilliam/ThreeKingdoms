# 據點與駐紮（Settlement Sites）

**狀態**：類型與駐紮防禦加成定案；建造消耗、耐久待參數表。  
程式：`SettlementSiteKind`、`SettlementSiteRules`、`StationedCombatRules`、`Unit.IsStationed`。

---

## 1. 共通規則

- **駐紮** = `Unit.IsStationed == true`；部隊仍是 `Combat` 或 `Legion`，不換類別。
- 進入有據點的格子 → `StationRules.TryAutoStation` 自動駐紮。
- 下列 **八種** 據點皆可駐紮。

---

## 2. 地圖內建（4 種）

劇本／地圖編輯放置；玩家不能「新建」此類（只能占領、修築內部設施等，另訂）。

| 類型 | `SettlementSiteKind` | 說明 |
|------|----------------------|------|
| **城池** | `City` | 大都會、州治／郡治核心 |
| **縣城** | `CountyTown` | 縣級據點 |
| **港灣** | `Harbor` | 水軍、渡江節點（見 `NAVY_AND_RIVER_MOVEMENT.md`） |
| **關口** | `Pass` | 名關、關隘（虎牢、劍閣等） |

地名清單見 [`THREE_KINGDOMS_REGIONS.md`](THREE_KINGDOMS_REGIONS.md)。

---

## 3. 玩家可建造（4 種）

野戰格上由玩家部隊建造；建造後可駐紮。

| 類型 | `SettlementSiteKind` | 定位 |
|------|----------------------|------|
| **岩砦** | `RockFort` | 造價 **高**、防禦 **強** |
| **要塞** | `Fortress` | 造價 **高**、防禦 **強** |
| **陣** | `Camp` | 造價 **低**、建造 **快**、防禦 **弱** |
| **寨** | `Stockade` | 造價 **低**、建造 **快**、防禦 **弱** |

```text
        防禦強 ── 岩砦、要塞（貴、慢）
        防禦弱 ── 陣、寨（便宜、快）
```

**未實作**：建造指令、工期、金錢／兵力消耗、耐久、被攻城公式。

---

## 4. 與兵團／戰鬥隊

| 部隊 | 野戰 | 駐紮於上表任一地點 |
|------|------|-------------------|
| **Combat** | 正常作戰 | 可駐紮；耗糧為 0 |
| **Legion** | **不可**野戰（戰力 0） | 駐紮後可正常作戰 |

見 [`UNIT_AND_LOCATION.md`](UNIT_AND_LOCATION.md) 駐紮一節。

---

## 5. 程式對照

| 檢查 | API |
|------|-----|
| 可駐紮？ | `SettlementSiteRules.IsStationable(kind)` |
| 地圖內建？ | `SettlementSiteRules.IsMapPlaced(kind)` |
| 玩家建造？ | `SettlementSiteRules.IsPlayerBuilt(kind)` |
| 高防建造？ | `SettlementSiteRules.IsHighDefenseBuilt`（岩砦、要塞） |
| 低防建造？ | `SettlementSiteRules.IsLowDefenseBuilt`（陣、寨） |
| 駐紮防禦加成％ | `SettlementSiteRules.GetStationedDefenseBonusPercent(kind)` |
| 駐紮是否用面向 | `StationedCombatRules.UsesFacingVector(unit)` → 駐紮為 false |

---

## 7. 駐紮戰鬥：無向量 ＋ 據點防禦加成

### 7.1 駐紮時沒有「面向／向量」

部隊 **`IsStationed == true`** 時：

- **不套用** 野戰 Facing（正面全防／背面半防）。
- 不會因被從背後攻擊而降低攻擊或防禦。
- 等同據點內駐防，四面視為依托工事，而非野戰隊形。

野戰移動、塗格、面向規則僅在 **`IsStationed == false`** 時生效。

### 7.2 據點防禦加成（全軍防禦率）

駐紮於據點內時，**整支部隊防禦** 加上表內百分比（攻擊不加；攻擊仍受其他規則影響）。

| 順序 | 據點 | `SettlementSiteKind` | 防禦加成 |
|:----:|------|----------------------|:--------:|
| 1（最弱） | **陣** | `Camp` | **+5%** |
| 2 | **寨** | `Stockade` | **+10%** |
| 3 | **岩砦** | `RockFort` | **+15%** |
| 4 | **縣城** | `CountyTown` | **+20%** |
| 5 | **要塞** | `Fortress` | **+25%** |
| 5 | **港灣** | `Harbor` | **+25%** |
| 6（最強） | **關口** | `Pass` | **+30%** |
| 6 | **城池** | `City` | **+30%** |

```text
陣 +5%  <  寨 +10%  <  岩砦 +15%  <  縣城 +20%
        <  要塞／港灣 +25%  <  關口／城池 +30%
```

**公式（占位，接入戰鬥時用）：**

```text
effectiveDefense = baseDefense × (1 + 據點加成% / 100) × …其他修正
```

程式：

```csharp
SettlementSiteRules.GetStationedDefenseBonusPercent(siteKind);  // 5～30
SettlementSiteRules.GetStationedDefenseMultiplier(siteKind);    // 1.05～1.30
StationedCombatRules.GetDefenseMultiplier(unit);                // 未駐紮 → 1.0
StationedCombatRules.UsesFacingVector(unit);                    // 駐紮 → false
```

**未實作**：與 `CombatStatMath`、Facing 傷害結算的實際串接。

---

## 8. 舊 enum 對照（遷移備忘）

| 舊值 | 新值 |
|------|------|
| `SmallTown`（小城） | `CountyTown`（縣城） |
| `Fort`（砦） | 地圖關口 → `Pass`；玩家岩砦 → `RockFort` |
| `Camp`（陣） | 仍為 `Camp`，數值改為 12 |
