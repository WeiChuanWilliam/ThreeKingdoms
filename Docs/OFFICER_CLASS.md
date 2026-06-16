# 武將類別設計（`AbstractOfficer` / `Officer`）

本文件描述**執行時武將**的 high-level 邏輯與類別邊界。武將**參數表**（欄位、數值）仍在設計中，本文不展開參數檔格式細節。

程式位置（現有 Spike／過渡實作）：

- `Assets/Scripts/Core/Officers/AbstractOfficer.cs`
- `Assets/Scripts/Core/Officers/Officer.cs`
- `Assets/Scripts/Core/Officers/OfficerPool.cs`
- `Assets/Scripts/Core/Officers/OfficerRelationsSync.cs`
- `Assets/Scripts/Data/Officers/OfficerFlag.cs`

---

## 0. High-level 邏輯（設計共識）

### 0.1 三層資料，不要混在一起

```text
① 武將參數（表／設定）          ← 你在設計，尚未定稿
        │ 讀入
        ▼
② 劇本篩選（出生年 ＋ 壽命 vs 劇本年份）  ← 死了的不進這局
        │ 通過者才建立
        ▼
③ 本局 OfficerPool（存檔／劇本內 Singleton）  ← 遊戲中只改這裡
```

| 層 | 是什麼 | 生命週期 |
|----|--------|----------|
| **參數** | 武將圖鑑、預設六圍、壽命等 | 專案資源，跨存檔共用 |
| **劇本收錄** | 某劇本某年「還活著」的武將名單 | 開新局／選劇本時算一次 |
| **Pool 內 Officer** | 這一個存檔、這一個劇本裡的執行時狀態 | **讀檔一、讀檔二、讀檔三各有一池** |

**Singleton 指的是 ③**：關羽在「讀檔一＋這個劇本」裡只有一個 `Officer`；換存檔或換劇本是**另一池**，不是全遊戲後台一個 global 武將。

（若將來有跨存檔的「後台 control／圖鑑」，那是另一套，**不在本文**；本文只講**進行中這局**的 Pool。）

### 0.2 開局：誰會進 Pool？

```text
讀武將參數
    │
    ▼
劇本當前年份 = scenarioYear
    │
    ▼
對每位武將：若 birthYear ≤ scenarioYear < birthYear + lifespan（仍存活）
    │ 是                          │ 否
    ▼                             ▼
建立 Officer 放進本局 Pool        不收录，本局不存在此 defId
```

- 已死武將：劇本中**沒有這個人**，Pool 裡查不到。
- 活著武將：進 Pool 後，內政／戰鬥／個性增減都**直接改 Pool 裡這一個實例**。

### 0.3 局內操作（全部在 Pool 上改）

| 事件 | 改什麼 |
|------|--------|
| 生病、傷勢 | `Injury` → `CalculatePerformance` → `*Perform` 下降 |
| 體力消耗 | `stamina` → `*Perform` 下降 |
| 個性獲得／替換 | `AddPersonality` / `RemovePersonality` + 新 id |
| 忠誠、歸屬、人際 | 直接改 Singleton；人際雙向由 `OfficerRelationsSync` |

**沒有**「部隊複本改完再回寫 Pool」——Pool 就是唯一真相。

部隊（`Unit`）只**引用** Pool 裡的 `Officer`（`OfficerPool.Get`，已移除 `CloneForUnit`）。

### 0.4 `RollRandom` 的設計意圖

**本質**：給定 `[min, max]` 閉區間，每次呼叫回傳一個 `int`。是武將相關**隨機判定的統一入口**，不是能力加總。

**預期用途（尚未全實作）**：

| 用途 | 概念 |
|------|------|
| 普攻波動 | 實際傷害在攻擊力附近浮動，例如 ±5% |
| 單挑／舌戰（牌制） | 抽牌、出牌結果用亂數；牌池與規則另訂 |
| 智力影響區間 | 智力越高 → 有效 **range 越小** → 結果越穩定、越偏向「好牌／高傷」 |

```text
基礎 range [min, max]
        │
        │  （未來）依智力等縮窄 effective range
        ▼
RollRandom(effectiveMin, effectiveMax)  →  int
```

現行 `Officer.RollRandom` 只做區間亂數；**智力縮 range** 留給單挑／舌戰或戰鬥公式層再包一層。

### 0.5 屬性表：仍在討論

六圍、個性槽、道具、戰法等**參數欄位尚未定稿**。類別已留欄位與 `Set*`／`RefreshPerformance` 管線；數值與表結構定案後再接參數載入即可。

