# 存檔 vs 劇本：兩種 JSON，不要混成一種

你不是「蠢」，是需求本來就有兩層，一開始的 `UnitDefJson` 把兩層綁在一起才容易亂。

| | **劇本（Scenario）** | **存檔（Save）** |
|---|----------------------|------------------|
| 誰寫 | 開發／企劃 | 遊戲執行後自動產生 |
| 內容 | 開局有誰、放哪格、初始兵力 | 當下士氣、傷兵、位置、池外變化 |
| 檔名例 | `StreamingAssets/scenarios/opening.json` | `saves/slot_1.json`（之後放使用者目錄） |
| 讀進遊戲 | `ScenarioJsonLoader` → `UnitDef` → `new Legion(def)` | `SaveGameLoader` → 還原 `Unit` 狀態 |
| 人類可讀 | 要（企劃改） | 可選（除錯用 JSON 即可） |

**.properties** 仍可當劇本（註解方便）；**JSON** 適合巢狀（軍團含護衛、多支部隊）。兩種劇本格式可並存。

執行期永遠：**DTO（檔案）→ `UnitDef` 或還原器 → `new Combat(def)`**，不要跳過 `Unit`。

## 建議目錄

```text
StreamingAssets/
  officers.json          # 武將圖鑑（主資料）
  scenarios/
    opening.json         # 劇本：開局部隊
  scenario_start.properties   # 可選，舊 Spike 相容

（之後）PersistentData/saves/slot_0.json
```

## 武將池 vs 圖鑑 vs 存檔

| | 武將圖鑑 | 本局 OfficerPool | 存檔 |
|---|----------|------------------|------|
| 內容 | 參數、出生年、壽命等 | 劇本進行中**仍存活且收錄**的武將狀態 | 當下 Pool 快照 + Unit 引用 |
| 誰寫 | 企劃 | 開局篩選建立，局中 mutable | 遊戲自動 |
| 細節 | [`OFFICER_CLASS.md`](OFFICER_CLASS.md) | [`SCENARIO_OFFICER_POOL.md`](SCENARIO_OFFICER_POOL.md) | 本文 + 上列文件 §7 |

## 程式進入點

- 劇本：`ScenarioJsonLoader.Load(path)` → `ScenarioUnitSpawner.CreateUnit`
- 存檔：`SaveGameSerializer.Capture` / `Apply`（見 `Data/Persistence/`）
