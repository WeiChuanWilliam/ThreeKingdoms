# ThreeKingdoms 文件索引

所有設計／對照／測試說明集中在此目錄。程式碼旁的舊 README 已改為轉址 stub。

---

## 入門

| 文件 | 內容 |
|------|------|
| [SETUP.md](SETUP.md) | Unity 6 安裝、第一次開專案、HexSpike 場景 |
| [LOCAL_TESTS.md](LOCAL_TESTS.md) | 本機 `dotnet test`（不需 Unity） |
| [UNITY_TESTS.md](UNITY_TESTS.md) | Unity 選單／Test Runner 可選測試 |

---

## 核心系統

| 文件 | 內容 |
|------|------|
| [UNIT_AND_LOCATION.md](UNIT_AND_LOCATION.md) | 部隊 ↔ 地格、著火、駐紮（**請先讀**） |
| [UNIT_TYPES.md](UNIT_TYPES.md) | Legion / Combat / Transport |
| [UNIT_MANPOWER_AND_SKILLS.md](UNIT_MANPOWER_AND_SKILLS.md) | 兵力、Stats 兩段式 |
| [UNIT_STATUS_AND_TERRAIN_HAZARD.md](UNIT_STATUS_AND_TERRAIN_HAZARD.md) | 計略狀態 vs 地形傷害 |
| [TERRAIN.md](TERRAIN.md) | 地形類型、TerrainRegistry |
| [SETTLEMENT_SITES.md](SETTLEMENT_SITES.md) | 據點／建築種類 |
| [COMBAT_AND_CAMPAIGN_ARCHITECTURE.md](COMBAT_AND_CAMPAIGN_ARCHITECTURE.md) | 戰略／戰術架構修訂 |
| [NAVY_AND_RIVER_MOVEMENT.md](NAVY_AND_RIVER_MOVEMENT.md) | 水軍、江河（規劃） |

---

## 武將

| 文件 | 內容 |
|------|------|
| [OFFICER_CLASS.md](OFFICER_CLASS.md) | 武將類別、發揮值、Stats |
| [OFFICER_REFERENCE.md](OFFICER_REFERENCE.md) | 欄位速查 |
| [PERSONALITY_TRAITS_TABLES.md](PERSONALITY_TRAITS_TABLES.md) | 個性表 |
| [OFFICER_CPP_MIGRATION.md](OFFICER_CPP_MIGRATION.md) | C++ → C# 對照 |

---

## 兵種

| 文件 | 內容 |
|------|------|
| [TROOP_KIND_TREE.md](TROOP_KIND_TREE.md) | 升級樹總覽 |
| [TROOP_KIND_TREE_FAQ.md](TROOP_KIND_TREE_FAQ.md) | 常見問題 |
| [TROOP_KIND_TREE_逐行解釋.md](TROOP_KIND_TREE_逐行解釋.md) | 程式逐行 |
| [TROOP_KINDS_CODE.md](TROOP_KINDS_CODE.md) | 兵種 class 繼承結構 |
| [TROOP_KIND_CLASSES.md](TROOP_KIND_CLASSES.md) | 六圍語意 |

---

## 劇本／存檔

| 文件 | 內容 |
|------|------|
| [SCENARIO_UNIT_PLACEMENT.md](SCENARIO_UNIT_PLACEMENT.md) | 開局部隊放置 |
| [SCENARIO_OFFICER_POOL.md](SCENARIO_OFFICER_POOL.md) | 武將池 |
| [SAVE_AND_SCENARIO.md](SAVE_AND_SCENARIO.md) | 存檔與劇本 |
| [SCENARIO_ERAS_ALTERNATE.md](SCENARIO_ERAS_ALTERNATE.md) | 架空劇本線 |
| [LEGION_TERMINOLOGY.md](LEGION_TERMINOLOGY.md) | 軍團用語 |

---

## 其他

| 文件 | 內容 |
|------|------|
| [SKILL_DESIGN.md](SKILL_DESIGN.md) | 戰法設計 |
| [THREE_KINGDOMS_REGIONS.md](THREE_KINGDOMS_REGIONS.md) | 地區設定 |
| [UNIT_FILES_RESTORED.md](UNIT_FILES_RESTORED.md) | 檔案還原備忘 |

---

## C++ 遷移對照（簡表）

| 文件 | 對應 C++ |
|------|----------|
| [TERRAIN.md](TERRAIN.md) | `Terrain::AbstractTerrain` |
| [UNIT_CPP_MIGRATION.md](UNIT_CPP_MIGRATION.md) | `Unit::AbstractUnit` |
| [LOCATION_CPP_MIGRATION.md](LOCATION_CPP_MIGRATION.md) | `Location::AbstractLocation` |
| [OFFICER_CPP_MIGRATION.md](OFFICER_CPP_MIGRATION.md) | `Officer::AbstractOfficer` |
