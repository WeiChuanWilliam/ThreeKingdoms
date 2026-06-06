# 開局部隊放置（.properties）

與「玩家 UI 組隊」不同：這是**關卡／開局配置**，給人類手改。

## 建構鏈（`new Combat` 時發生什麼）

```text
CombatUnitDef def          ← UI 填 或 ScenarioUnitSpawner 從 .properties 填
        │
        ▼
new Combat(def)
        │
        ├─① base(UnitNameBuilder.Resolve(def), def.Belonged)
        │     Unit 建構：部隊名、勢力、UnitFlags、Location 綁定物件
        │
        └─② def.ApplyTo(this)
              ├─ ApplyCommonTo   → 兵力、士氣、主將/副將（從 OfficerPool 複製）
              ├─ SetTroopType
              └─ AddBattleSkill / …（從 SkillPool 複製技能）
```

`ApplyCommonTo` = **只處理 Unit 父類共用欄位**；`ApplyTo` = 共用 + 該兵種專屬（兵科、技能表）。

`internal` = 只給同專案組裝用（Def → Unit），外部請用 `new Combat(def)`。

## 設定檔

路徑：`Assets/StreamingAssets/scenario_start.properties`

```properties
spawn.player.type=legion
spawn.player.commander=1
spawn.player.hex_q=12
spawn.player.hex_r=8
```

- 前綴 `spawn.<名稱>` 一組一部隊
- `vice=3,4` 副將多個用逗號
- `custom_name=` 可覆寫自動命名

讀檔：`ScenarioUnitSpawner` → 仍走 `new Legion(def)`，與 UI 組隊同一條路。
