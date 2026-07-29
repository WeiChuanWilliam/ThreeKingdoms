# 兵種戰法設計（Skill）

**狀態**：設計定案；行為寫死在 C#；**名稱／說明走多語言 properties**（目前只做中文）。

---

## 1. 實作邊界（定案）

| 項目 | 定案 |
|------|------|
| 行為／結算 | **寫死在 C#**（位移、止步、著火、貫穿、擊退等特殊動作太多，不適合資料驅動） |
| `skill.properties` | **名稱／說明**＝多語言；**`.front`／`.end`**／**`.stamina`**／**`.damage`**＝數值（語系無關，暫寫在 chinese） |
| 戰法是什麼 | **獨立物件**（code 內），不是部隊普攻的別名 |
| 戰法射程 | **`.front`／`.end` 自有**；**無關**部隊普攻 `attack_range` |
| 體力消耗 | **`.stamina`**：每招可設不同整數；施放時扣體力（扣部隊或主將，實作時再定） |
| 傷害加乘 | **`.damage`**：百分率；`100`＝等同普攻，`125`＝1.25 倍；再乘部隊攻擊係數等 |
| 誰能用 | 規劃上對齊兵系 B／A／S 鍵；實際解鎖條件寫在 code |

```text
StreamingAssets/chinese/skill.properties   ←  名稱 + .desc + .front + .end + .stamina + .damage
StreamingAssets/english/skill.properties   ←  之後：同 key 的名稱／說明（不必重複數值鍵）
C# 戰法 class（拼音名）                    ←  特殊動作；可讀上列數值
部隊 attack_range                          ←  僅普攻
部隊攻擊係數                               ←  戰法結算時讀取（再乘 .damage/100）
```

---

## 2. properties key

```text
skill.<兵系>.B                 顯示名
skill.<兵系>.B.desc            說明
skill.<兵系>.B.front          攻擊距離下限
skill.<兵系>.B.end            攻擊距離上限
skill.<兵系>.B.stamina         體力消耗（非負整數）
skill.<兵系>.B.damage          傷害加乘（百分率）
skill.<兵系>.A / .desc / .front / .end / .stamina / .damage
skill.<兵系>.S.<尾碼> / .desc / .front / .end / .stamina / .damage
```

### `.front`／`.end` 語法

| 例 | 意義（格，**含兩端**；須 `front ≤ end`） |
|----|------------------------------------------|
| `front=0` `end=0` | 自身／無敵人目標格 |
| `front=1` `end=1` | 僅距離 1 |
| `front=1` `end=3` | 距離 1～3 |
| `front=2` `end=3` | 距離 2～3 |

合法目標距離 `d` 滿足 `front ≤ d ≤ end`。與普攻無關；UI 選格與戰法判定都以該招為準。

### `.stamina` 語法

| 寫法 | 意義 |
|------|------|
| `0` | 不耗體力 |
| `10`、`25`… | 施放消耗該點體力 |

每招各自設定（B／A／S 可不同）。數值暫定，直接改 properties 即可。扣誰的體力（部隊 `stamina` 或主將）實作時定。

### `.damage` 語法

| 寫法 | 意義 |
|------|------|
| `100` | 等同普攻傷害（×1.00） |
| `125` | 1.25 倍 |
| `80` | 0.80 倍 |
| `0` | 本招不以傷害公式結算（純增益／純控制等） |

```text
戰法傷害 ≈ 同部隊普攻傷害基準 × (.damage / 100) ×（個性等其他修正）
```

特殊分段（如貫射第二目標半傷、連擊兩下）寫在 C#；`.damage` 是主效果／每段的基準加乘。

| 兵系 key | 對應 `TroopType` | 備註 |
|----------|------------------|------|
| `infantry` | Infantry | |
| `cavalry` | Cavalry | |
| `archary` | Archer | **拼寫約定**（非 `archer`） |
| `siege` | Siege | **無 B／A**，只有 `S.<器械>` |
| `navy` | Navy | |

S 尾碼對齊 `unit.properties` 特色兵種鍵尾（如 `bow.wudan` → `S.wudan`）。

