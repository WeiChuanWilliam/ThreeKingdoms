# 劇本武將池（OfficerPool）— 設計備忘

**狀態**：設計共識／文件階段，**尚未實作**。武將類別細節見 [`OFFICER_CLASS.md`](OFFICER_CLASS.md)。

本文補充：**Pool 與 Unit 怎麼連**、**Pool 怎麼變**、**存檔存什麼**、以及劇本開局篩選／未滿十五歲等**之後才做**的規則。

---

## 1. 範圍

| 現在要寫清楚 | 現在不做 |
|--------------|----------|
| Pool 是可變的本局武將集合 | 劇本開局完整篩選程式 |
| Unit 主將／副將直接引用 Pool | ✅ `OfficerPool.Get`（已移除 `CloneForUnit`） |
| 武將死亡 → 從 Pool 移除的 trigger | 未滿 15 歲「讀書／不可登場」算法 |
| 存檔只序列化本局 Pool | 完整 save/load 格式定稿 |

---

## 2. Pool 是什麼

```text
一個存檔 slot ＋ 一個劇本進行中
    │
    ▼
ScenarioOfficerPool（本文件簡稱 Pool）
    │
    ├── defId → Officer   （池內 Singleton：同一 id 只有一個物件）
    ├── 可新增（開局篩選後建立）
    ├── 可移除（武將死亡等）
    └── 可改狀態（傷病、個性、忠誠…全在 Officer 上改）
```

- **不是**全專案 global 一池；讀檔一／二／三各有一份。
- **不是**參數表本身；參數是唯讀圖鑑，Pool 是**這局執行時狀態**。
- Pool **必須可修改**（mutable）：人會進局、也會死、會離開可玩集合。

---

## 3. Unit 與 Pool：直接引用

每個 `Unit` 有主將、副將；兩者都**指向 Pool 裡的 `Officer` 實例**，不 Clone。

```text
ScenarioOfficerPool
    │
    ├── Officer (id=1 關羽) ◄──── Unit.A 的 Commander
    │                    ◄──── Unit.B 的 ViceOfficer   （若同一人不可重複指派，編組層檢查）
    └── Officer (id=3 諸葛亮) ◄──── …
```

| 規則 | 說明 |
|------|------|
| 引用 | `Unit.Commander` / `ViceOfficers[]` 存 `Officer` 引用或 defId + Pool 查詢 |
| 狀態一致 | 關羽在 Pool 裡生病 → 所有引用他的 Unit 讀到同一狀態 |
| 死亡 | Pool 移除該武將後，引用失效；相關 Unit 須一併處理（見 §5） |

**定案**：廢除「部隊複本改完回寫 Pool」。Pool 即唯一真相。

---

## 4. 劇本開局：誰進 Pool（之後做，先寫規則）

開新局時依**劇本年份**與武將**出生年、壽命**篩選（細節與參數表一併定稿後實作）。

```text
scenarioYear = 劇本當前年份（ fictive 例：184 ）

對參數表每位武將 def：
  alive = (birthYear <= scenarioYear) AND (scenarioYear < birthYear + lifespan)
  if alive → 建立 Officer，加入 Pool
  else     → 本局 Pool 無此 defId
```

**占位 API（虛構，未實作）**：

```csharp
// 未来：ScenarioOfficerPoolBuilder
ScenarioOfficerPool BuildForScenario(
    IReadOnlyOfficerCatalog catalog,
    int scenarioYear,
    ScenarioOfficerFilterOptions options);
```

- 現行 Spike 可暫時「表上全載進 Pool」；與本設計不一致，列在 [`OFFICER_CLASS.md` §9](OFFICER_CLASS.md) 技術債。
- **本文不 dive in 實作**；等劇本／參數討論進入實作階段再接。

---

## 5. 武將死亡：從 Pool 移除

劇本進行中若武將**死亡**（戰死、病逝、劇情等），需有**單一入口**把該武將從本局 Pool 拿掉，並觸發連帶清理。

```text
Officer.SetAlive(false)   ← 不 RefreshPerformance；不支援死而復生
    │
    ▼
OfficerPool.RemoveOfficer(defId)   ← 劇本級 method（TODO，待實作）
    │
    ├── 從 Pool dictionary 移除 Officer
    ├── 人際：對稱關係清理（OfficerRelationsSync 反向）
    ├── 若在某 Unit 任主將／副將 → 卸下或部隊解散（規則待訂）
    └── UI／勢力武將列表不再顯示此人
```