### 0.6 與 Unit、存檔、Pool 變更（設計共識，未實作）

詳見 [`SCENARIO_OFFICER_POOL.md`](SCENARIO_OFFICER_POOL.md)。

| 主題 | 定案摘要 |
|------|----------|
| **Unit → Pool** | 主將／副將**直接引用** Pool 內 `Officer`，不 Clone |
| **武將死亡** | 劇本級 method 從 Pool **Remove**；本局不再可操作 |
| **存檔** | 只序列化**本局 Pool** + Unit 用 defId 還原引用 |
| **開局篩選** | 劇本年 vs 出生／壽命決定誰進 Pool；**之後做**，現僅文件 |
| **未滿 15 歲** | 可在 Pool，**不可登場**；成長／讀書算法**之後做** |

---

## 1. 設計目標（已定案）

| 目標 | 作法 |
|------|------|
| 承接舊 C++ `Officer::AbstractOfficer` 結構 | `AbstractOfficer` 集中欄位；行為差異用虛方法 |
| **本局 Pool 內每位武將只有一個實例** | `OfficerPool` 以 defId 為 key 的 **Singleton（限本存檔＋本劇本）** |
| **所有狀態變化都作用在同一實例上** | 部隊、內政、戰鬥直接改池內 `Officer`，**不複製、不回寫**（因為沒有第二份） |
| 基礎能力與實際發揮分離 | 基礎六圍（`attack` 等）＝表上能力；發揮值（`attackPerform` 等）＝經**傷病、體力**修正後的實戰數值 |
| 之後可拆玩家 AI、NPC 行為 | `Accept*`、`Defend*` 等留在抽象層，子類覆寫 |

---

## 2. Singleton 模型

### 2.1 核心原則

```text
一個存檔 ＋ 一個劇本（scenarioYear 已定）
    │
    ▼
篩選仍存活武將 → 建立本局 OfficerPool
    │
    └── defId → Officer  （本池內每 id 恰好一個實例）
```

- **不是**跨存檔 global Singleton，也**不是**「池內本尊 ＋ 部隊複本」。
- **是**本局 Pool Singleton：關羽在這個存檔的這局裡只有一個 `Officer`；當主將、副將、在野，讀寫都是它。
- 因此**不存在「回寫」**——生病、降攻、改個性，都是改 Pool 裡同一物件。

### 2.2 指派規則

```text
OfficerPool.Get(defId)          取得該武將唯一實例
        │
        ▼
Unit.SetCommander(officer)      部隊只「引用」此實例，不 Clone
Unit.AddViceOfficer(officer)    同上
```

**約束（由編組／指派系統保證，非類別自動 enforce）：**

| 規則 | 說明 |
|------|------|
| 同一 defId 不可同時在兩支部隊 | Singleton 只能有一個「當前部隊職位」 |
| 指派前檢查 | 若武將已在其他部隊任職，須先卸下或禁止重複指派 |
| 在野／在城 | 無部隊引用時，Singleton 仍存在，狀態保留 |

### 2.3 類別層次（修訂）

```text
AbstractOfficer（abstract）
    │
    ├── 欄位：姓名、六圍、發揮值、狀態旗標、歸屬、個性、人際、戰法等
    ├── 唯讀屬性：UI / 戰鬥系統讀取
    └── 虛方法：體力、傷病、交涉、防禦戰法、發揮值計算

Officer（sealed，目前唯一實作）
    │
    ├── RuntimeId：與武將表 id 相同，終身不變
    ├── Set* / StaminaChange / HealthChange：直接改 Singleton 狀態
    └── CalculatePerformance()：由基礎六圍 ＋ 傷病 ＋ 體力 → 寫入 *Perform

OfficerPool（static）
    │
    └── Get(defId) / GetShared(defId)  →  該武將在本局的唯一 Officer
```

**邊界約定**

- **表資料 → 執行時**：開局由工廠建立 Singleton 並登記進 `OfficerPool`（不在此文件展開）。
- **部隊持有方式**：`Unit.Commander`、`Unit.ViceOfficers` 持有**同一個** `Officer` 引用（`OfficerPool` 取出的實例），不是複本。

---

## 3. 生命週期

```text
新局／讀檔
    │
    ▼
依劇本年份篩選存活武將 → 建立或還原本局 OfficerPool
    │
    ▼
OfficerPool.Initialize(本局 database)
    │
    ▼
Pool 內每個 defId 一個 Officer（Singleton）
    │
    ├── 內政、登用、忠誠、傷病、個性  →  直接改 Pool 內 Officer
    ├── 指派主將／副將        →  Unit 引用 Pool 內實例（目標）
    └── 存檔                  →  序列化**本局 Pool** 狀態
```

