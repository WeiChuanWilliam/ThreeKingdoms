# TroopKindTree 常見困惑（直接回答）

## 1. `TroopType` 定義在哪？

**不在** `TroopKindTree.cs`，也**不在** properties。

在：

`Assets/Scripts/Data/Units/TroopType.cs`

```csharp
public enum TroopType : byte
{
    Infantry = 0,
    Cavalry = 1,
    Archer = 2,
    Spear = 3,
    Siege = 4,
    Navy = 5
}
```

`TroopKindTree` 的 `Add(..., TroopType.Infantry, ...)` 只是**引用**這個 enum，表示「這個節點屬於步兵大類」。

properties 裡的 `troop.type.infantry=步兵` 是**中文顯示名**，和 enum 是兩套東西。

---

## 2. 哪裡讀 properties？路徑在哪？

| 讀什麼 | 誰讀 | 路徑怎麼來 |
|--------|------|------------|
| 兵種**中文名**、後綴、狀態文案 | `UnitConfigUtil.Load(...)` | `GamePaths.ChineseUnitProperties` |
| 升級**父子關係**（誰的下一級） | **`TroopKindTree` 不讀檔** | 寫死在 `Build()` 裡 |

路徑設定在：

`Assets/Scripts/Unity/GamePaths.cs`

```csharp
public static string ChineseUnitProperties =>
    Path.Combine(StreamingAssets, "chinese", "unit.properties");
```

遊戲啟動時（`HexSpikeBootstrap`）：

```csharp
UnitConfigUtil.Load(GamePaths.ChineseUnitProperties);
```

**`TroopKindTree` 從來沒有呼叫 `Load`，也沒有路徑設定**——這是你覺得「沒設定路徑」的原因：樹的資料目前不在 properties 裡。

---

## 3. `Add` 和 `AddLine` 在幹嘛？

### `Add` — 登記「一個節點」

```csharp
Add(map, "spear", TroopType.Infantry, "infantry");
```

等於在字典裡寫一筆：

```text
key = "spear"
父節點 = "infantry"
大類 = Infantry
```

### `AddLine` — 登記「步/騎/弓那種 4 階 Y 分叉線」

```csharp
AddLine(map, TroopType.Infantry, "infantry", "spear", "spear.advance", "spear.qingzhou", "spear.daji");
```

等同連續四次 `Add`：

```text
infantry (根，前面單獨 Add 過)
  └─ spear          父 = infantry
       └─ spear.advance   父 = spear
            ├─ spear.qingzhou  父 = spear.advance
            └─ spear.daji      父 = spear.advance
```

**不是** properties 讀進來的，是程式裡**手寫**你設計的樹。

---

## 4. 為什麼 hardcode？為什麼不用 properties / static 變數？

**現狀（誠實說）：**

- `unit.properties`：給人看的字、部分規則數字（行軍力等）
- `TroopKindTree.Build()`：給程式用的**升級樹結構**，目前寫死在 C# 裡

原因當初是：properties 用 `key=value` 不好表達「一個父節點有兩個孩子」這種樹，所以拆成兩層。

**若你要「樹也進設定檔」**，可以之後改成例如：

```properties
troop.tree.spear.parent=infantry
troop.tree.spear.advance.parent=spear
```

再由 `TroopKindTreeLoader` 讀進 `Dictionary`——**還沒做**，所以現在才是 hardcode。

**兵種 class 的攻防數值**也在 C# `Configure()` 裡占位，同樣還沒接到 properties 的 `stat.*`。

---

## 5. 三份資料對照（記這張表就好）

```text
TroopType.cs (enum)     → 大類 ID：Infantry, Navy…
unit.properties         → 顯示名、UI 文案（有路徑、有 Load）
TroopKindTree.Build()   → 升級誰是誰的爸爸（無路徑、無 Load）
TroopKindRegistry       → 每個兵種 class 實例（無路徑、無 Load）
```

```text
顯示名：properties → UnitConfigUtil
升級樹：TroopKindTree（硬編碼）
戰鬥邏輯：TroopKindRegistry → KnightHubaoTroopKind 等 class
```
