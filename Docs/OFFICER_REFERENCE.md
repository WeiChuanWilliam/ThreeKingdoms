# 武將參數與公式速查

**用途**：檢查 class 欄位、參數檔、公式實作狀態。設計共識見 [`OFFICER_CLASS.md`](OFFICER_CLASS.md)。

**參數檔**

| 檔案 | 路徑 |
|------|------|
| 武將實例 | `Assets/StreamingAssets/officers.json` |
| 系統常數 | `Assets/StreamingAssets/chinese/officer.properties` |
| 個性定義 | `Assets/StreamingAssets/personality_traits.json` |

**Local 測試輸出**

```powershell
cd local-tests
.\run-tests.ps1
# 或
dotnet test
```

會印出每名武將：`統93(93) 武97(97)…`（括號外＝發揮值，括號內＝表上基礎值）。

---

## 1. 類別結構

```text
AbstractOfficer（abstract）
    欄位 + 唯讀屬性 + 抽象方法
Officer（sealed）
    Set* 修改 + CalculatePerformance 實作
OfficerPool（static）
    Get(defId) → 本局 Singleton
OfficerDef（JSON 表）
OfficerFactory.FromDef → Officer
OfficerPerformanceRules（靜態公式）
OfficerCombatAbilities.FromOfficer（戰鬥讀取）
```

---

## 2. 參數：officers.json（每位武將）

| 欄位 | 型別 | 說明 | 執行時對應 |
|------|------|------|------------|
| `id` | int | 武將 defId | `Officer.RuntimeId` |
| `lastName` / `firstName` / `aliasName` | string | 姓、名、字 | `SetName` |
| `biography` | string | 列傳 | `Biography` |
| `tone` / `voice` / `picture` | string | 演出 | `SetPresentation` |
| `leadership` … `charisma` | short | 五維基礎 1～100 | `Attack` … `Charisma` |
| `stamina` | short | 體力 0～100 | `Stamina` |
| `gender` | byte | 0 男、1 女 | `OfficerFlag.Gender` |
| `loyalty` / `belong` | short | 忠誠、勢力 | `SetBelong` |
| `injury` | byte | 0～3 傷勢 | `SetInjury` |
| `compatibility` | byte | 相性 | `Compatibility` |
| `troopAptitude` | object | 步騎弓器水 0=C…3=S | `TroopAptitude` |
| `personalityIds` | int[] | 個性 id | `Personalities` |
| `itemIds` | int[] | 道具 id | `ItemIds` |
| `relations` | object | 人際四表 | `SetRelations` |
| `birthYear` / `lifespan` | short | 出生、壽命 | `DeathYear` = 和 |
| `title` | string | 官職 | `Title` |
| `battleSkills` | object | 六槽戰法 | `BattleSkills` |

**執行時另有、JSON 可後補**：`legionLeaderId`（兵團主將）、`OfficerFlag.Show`（登場狀態）、`salary`。

---

## 3. 參數：officer.properties（系統常數）

| 鍵 | 預設 | 用途 |
|----|------|------|
| `officer.stamina.default` | 100 | 新建武將體力 |
| `officer.compatibility.default` | 145 | 預設相性 |
| `officer.lifespan.default` | 60 | 未填 lifespan 時 |
| `officer.aptitude.default` | 0 (C) | 兵科預設 |
| `officer.personality.*_max` | 各槽上限 | 個性合成 |
| `officer.item.max_count` | 12 | 道具 id 上限 |
| `officer.relations.*_max` | 親愛/義兄弟/配偶 | 人際上限 |
| `officer.signature_troop.*` | 兵種鍵,科技 | 特色兵種條件 |

讀取：`OfficerConfigUtil.Load(path)` → `GetDefaultStamina()` 等。

---

## 4. AbstractOfficer 欄位一覽

### 4.1 身分與演出

| 屬性 | 說明 |
|------|------|
| `LastName` / `FirstName` / `AliasName` | 姓名 |
| `FullName` / `DisplayName` | 顯示用 |
| `Biography` | 列傳 |
| `Tone` / `Voice` / `Picture` | 演出資源 |
| `PictureBuffer` | 頭像快取 |

### 4.2 能力（基礎 vs 發揮）