| 操作 | 作用對象 |
|------|----------|
| `OfficerPool.Get(id)` | 唯一實例（查詢／修改皆此） |
| `unit.Commander` | 指向該實例的引用 |
| `SetStamina` / `HealthChange` / `SetInjury` | 改 Singleton → 觸發 `RefreshPerformance` |
| 卸下武將 | `Unit` 清空引用；`Officer` 仍在 Pool |

---

## 4. 能力值：基礎 vs 發揮（已定案）

### 4.1 語意

| 欄位 | 名稱 | 說明 |
|------|------|------|
| `attack` 等五維 | **基礎值** | 武將表／培養後的「素質」，不隨單場體力、輕重傷即時波動 |
| `attackPerform` 等五維 | **發揮值** | 基礎值經 **傷病**、**體力** 修正後，實際用於戰鬥／判定 |
| `stamina` | **體力** | 武將個人體力；下降時拉低 `*Perform`；與部隊 `Unit.Stamina` 是不同欄位 |

```text
基礎值（attack, leadership, …）
        │
        │  CalculatePerformance()
        │    ├── 傷病修正（Injury：正常／輕傷／重傷）
        │    └── 體力修正（stamina 高低）
        ▼
發揮值（attackPerform, leadershipPerform, …）
        │
        ▼
戰鬥讀取（OfficerCombatAbilities.FromOfficer）
```

### 4.2 計算入口（單一）

所有「傷病、體力 → 發揮值」的邏輯**只**放在 `CalculatePerformance()`（與其觸發點 `RefreshPerformance()`）：

**應觸發 Refresh 的時機：**

- `SetStats`（基礎六圍變了）
- `SetStamina`（體力變了）
- `HealthChange` / `SetInjury` / `SetAlive`（傷勢或死亡變了）
- `AddItemId` / `RemoveItemId`（道具變了，待 ItemCatalog 接上）
- 讀檔還原後

**`CalculatePerformance()`**：目前**留空**（TODO）；完成後在此計算五維 `*Perform`。暫存參考公式見 `OfficerPerformanceRules`：

**目標公式**（待實作於 `CalculatePerformance`）：

```text
perform = round( base × 傷勢係數 × 體力係數 × 道具係數 )

傷勢：正常 1.0｜輕傷 0.9｜中傷 0.75｜重傷 0.55
體力：0→×0.5，100→×1.0（線性）
道具：目前 stub ×1.0（TODO: ItemCatalog）
死亡（IsAlive=false）：perform = 0
```

- 基礎六圍、發揮值皆 **`byte`**，設計區間 **1～100**；體力 **0～100**。
- 傷勢 **1～3** 與 **死亡（`IsAlive`）分離**；`HealthChange` 只升降傷勢，不直接設死亡。

### 4.3 戰鬥讀取與 `OfficerCombatAbilities`（白話）

戰鬥不應直接散落讀 `officer.Attack` 或 `officer.AttackPerform`，而是走 **`OfficerCombatAbilities.FromOfficer(officer)`**，把「該讀哪個數」集中在一處。

`FromOfficer` 內部用 `PickPerform(perform, baseStat)`：

```text
若 attackPerform > 0  →  戰鬥用 attackPerform（已反映傷病、體力）
若 attackPerform == 0 →  才退回 attack（基礎值；防禦用，正常不應發生）
```

**白話**：關羽表上武力 97（`attack`），重傷又疲勞後可能只剩 70（`attackPerform`）。戰鬥算傷害應讀 70，不是 97。`OfficerCombatAbilities` 就是幫你自動選「該用發揮值還是基礎值」的薄封裝，避免每個戰鬥公式重複寫 if。

程式位置：`Assets/Scripts/Core/Units/OfficerCombatAbilities.cs` 的 `FromOfficer` / `PickPerform`。

統率部隊加成（`CombatStatMath`）目前仍用主將 `Leadership`（基礎統率）；是否改為 `LeadershipPerform` 待戰鬥規則一併調整（見 §10.3）。

---

## 5. 欄位分組

### 5.1 身分與演出

| 欄位 | 說明 |
|------|------|
| `lastName` / `firstName` / `aliasName` | 姓、名、字 |
| `FullName` / `DisplayName` | 顯示用組合字串 |
| `biography` | 列傳 |
| `tone` / `voice` / `picture` | 演出資源 |
| `pictureBuffer` | 執行時快取 |