`# Chandou` 這類註解是 **C# class 名**（拼音），語系無關。

目前只維護 **`chinese/`**；其他語系目錄有需要再建（建議只放名稱／說明）。

---

## 2.1 C# 類名（拼音 PascalCase）

實作時例：`class Chandou : Skill`。同顯示名可共用邏輯類（係數吃部隊）；**每條 properties 鍵可有自己的 `.front`／`.end`／`.stamina`／`.damage`**。

| 中文 | 類名 | 規劃鍵 |
|------|------|--------|
| 纏鬥 | `Chandou` | infantry.B |
| 堅持 | `Jianchi` | infantry.A |
| 突刺 | `Tuci` | infantry.S.qingzhou |
| 拉扯 | `Lache` | infantry.S.daji |
| 破陣 | `Pozhen` | infantry.S.xianzhen |
| 死守 | `Sishou` | infantry.S.baimao |
| 強襲 | `Qiangxi` | cavalry.B |
| 衝鋒 | `Chongfeng` | cavalry.A |
| 連擊 | `Lianji` | cavalry.S.hubao |
| 追擊 | `Zhuiji` | cavalry.S.xilian |
| 突破 | `Tupo` | cavalry.S.bingzhou |
| 閃擊 | `Shanji` | cavalry.S.baima |
| 齊射 | `QiShe` | archary.B |
| 火矢 | `HuoShi` | archary.A／siege.S.shooter／navy.S.large |
| 破軍 | `Pojun` | archary.S.wudan |
| 火箭 | `Huojian` | archary.S.danyang |
| 貫射 | `Guanshe` | archary.S.xiandeng |
| 亂射 | `LuanShe` | archary.S.zhuge |
| 衝撞 | `Chongzhuang` | siege.S.charger |
| 火龍 | `Huolong` | siege.S.mushou |
| 踐踏 | `Jianta` | siege.S.elephant |
| 投石 | `Toushi` | siege.S.stone |
| 貫穿 | `Guanchuan` | siege.S.crossbow／navy.S.final |
| 火攻 | `Huogong` | navy.B |
| 猛撞 | `Mengzhuang` | navy.A |

`.front`／`.end`／`.stamina`／`.damage` 以 [`skill.properties`](../Assets/StreamingAssets/chinese/skill.properties) 為準。

---

## 3. 與其他文件的關係

| 文件／概念 | 關係 |
|------------|------|
| [`TROOP_KIND_CLASSES.md`](TROOP_KIND_CLASSES.md) `AttackRange` | **普攻**射程；戰法用 `.front`／`.end`，二者無關 |
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

## 5. 文案／覆蓋缺口（改中文 properties 即可）

1. 水軍目前無 `S.small`／`S.medium`（若刻意則維持）。
2. `.front`／`.end`／`.stamina`／`.damage` 皆為**暫定值**，請直接改 properties。
3. 解鎖條件（蒙衝或以上等）目前只在 desc；實作時寫進 code。
4. 扣體力對象（部隊 vs 主將）尚未定。

---

## 6. 之後實作（現在不要做）

- 各戰法 C# class（特殊動作寫死；可讀 `.front`／`.end`／`.stamina`／`.damage`）
- 讀取 `.front`／`.end`（整數，front ≤ end）
- 施放時扣 `.stamina`；傷害 × (`.damage` / 100)
- 與部隊攻擊係數接線
- 與 `Combat` 四槽／存檔是否對齊或廢除

**不要做**：把特殊動作做成 properties 腳本／通用效果 DSL。

---

## 7. 相關路徑

| 路徑 | 用途 |
|------|------|
| `Assets/StreamingAssets/chinese/skill.properties` | 名稱＋`.desc`＋`.front`＋`.end`＋`.stamina`＋`.damage` |
| `Assets/StreamingAssets/<locale>/skill.properties` | 之後其他語系（名稱／說明；可不重複數值鍵） |
| `Assets/Scripts/Data/Skill/SkillConfigUtil.cs` | 讀文案；不驅動行為 |
| 本文件 | 寫死 code／多語言／數值邊界 |
