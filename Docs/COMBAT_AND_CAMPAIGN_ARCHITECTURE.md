# 戰役與戰鬥架構（修訂）

**狀態**：架構定案／文件階段；**尚未**大規模改程式。  
相關：[`LEGION_TERMINOLOGY.md`](LEGION_TERMINOLOGY.md)（兵團）、[`UNIT_AND_LOCATION.md`](UNIT_AND_LOCATION.md)（地格）。

---

## 0. 修訂摘要

| | 舊思路（取消） | 新思路（定案） |
|---|----------------|----------------|
| 後方連線 | 部隊與主城／主隊切開 → **斷補給線／斷糧** | **取消**；兵團自帶糧薪出征 |
| 戰術格地形 | 山谷、之石等特殊**塗格地形**加成 | **取消**該類戰術格設計 |
| 戰略壓制 | （補給線隱含） | **Adjacent**：據點鄰接控制權影響戰鬥加成 |
| 遠征後勤 | 靠連線維持 | 自帶**有限**軍糧／金錢；後方據點失守 → 戰力降、無法久戰 |
| 戰術朝向 | 無 | **向量（Facing）**：正面全防／背面半防半攻 |

---

## 1. 取消的設計

### 1.1 補給線切斷（斷糧／斷冰糧）

**不再使用**下列邏輯：

```text
兵團出征 ──連線──► 出軍城池／主隊
        │
        ╳ 連線被敵切斷
        ▼
    立即斷糧、部隊失效
```

- 部隊**不會**因為與主城地理「切開」而自動斷糧。
- 個性、戰法上的「斷糧」**debuff** 仍可存在（[`PERSONALITY_TRAITS_TABLES.md`](PERSONALITY_TRAITS_TABLES.md)），但**不是**來自補給線幾何。

### 1.2 戰術格特殊地形（山谷、之石等）

**不再**在戰鬥塗格上設計「山谷／之石」等額外地形類，也不靠這類格提供攻防加成。

戰術層地形簡化為：

- 六角格**是否可塗**（控制範圍）
- 單位**面向（向量）**
- （可選）戰略地圖已有的 `TerrainType` 僅影響**行軍**，不引入第二套戰術山谷系統

---

## 2. 戰略層：Adjacent 壓制

### 2.1 節點與鄰接

戰略地圖上的**據點**（資料節點，非單一六角）例如：

| 類型 | 例 |
|------|-----|
| 縣城 | 麥城、長坂… |
| 要塞 | 關隘、營寨 |
| 城市 | 江陵、襄陽… |

每個據點有一組 **`adjacent`**（鄰接據點 id 列表），形成戰略圖：

```text
        [江陵]──[要塞A]
           │         │
        [麥城]──[長坂]
           │
        [荊州城]
```

### 2.2 戰鬥發生點的「鄰接控制權」

當戰鬥發生在據點 **P**（或 P 所轄戰場）：

```text
對 P 的每一個 adjacent 據點 A：
  若 A 的 owner == 己方勢力 → 己方 Aggression 計分 +1
  若 A 的 owner == 敵方勢力 → 敵方 Aggression 計分 +1
```

**Aggression（壓制值）** 轉為該場戰鬥的**戰鬥力加成**（具體係數待平衡表）。

### 2.3 範例：麥城

關羽困**麥城**，四周**江陵、長坂**等已被**孫權**占領：

| 據點 | 控制者 |
|------|--------|
| 麥城（戰場） | 關羽（或僵持） |
| 鄰接：江陵、長坂… | 孫權 |

→ 孫權在麥城戰場的部隊享有 **Adjacent 壓制加成**；關羽方鄰接多為敵，加成低或為負。

**重點**：加成來自**戰略據點鄰接**，不是戰術格上畫補給線。

### 2.4 占位 API（虛構）

```csharp
// 戰略圖（開局／存檔載入）
StrategicNodeGraph  // id, type, owner, adjacent[]

float GetAdjacentAggressionBonus(int battleNodeId, int factionId);
// 比較己方 vs 敵方鄰接控制數 → 戰鬥修正
```

---

## 3. 戰略層：兵團出征與自帶糧薪

### 3.1 兵團（Legion）自給自足

[`Legion`](LEGION_TERMINOLOGY.md) 出征時：

| 欄位（概念） | 說明 |
|--------------|------|
| `carriedFood` | 自帶軍糧，**有上限** |
| `carriedMoney` | 自帶金錢，**有上限** |
| 每日消耗 | 依兵力、兵種耗糧係數遞減 |

**沒有**「連回出軍城才不算斷糧」的規則。

### 3.2 鄰接失守 → 戰力下降（非瞬間斷糧）

敵方奪取兵團**四周**縣城／要塞（戰場據點的 adjacent）時：

1. **敵方**在該區作戰獲得 §2 的 Adjacent 加成（壓制）。
2. **己方兵團**戰鬥力**下降**（士氣／補給線心理／後路被斷的綜合修正——**不是**糧食歸零）。
3. 兵團仍靠自帶糧食續戰，但**無法從失守後方再補給** → 糧盡後必須撤退或潰敗。

```text
關羽兵團在麥城（帶糧 30 天份）
        │
        │ 孫權偷襲奪江陵、長坂（adjacent 全變敵）
        ▼
├── 孫權：Adjacent 加成 ↑
├── 關羽：戰鬥修正 ↓
└── 關羽：無法再從後方補糧，30 天後糧盡 → 被迫結束遠征
```