### 5.2 歸屬與生涯

| 欄位 | 說明 |
|------|------|
| `belong` | 勢力 id；`0` = 在野 |
| `loyalty` | 忠誠 0～100 |
| `salary` | 俸祿 |
| `title` | 官職 |
| `birthYear` / `lifespan` / `DeathYear` | 出生、壽命、卒年 |
| `compatibility` | 相性 |
| `troopAptitude` | 五大兵科適性 |

### 5.3 勢力、兵團主將與 `OfficerFlag`

| 欄位 | 說明 |
|------|------|
| `belong` | **勢力** id；`0` = 在野 |
| `legionLeaderId` | **兵團（Legion）主將** defId（Leader）；自領兵團＝自身 id；見 [`LEGION_TERMINOLOGY.md`](LEGION_TERMINOLOGY.md) |
| `officerFlag.Show` | 登場／可見狀態（見下表） |

**Legion＝兵團**（出征部隊），**軍團**留給後方城市管理（英文待定）。

```text
OfficerFlag
├── Injury     0 無傷｜1 輕傷｜2 中傷｜3 重傷 → 影響 *Perform
├── IsAlive    true 存活｜false 死亡（與傷勢分離）
├── Show       0 未登場｜1 隱藏｜2 在野可見｜3 已歸屬
└── Gender     男／女
```

**Show 語意**：

| 值 | 說明 |
|----|------|
| 0 | 尚未登場（未滿 15 歲，或劇本尚未放出） |
| 1 | 已登場但隱藏，需探索人才後才可見 |
| 2 | 在野可見，可登用 |
| 3 | 已歸屬勢力 |

- `IsDead`：`!IsAlive`
- `IsBelonged`：`belong != 0` 且 `Show == Belonged`

### 5.4 個性：單一 `HashSet<PersonalityDef>`

`PersonalityDef` 實作 `Equals`/`GetHashCode` **只看 `Id`**（同 Java 以 id 為 Set 鍵）。

- 一個 Set 即可：含 id、key、顯示名；`Contains`／`Remove` 用 id
- 查總庫：`PersonalityDatabase.GetById(id)` 取表定義；身上 Set 存執行時快照
- `HasPersonalityId(id)`、`AddPersonality`、`RemovePersonality(id)`

### 5.5 道具：單一 `HashSet<int>`

只存道具 **id**；數量／耐久等若需要再以 `ItemInstance` + id 相等擴充。目前無 `List` 第二份。

### 5.6 兵科適性

`TroopAptitudeGrade`：**0＝C … 3＝S**（數字越大越好）。

### 5.7 人際關係（已定案：雙向對稱）

人際以 **def id 列表** 存在 Singleton 上，不持有 `Officer` 物件引用。

| 類型 | 雙向規則 |
|------|----------|
| 親愛 `belovedOfficerIds` | A 親愛 B ⇒ B 也親愛 A |
| 義兄弟 `swornBrotherIds` | A、B 為義兄弟 ⇒ 雙方列表都含對方 id |
| 配偶 `spouseOfficerIds` | A 配偶 B ⇒ B 配偶 A |
| 厭惡 `hatedOfficerIds` | A 厭惡 B ⇒ B 也厭惡 A |

**實作**：`Officer.SetRelations` → `OfficerRelationsSync.Apply`；表載入後 `OfficerDatabase.SyncAllRelations()` 補齊對向 id。

### 5.8 戰法

`battleSkills`：六槽（盾／騎／槍／弓／水／器），各 `SkillId`。與兵科適性分開；適性管能帶什麼兵，戰法槽管技能。

### 5.9 C++ 遺留虛方法（說明）

| 方法 | 用途 | 現況 |
|------|------|------|
| `AcceptOffer` | 登用／勸降時，武將是否接受對方招攬 | stub `false` |
| `AcceptFight` | 是否接受單挑挑戰 | stub；未來牌制單挑 |
| `AcceptDebate` | 是否接受舌戰 | stub；未來牌制舌戰 |
| `DefendCavalrySkill` 等 ×4 | 遭敵方該系戰法攻擊時，是否發動**防禦戰法**反制 | stub；戰鬥 AI |

繼承自舊 C++ `AbstractOfficer`；之後可改 `IOfficerBehavior` 或子類覆寫。

---

## 6. 存取規則

