# 兵種戰法設計（Skill）

**狀態**：設計定案／文件階段；特殊動作**寫死在 code**，properties 只當規劃備忘。

---

## 1. 實作邊界（定案）

| 項目 | 定案 |
|------|------|
| 行為／結算 | **寫死在 C#**（位移、止步、著火、貫穿、擊退等特殊動作太多，不適合資料驅動） |
| `skill.properties` | **規劃備忘**：記住有哪些招、顯示名、效果註解；可隨時改規劃，**不是**戰鬥引擎的資料來源 |
| 戰法是什麼 | **獨立物件**（code 內），不是部隊普攻的別名 |
| 戰法射程 | **戰法自己的攻擊範圍**（寫在該戰法 class／邏輯裡）；**無關**部隊普攻 `attack_range` |
| 傷害／強度 | 結算時**吃部隊攻擊係數**（及日後六圍／個性）；同名招可用不同兵種係數 |
| 誰能用 | 規劃上對齊兵系 B／A／S 鍵；實際解鎖條件寫在 code |

```text
skill.properties     ←  名稱／規劃筆記（可改、可丟；不驅動戰鬥）
C# 戰法 class        ←  真實行為、射程、效果、條件（準）
部隊 attack_range    ←  僅普攻
部隊攻擊係數         ←  戰法結算時讀取
```

---

## 2. properties key（規劃用）

```text
skill.<兵系>.B          兵系共用（適性 B）
skill.<兵系>.A          兵系共用（適性 A）
skill.<兵系>.S.<尾碼>   特色兵種專有
```

| 兵系 key | 對應 `TroopType` | 備註 |
|----------|------------------|------|
| `infantry` | Infantry | |
| `cavalry` | Cavalry | |
| `archary` | Archer | **拼寫約定**（非 `archer`） |
| `siege` | Siege | **無 B／A**，只有 `S.<器械>` |
| `navy` | Navy | |

S 尾碼對齊 `unit.properties` 特色兵種鍵尾（如 `bow.wudan` → `S.wudan`）。

`SkillConfigUtil` 可選讀顯示名（UI／除錯）；**不**從 properties 長出技能邏輯。

---

## 2.1 C# 類名（拼音 PascalCase）

備註寫在 `skill.properties`：`# <類名> | <效果>`。實作時例：`class Chandou : Skill`。

同顯示名 → **同一類**（係數吃部隊）。唯一拼音撞名：齊射／騎射皆 qishe → `QiShe`／`QiBingShe`。

| 中文 | 類名 | 英文意譯（僅參考） | 規劃鍵 |
|------|------|-------------------|--------|
| 纏鬥 | `Chandou` | Entangle / Bind | infantry.B |
| 堅持 | `Jianchi` | Hold Fast | infantry.A |
| 突刺 | `Tuci` | Thrust | infantry.S.qingzhou |
| 拉扯 | `Lache` | Drag | infantry.S.daji |
| 破陣 | `Pozhen` | Break Formation | infantry.S.xianzhen |
| 死守 | `Sishou` | Last Stand | infantry.S.baimao |
| 強襲 | `Qiangxi` | Shock Assault | cavalry.B |
| 衝鋒 | `Chongfeng` | Charge Through | cavalry.A |
| 突擊 | `Tuji` | Sudden Strike | cavalry.S.hubao |
| 追擊 | `Zhuiji` | Pursuit | cavalry.S.xilian |
| 閃擊 | `Shanji` | Raid | cavalry.S.bingzhou |
| 騎射 | `QiBingShe` | Mounted Shot | cavalry.S.baima |
| 齊射 | `QiShe` | Volley | archary.B |
| 火矢 | `HuoShi` | Fire Arrow | archary.A／siege.S.shooter／navy.S.large |
| 破軍 | `Pojun` | Break Army | archary.S.wudan |
| 遠射 | `Yuanshe` | Long Shot | archary.S.danyang |
| 貫射 | `Guanshe` | Piercing Shot | archary.S.xiandeng |
| 殲滅 | `Jianmie` | Annihilate | archary.S.zhuge |
| 衝撞 | `Chongzhuang` | Ram | siege.S.charger |
| 火龍 | `Huolong` | Fire Dragon | siege.S.mushou |
| 踐踏 | `Jianta` | Trample | siege.S.elephant |
| 投石 | `Toushi` | Stone Throw | siege.S.stone |
| 貫穿 | `Guanchuan` | Pierce | siege.S.crossbow／navy.S.final |
| 火攻 | `Huogong` | Fire Attack | navy.B |
| 猛撞 | `Mengzhuang` | Heavy Ram | navy.A |

共 **25** 個獨立類（火矢、貫穿各只算一次）。

---

## 3. 與其他文件的關係

| 文件／概念 | 關係 |
|------------|------|
| [`TROOP_KIND_CLASSES.md`](TROOP_KIND_CLASSES.md) `AttackRange` | **普攻**射程；戰法射程在各戰法 code 裡 |
| [`TROOP_KIND_TREE.md`](TROOP_KIND_TREE.md) | 兵種樹；S 尾碼對齊葉子，不改樹 |
| `Combat` 四槽 HashSet | 舊裝備槽管線；與 B／A／S 規劃表尚未對齊，之後 API 再定 |
| 武將無戰法欄位 | **相容** |
| 個性表「射程／火神」等 | 可修正成功率／傷害／射程；實作時標明改普攻還是戰法 |

---

## 4. 同名戰法（有意）

| 顯示名 | 規劃鍵 |
|--------|--------|
| 火矢 | `archary.A`、`siege.S.shooter`、`navy.S.large` |
| 貫穿 | `siege.S.crossbow`、`navy.S.final` |

註解「同樣戰法、用該兵種係數」→ code 可共用一個 class，或分 class 但共用效果；係數一律吃部隊。

---

## 5. 規劃備忘缺口（改 properties 即可，不影響引擎）

1. 水軍目前無 `S.small`／`S.medium`（若刻意則維持）。
2. `siege.S.shooter` 註解「井嵐」→「井欄」。
3. 解鎖條件（蒙衝或以上等）目前只在註解；實作時寫進 code。

---

## 6. 之後實作（現在不要做）

- 各戰法 C# class（射程、效果、條件寫死）
- 與部隊攻擊係數接線
- 與 `Combat` 四槽／存檔是否對齊或廢除

**不要做**：把特殊動作做成 properties 腳本／通用效果 DSL。

---

## 7. 相關路徑

| 路徑 | 用途 |
|------|------|
| `Assets/StreamingAssets/chinese/skill.properties` | 規劃：名稱＋效果註解 |
| `Assets/Scripts/Data/Skill/SkillConfigUtil.cs` | 可選讀名稱；不驅動行為 |
| 本文件 | 寫死 code／properties 邊界 |
