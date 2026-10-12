# 據點與駐紮（Settlement Sites）

**狀態**：類型與駐紮防禦加成定案；佔地、駐紮兵團、攻城流程方向定案（§6，尚未實作）；建造消耗、耐久數值待參數表。  
程式：`SettlementSiteKind`、`SettlementSiteRules`、`StationRules`、`Unit.IsGarrison`。

---

## 1. 共通規則

- **駐紮** = `Unit.IsGarrison == true`；部隊仍是 `Combat`／`Legion`／`Transport`，**不換類別**。
- 進入有據點的格子 → `StationRules.TryAutoStation` 自動駐紮。
- 下列 **八種** 據點皆可駐紮。
- 駐紮時戰鬥力走 `CombatStatMath` 的駐紮分支（公式未寫）— 見 [`UNIT_TYPES.md`](UNIT_TYPES.md)。

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

**能不能蓋**看地形旗標（見 [`TERRAIN.md`](TERRAIN.md)）：

| 旗標 | 意義 | 對應據點 |
|------|------|----------|
| `Construction`（可建） | 能蓋普通建物 | 陣、寨 |
| `Fort`（要塞） | 能蓋花大量金錢、時間的建物 | 要塞、岩砦 |

**未實作**：建造指令、工期、金錢／兵力消耗、耐久數值、被攻城公式（流程見 §6）。

---

## 4. 與兵團／戰鬥／運輸

| 部隊 | 野戰 | 駐紮（`IsGarrison`） |
|------|------|----------------------|
| **Combat** | 一般戰鬥力 | 駐紮戰鬥力（攻防強化，參數另訂） |
| **Legion** | 一般戰鬥力（既有規則） | 駐紮戰鬥力：**高攻高防** |
| **Transport** | 一般戰鬥力（既有規則） | 駐紮戰鬥力：**偏提高防禦** |

完整規則：[`UNIT_TYPES.md`](UNIT_TYPES.md)。地格銜接：[`UNIT_AND_LOCATION.md`](UNIT_AND_LOCATION.md)。

---

## 5. 程式對照

| 檢查 | API |
|------|-----|
| 可駐紮？ | `SettlementSiteRules.IsStationable(kind)` |
| 地圖內建？ | `SettlementSiteRules.IsMapPlaced(kind)` |
| 玩家建造？ | `SettlementSiteRules.IsPlayerBuilt(kind)` |
| 高防建造？ | `SettlementSiteRules.IsHighDefenseBuilt`（岩砦、要塞） |
| 低防建造？ | `SettlementSiteRules.IsLowDefenseBuilt`（陣、寨） |
| 駐紮防禦加成％ | `SettlementSiteRules.GetStationedDefenseBonusPercent(kind)`（骨架，未接戰鬥） |

> 現況：`SettlementSiteRules` 全部方法仍是骨架（回傳 false／0），所以 `TryAutoStation` 目前不會真的讓部隊駐紮。

---

## 6. 佔地、駐紮兵團與攻城

> 狀態：**方向定案，尚未實作**。

### 6.1 佔地（多格建築）

- 每個據點有一個 **中心格**（`CenterHex`，地圖上標示的那格），另有 **佔用格清單**（`Footprint`）。
- 佔用格的 Location 全部指向**同一個建築實例**：從任一格可找到建築，從建築可找到所有格。
- 例：**關口佔 4 格成一直線**。
- 佔用格的地形：

| 地形 | 據點 | 著火 |
|------|------|------|
| 城池 `City` | 城池、關口（關隘）、要塞、岩砦 | 較不易著火（火 n＝1） |
| 城寨 `Stockade` | 港灣、縣城、陣、寨 | 較易著火（火 n＝2） |

- **玩家建造**的據點：建好時佔用格地形改成城池／城寨；建築記住每格**原本的地形**，拆除時還原。地圖內建據點不會被拆，無此問題。

### 6.2 駐紮兵團

- **城池自動有兵團駐紮**；兵團站在**中心格**。
- 駐紮的兵力**屬於兵團**。

### 6.3 出兵

- 出兵從**中心格**開始移動。
- 城池／城寨地形離格消耗為 0，所以穿過城內佔用格不花行動力，出了城才依外面地形扣。

### 6.4 攻城

```text
攻擊任一佔用格 → 找到所屬建築 → 先扣耐久 → 再算駐紮兵團傷害
耐久歸零 → 攻擊部隊自動進入中心格，轉為兵團、駐紮
```

- 耐久歸零前，敵軍**不能進入**任何佔用格，只能從周圍攻擊。
- 所以關口在被攻下前，敵軍無法穿越。

### 6.5 待決定

- **出兵部隊放哪**：中心格已有駐紮兵團，而目前一格只記一支部隊。
- **耐久歸零時原守軍怎麼處理**：要先打光，還是潰散／撤退。
- **多支部隊攻城時誰進城**，以及戰鬥部隊轉兵團時武將、兵力、傷兵怎麼帶（技術上可用 `UnitRegistry.Replace`）。
- **命名衝突**：地形 `Stockade`（城寨）與據點 `SettlementSiteKind.Stockade`（寨）同名不同義，建議地形改名。

---

## 7. 駐紮戰鬥：無向量 ＋ 據點防禦加成

### 7.1 駐紮時沒有「面向／向量」

部隊 **`IsGarrison == true`** 時：

- **不套用** 野戰 Facing（正面全防／背面半防）。
- 不會因被從背後攻擊而降低攻擊或防禦。
- 等同據點內駐防，四面視為依托工事，而非野戰隊形。

野戰移動、塗格、面向規則僅在 **`IsGarrison == false`** 時生效。

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
SettlementSiteRules.GetStationedDefenseBonusPercent(siteKind);  // 骨架 0，未接戰鬥
SettlementSiteRules.GetStationedDefenseMultiplier(siteKind);    // 骨架 1.0
```

**未實作**：據點加成尚未接入 `CombatStatMath`／`CombatBattleFormulas`。

---

## 8. 舊 enum 對照（遷移備忘）

| 舊值 | 新值 |
|------|------|
| `SmallTown`（小城） | `CountyTown`（縣城） |
| `Fort`（砦） | 地圖關口 → `Pass`；玩家岩砦 → `RockFort` |
| `Camp`（陣） | 仍為 `Camp`，數值改為 12 |