**占位 API（虛構）**：

```csharp
bool TryRemoveOfficer(int defId, OfficerRemovalReason reason);
// reason: BattleDeath, IllnessDeath, StoryEvent, …
```

- Pool 內 `IsDead` 與「不在 Pool」的關係：定案倾向 **死亡即移出 Pool**（本局不再有可操作的 Officer）；若需「陣亡紀錄」另用勢力／年表結構，不佔 Pool 槽位。
- 與開局篩選「壽命已盡未收錄」不同：那是**從未進局**；這是**進局後死亡**。

---

## 6. 未滿十五歲：在 Pool 但不可登場（之後做）

部分武將**已收錄進 Pool**（劇本年份仍存活），但**未滿 15 歲**，不能擔任主將／副將或出現在戰場。

```text
在 Pool 內
    │
    ├── CanDeploy(officer, scenarioYear) ?
    │       age = scenarioYear - birthYear
    │       age >= 15 → 可登場編組
    │       age <  15 → 不可登場（仍在 Pool，可成長／讀書等，算法之後訂）
    └── …
```

| 狀態 | Pool | 可編組上場 |
|------|------|------------|
| 劇本年已死 | 不在 | 否 |
| 存活但 &lt; 15 歲 | 在 | 否 |
| 存活且 ≥ 15 歲 | 在 | 是（另受勢力、位置等限制） |

**占位（虛構）**：

```csharp
bool OfficerDeployRules.CanAppearOnMap(Officer o, int scenarioYear);
// < 15 返回 false；讀書／內政成長另訂
```

**現階段**：只在文件記錄；武將類別設計主軸仍放在 [`OFFICER_CLASS.md`](OFFICER_CLASS.md)。

---

## 7. 存檔：只序列化本局 Pool

| 存什麼 | 不存什麼 |
|--------|----------|
| 本 slot、本劇本 **Pool 內** 各 `Officer` 執行時狀態 | 參數表全圖鑑（讀檔時仍從 StreamingAssets 載） |
| Pool 當前 defId 集合（誰還在局裡） | 已死亡且已 Remove 的武將 |
| 劇本年份、slot id 等 meta | 其他存檔 slot 的 Pool |

```text
Save slot_1.json
  scenarioId, scenarioYear, …
  officerPool: [
    { defId, stats, injury, personalities, belong, … },
    …
  ]
  units: [
    { commanderDefId, viceDefIds, … }   // 還原時向 Pool 重新綁引用
  ]
```

讀檔：還原 Pool → `Unit` 用 defId 向 Pool `Get` 綁定 Commander／Vice。

與 [`SAVE_AND_SCENARIO.md`](SAVE_AND_SCENARIO.md) 互補：該文件講 Unit 劇本 vs 存檔；本文補 **OfficerPool 屬於存檔本體**。

---

## 8. 與現行程式差距（僅列項，不改 code）

| 設計 | 現行 Spike |
|------|------------|
| 每存檔一 Pool | `OfficerPool` 靜態 + 單一 `OfficerDatabase` |
| 劇本年篩選進 Pool | 表上武將 lazy 全建 |
| Unit 引用 Pool | ✅ `OfficerPool.Get` |
| 死亡 `RemoveOfficer` | 僅 `Injury=Dead` 欄位，未移出 Pool |
| 未滿 15 不可登場 | 未做 |
| 存檔 Pool | 未完整 |

---

## 9. 建議實作順序（將來）

1. `ScenarioOfficerPool` 類別（取代靜態 Pool 的存檔歸屬）
2. Unit 改引用 + 移除 Clone
3. `RemoveOfficer` + Unit 卸下
4. 存檔／讀檔 Pool
5. 開局 `BuildForScenario(scenarioYear)` 篩選
6. `CanDeploy` / 未滿 15 歲規則

**目前**：維持武將類別與文件討論；劇本只碰概念，不 dive in coding。

---

## 10. 相關文件

- [`OFFICER_CLASS.md`](OFFICER_CLASS.md) — `AbstractOfficer` / `Officer`、Singleton、Perform、RollRandom
- [`SAVE_AND_SCENARIO.md`](SAVE_AND_SCENARIO.md) — 劇本 JSON vs 存檔 JSON
- [`SCENARIO_UNIT_PLACEMENT.md`](SCENARIO_UNIT_PLACEMENT.md) — 開局部隊放置