| 基礎 (byte 1～100) | 發揮 (*Perform) |
|--------------------|-----------------|
| `Leadership` | `LeadershipPerform` |
| `Attack` | `AttackPerform` |
| `Intelligence` | `IntelligencePerform` |
| `Policy` | `PolicyPerform` |
| `Charisma` | `CharismaPerform` |
| `Stamina` | （體力本身參與公式，非 Perform） |

### 4.3 狀態與生涯

| 屬性 | 說明 |
|------|------|
| `OfficerFlag` | Injury, Show, Gender, IsAlive |
| `Belong` | 勢力 id；0＝在野 |
| `LegionLeaderId` | 兵團主將 defId |
| `Loyalty` / `Salary` / `Title` | 忠誠、俸祿、官職 |
| `BirthYear` / `Lifespan` / `DeathYear` | 生涯 |
| `Compatibility` | 相性 |
| `TroopAptitude` / `GetTroopAptitude` | 兵科適性 |
| `Personalities` / `ItemIds` | 個性、道具 |
| 人際四表 | Beloved, Hated, SwornBrother, Spouse |
| `BattleSkills` | 戰法六槽 |

---

## 5. 公式（實作狀態）

### 5.1 發揮值 *Perform — ✅ 已實作

入口：`Officer.CalculatePerformance()` ← `RefreshPerformance()`  
（`SetStats` / `SetStamina` / `SetInjury` / `HealthChange` 後觸發）

```text
perform = round( base × 傷勢係數 × 體力係數 × 道具係數 )

傷勢：正常 1.0｜輕 0.9｜中 0.75｜重 0.55
體力：0 → ×0.5，100 → ×1.0（線性）
道具：目前 ×1.0（TODO: ItemCatalog）
死亡：perform = 0
```

程式：`OfficerPerformanceRules.ComputePerform`  
常數：`StatMin=1`, `StatMax=100`, `StaminaMin=0`, `StaminaMax=100`

### 5.2 戰鬥讀取五維 — ✅

`OfficerCombatAbilities.FromOfficer(officer)`  
→ 優先 `*Perform`，若 Perform==0 才退回基礎值  
主副將合算：`BlendCommanderAndVice`（主×2 + 副）/ 3

### 5.3 亂數 — ✅

`Officer.RollRandom(min, max)` — 均勻整數；智力縮 range **未做**

### 5.4 抽象方法 — ❌ stub（回 false）

| 方法 | 設計用途 |
|------|----------|
| `AcceptOffer` | 登用／勸降 |
| `AcceptFight` | 是否接單挑 |
| `AcceptDebate` | 是否接舌戰 |
| `DefendCavalrySkill` 等 ×4 | 防禦戰法反制 |

### 5.5 尚未實作

| 項目 | 說明 |
|------|------|
| `OfficerPool.RemoveOfficer` | 死亡移出池 |
| 劇本開局篩選 | birthYear + lifespan vs 劇本年 |
| 未滿 15 不可登場 | `CanDeploy` |
| 同一 defId 不可多部隊 | 編組檢查 |
| 道具係數 | `GetItemMultiplier` TODO |
| 個性影響 Perform | 待併入或戰鬥回合 |
| 存檔 Pool 完整序列化 | 部分有 Unit 快照 |
| `StaminaChange` | 文件曾提，目前用 `SetStamina` |

---

## 6. Officer 主要 Set* 方法

| 方法 | 觸發 RefreshPerformance |
|------|-------------------------|
| `SetStats` | ✅（含可選 SetStamina） |
| `SetStamina` | ✅ |
| `SetInjury` / `HealthChange` | ✅ |
| `SetAlive(false)` | ❌（不 Refresh；TODO RemoveOfficer） |
| `SetBelong` / `SetName` 等 | 多數不觸發 |

---

## 7. 相關文件

- [`OFFICER_CLASS.md`](OFFICER_CLASS.md) — Singleton、Pool、設計共識  
- [`SCENARIO_OFFICER_POOL.md`](SCENARIO_OFFICER_POOL.md) — 開局篩選、死亡、存檔  
- [`Assets/Scripts/README_OFFICER.md`](../Assets/Scripts/README_OFFICER.md) — C++ 對照（若存在）