**設計意圖**：迫使玩家在出征時考慮**後方會不會被偷袭**；敵方可透過佔鄰接據點達成**壓制**，而不靠補給線幾何切斷。

### 3.3 與「斷糧」個性的關係

- **戰略機制**：不再用補給線切斷。
- **戰術／個性**：「斷糧」類效果仍可對**單場戰鬥或單日**施加糧食消耗／戰力 debuff，與本章戰略分開。

### 3.4 占位 API（虛構）

```csharp
struct LegionCampaignSupply {
    int CarriedFood;
    int CarriedMoney;
    int MaxCarriedFood;
    int MaxCarriedMoney;
}

float GetAdjacentCombatModifier(Legion legion, int battleNodeId);
// 綜合 §2 aggression + 後方據點失守懲罰
```

---

## 4. 戰術層：向量（Facing）與塗格

### 4.1 概念（類三國志塗格，加朝向）

- 每日（或每戰術回合）部隊可**移動**並**塗**相鄰六角為己方控制。
- 每支部隊有 **Facing**（六向之一）：東、東北、西北、西、西南、東南。

### 4.2 前移與「正面／背面」

假設部隊 **面向東** 前進一格：

```text
        [東北]  [東]←面向   （東北、東南 = 正面弧）
[西北]  [部隊]  [東南]
        [西南]  [西]       （西、西北、西南 = 背面弧）
```

六角共六鄰：**面向東**時

| 弧區 | 鄰格方向 | 受擊時攻防 |
|------|----------|------------|
| **正面** | 東、東北、東南 | **100%** 攻擊／防禦 |
| **背面** | 西、西北、西南 | **50%** 攻擊／防禦（被偷襲） |

（左右各一弧合併進正面／背面；上表按「朝東」範例。）

### 4.3 塗格與包圍

- 前進時可將**正面兩側**及前方格納入控制（塗色），幅度可設「左右各 1～2 格」（待與三國志對齊細節）。
- **孤軍深入**：四面皆敵、且多數攻擊來自背面弧 → 有效戰力大幅打折，鼓勵不要單兵團無後援狂衝。

### 4.4 占位 API（虛構）

```csharp
enum HexDirection { East, NorthEast, NorthWest, West, SouthWest, SouthEast }

struct TacticalUnitState {
    HexCoord Position;
    HexDirection Facing;
}

float GetFacingCombatMultiplier(HexDirection attackerFrom, HexDirection defenderFacing);
// 正面 1.0，背面 0.5（攻、防同乘）
```

---

## 5. 兩層如何協作

```text
戰略地圖（據點 + adjacent + 兵團糧薪）
        │
        │ 在某據點開戰
        ▼
計算 Adjacent Aggression 加成（§2、§3）
        │
        ▼
戰術戰場（六角 + Facing + 塗格）
        │
        │ 每格交戰再乘 Facing 修正（§4）
        ▼
最終傷害／防禦
```

| 層級 | 回答的問題 |
|------|------------|
| 戰略 Adjacent | 「誰占住戰場四周的縣／要塞？」 |
| 兵團糧薪 | 「還能撐多久？後方丢了能否補給？」 |
| 戰術 Facing | 「這一仗是正面打還是被繞後？」 |

---

## 6. 與現行程式的關係

| 現有 | 修訂後 |
|------|--------|
| `CellData.CountyId` / `OwnerId` | 可對接戰略據點；需另建 `StrategicNodeGraph` |
| `TerrainType`（平原／山林…） | 保留**行軍**；不作戰術山谷加成 |
| `MovementRules` 每日行動力 | 戰略行軍沿用；戰術塗格另訂消耗 |
| `Legion` | 待加 `carriedFood` / `carriedMoney` 上限 |
| 補給線程式 | **不實作**；若舊稿有則刪 |
| `Combat` 戰鬥實例 | 待加 `Facing`、戰術格控制表 |

**未實作**：`StrategicNodeGraph`、`AdjacentAggressionRules`、`FacingCombatRules`、兵團攜帶糧薪。

---

## 7. 建議實作順序（將來）

1. 戰略據點表 + `adjacent` 圖（可手寫 JSON）
2. `AdjacentAggressionRules` → 接入戰鬥開場加成
3. `Legion` 攜帶糧薪 + 每日消耗 + 無法補給時結束遠征
4. 戰術層 `HexDirection` + `FacingCombatRules`
5. 塗格控制範圍（正面弧展開）

---

## 8. 已取消項（開發時請勿再加）

- [ ] 補給線幾何切斷 → 斷糧
- [ ] 戰術格「山谷／之石」特殊地形類
- [ ] 部隊必須與主城連線才能作戰

---

## 9. 相關文件

- [`LEGION_TERMINOLOGY.md`](LEGION_TERMINOLOGY.md) — 兵團 vs 軍團
- [`NAVY_AND_RIVER_MOVEMENT.md`](NAVY_AND_RIVER_MOVEMENT.md) — 水戰／港灣（文中「斷補給」改指**戰術騷擾**，非補給線幾何）
- [`SCENARIO_OFFICER_POOL.md`](SCENARIO_OFFICER_POOL.md) — 武將池（與本架構獨立）