```text
讀取  →  AbstractOfficer 公開唯讀屬性
寫入  →  Officer 的 Set* / StaminaChange / HealthChange（直接改 Singleton）
戰鬥  →  讀 *Perform；改體力／傷病後必須 RefreshPerformance
```

`AbstractOfficer` 不暴露 setter，避免繞過 clamp 與 Refresh 時機。

---

## 7. 與部隊（`Unit`）的關係

```text
Unit
├── Commander: Officer     → 引用 OfficerPool 的 Singleton
└── ViceOfficers: List<Officer>  → 同上（不同 defId）

指派（目標 API）：
  SetCommanderFromPool(defId)  →  Get(defId) → SetCommander
  AddViceOfficerFromPool(defId) →  Get(defId) → AddViceOfficer（須通過「未在其他部隊」檢查）
```

主副將能力混合（`OfficerCombatAbilities.BlendCommanderAndVice`）仍用各將的 **發揮值** 合算。

---

## 8. 抽象方法與目前實作

| 方法 | 設計意圖 | 目標行為 |
|------|----------|----------|
| `StaminaChange` | 體力增減 | 改 `stamina` → **RefreshPerformance** |
| `HealthChange` | 傷病升降 | 改 `Injury` → **RefreshPerformance** |
| `CalculatePerformance` | 基礎 → 發揮 | 傷病 ＋ 體力修正 |
| `Accept*` / `Defend*` | AI／玩家決策 | 待子類或行為策略 |
| `RollRandom(min, max)` | 區間內整數亂數 | ✅ `Random.Shared` |

### 8.1 `RollRandom`（已定案）

```csharp
public abstract int RollRandom(int minInclusive, int maxInclusive);
```

武將參與需擲隨機的判定時使用；`min > max` 時自動交換上下界。

**設計延伸（§0.4）**：戰鬥波動、單挑／舌戰抽牌；高智力可縮小 effective range 後再呼叫。現行實作僅區間均勻亂數。

---

## 9. 與現行程式的差異（待重構）

目前程式仍為 **Clone 模型**，與本文件定案不一致：

| 現況 | 應改為 |
|------|--------|
| `OfficerPool` 靜態全域、表上武將全載 | **每存檔一池**；依 **劇本年份＋壽命** 篩選存活者才進 Pool |
| ~~`CloneForUnit`~~ | **已移除**；`Unit` 用 `OfficerPool.Get` 直接引用 |
| `Unit.SetCommanderFromPool` 內 Clone | `OfficerPool.GetShared(defId)` |
| `CalculatePerformance` 1:1 | 實作傷病、體力係數 |
| `StaminaChange` / `HealthChange` 未 Refresh | 結尾 `RefreshPerformance()` |
| 無重複指派檢查 | 編組層 enforce |
| `RollRandom` 僅均勻亂數 | 戰鬥／牌戰外層依智力等縮 range 後再呼叫 |

重構完成前，請以**本文件**為準進行新功能；舊 Clone 路徑視為技術債。

---

## 10. 仍待討論

### 10.1 子類 vs `IOfficerBehavior`

`Accept*`、`Defend*` 放子類或外掛行為策略。

### 10.2 道具／個性 buff

併入 `CalculatePerformance`（Singleton 狀態一併反映），或戰鬥回合內暫時疊算？

### 10.3 統率加成用基礎還是發揮

`CombatStatMath.GetLeadershipFactor` 用 `Leadership` 或 `LeadershipPerform`。

### 10.4 副將上限

在 `Unit` 或 UI 規則層限制。

---

## 11. 建議實作順序

1. **Singleton 指派**：`Unit` 改引用 Pool；廢除 Clone 路徑
2. **`CalculatePerformance`**：傷病 ＋ 體力公式；`StaminaChange` / `HealthChange` 後 Refresh
3. **指派衝突檢查**：同一 defId 不可多部隊
4. 戰鬥／統率讀取統一改 `*Perform`
5. 存檔：只存 Pool 內 Officer 列表 + 各部隊 commander/vice **defId**（還原時重新綁引用）

---

## 附：快速對照表

| 我想… | 用… |
|-------|-----|
| 取得武將（唯一實例） | `OfficerPool.GetShared(defId)` |
| 讀實戰武力 | `officer.AttackPerform` |
| 讀表上武力 | `officer.Attack` |
| 體力變化後更新發揮 | `officer.StaminaChange(delta)`（內部 Refresh） |
| 派主將 | `unit.SetCommanderFromPool(defId)` → 綁定 Singleton |
| 戰鬥合算五維 | `OfficerCombatAbilities.FromOfficer(officer)` |
