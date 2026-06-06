# TroopKindTree.cs 逐行解釋（學 C# 用）

對照檔案：`Assets/Scripts/Data/Units/TroopKindTree.cs`

---

## 第 1～4 行：引用與命名空間

```csharp
using System.Collections.Generic;

namespace ThreeKindoms.Data.Units
```

- `using`：告訴編譯器「我要用別的檔案裡的型別」，這裡用 `Dictionary`、`List`。
- `namespace`：類似 Java 的 package，避免類名衝突。`TroopKindTree` 全名是 `ThreeKindoms.Data.Units.TroopKindTree`。

---

## 第 9～11 行：靜態類別

```csharp
public static class TroopKindTree
```

- `static class`：**不能** `new TroopKindTree()`，整個遊戲共用這一個「工具箱」。
- 裡面的方法都寫成 `TroopKindTree.GetStage(...)` 這樣呼叫。
- 類比 Java 的 `class TroopKindTree { private TroopKindTree() {} static ... }`。

---

## 第 11 行：字典欄位（電話簿本體）

```csharp
static readonly Dictionary<string, TroopKindNode> nodes = Build();
```

| 關鍵字 | 意思 |
|--------|------|
| `static` | 屬於類別本身，不是某個物件實例 |
| `readonly` | 指派只能一次（這裡在類別載入時由 `Build()` 指派），之後不能 `nodes = 別的` |
| `Dictionary<string, TroopKindNode>` | 鍵是字串（如 `"spear"`），值是 `TroopKindNode` 結構 |
| `= Build()` | **類別第一次被用到時**執行 `Build()`，把整棵樹填進字典 |

**和兵種 class 的關係**：Tree 只存「誰是誰的父節點」；真正的 `KnightHubaoTroopKind` 在 `TroopKindRegistry` 另一本電話簿。

---

## 第 13～14 行：TryGetNode

```csharp
public static bool TryGetNode(string kindKey, out TroopKindNode node) =>
    nodes.TryGetValue(kindKey ?? "", out node);
```

- `TryGet`：查得到回 `true`，查不到回 `false`，不會拋例外。
- `out TroopKindNode node`：C# 用 **out 參數**把結果傳回給呼叫者（類似 Java 用長度 1 的陣列當 out，但 C# 語法更明確）。
- `kindKey ?? ""`：若 `kindKey` 是 `null`，改成空字串（避免 null 當 key）。
- `=>`：**運算式主體**，等同 `{ return nodes.TryGetValue(...); }`。

使用範例：

```csharp
if (TroopKindTree.TryGetNode("spear", out TroopKindNode node))
    Console.WriteLine(node.ParentKey); // "infantry"
```

---

## 第 17～31 行：GetStage（第幾階）

```csharp
public static int GetStage(string kindKey)
{
    if (!TryGetNode(kindKey, out TroopKindNode node))
        return 1;
    int stage = 1;
    string parent = node.ParentKey;
    while (!string.IsNullOrEmpty(parent))
    {
        stage++;
        if (!TryGetNode(parent, out TroopKindNode p))
            break;
        parent = p.ParentKey;
    }
    return stage;
}
```

邏輯（沿父節點往上數層數）：

```text
spear.daji → 父 spear.advance → 父 spear → 父 infantry → 無父
stage = 1 + 1 + 1 + 1 = 4
```

- `string.IsNullOrEmpty(parent)`：父鍵為 null 或 `""` 就停止。
- 器械 `siege.mushou` 父 `siege.charger` 無再父 → stage = 2。

---

## 第 33～43 行：GetChildren（升級 UI 用）

```csharp
public static IReadOnlyList<string> GetChildren(string parentKey)
{
    var list = new List<string>();
    foreach (var kv in nodes)
    {
        if (kv.Value.ParentKey == parentKey)
            list.Add(kv.Key);
    }
    list.Sort();
    return list;
}
```

- 遍歷整本字典，找出「爸爸是 `parentKey`」的所有 key。
- 例：`GetChildren("spear.advance")` → `["spear.daji", "spear.qingzhou"]`（排序後）。
- `IReadOnlyList`：呼叫端只能讀不能改列表。

---

## 第 45～46 行：GetNavyNext（水軍專用）

```csharp
public static string GetNavyNext(string kindKey) =>
    TryGetNode(kindKey, out TroopKindNode n) ? n.NextLinear : null;
```

- 水軍是**一條線**，不是 Y 分叉，所以多存 `NextLinear`：small → medium → large → final。
- `? :` 三元運算：查到就用 `NextLinear`，否則 `null`。

---

## 第 48～82 行：Build() 建樹

```csharp
static Dictionary<string, TroopKindNode> Build()
{
    var map = new Dictionary<string, TroopKindNode>();
    // ... Add / AddLine ...
    return map;
}
```

- `var map = new Dictionary<...>()`：建立空字典。
- 每次 `Add` 放一筆：`key → TroopKindNode`。
- `AddLine` 幫步/騎/弓寫**一條 4 階線**（2 個第 4 階葉子）。

### AddLine 在做什麼（第 84～91 行）

```csharp
static void AddLine(..., string root, string b1, string b2, string l1, string l2)
{
    Add(map, b1, type, root);      // b1 的父 = root，例 spear 父 infantry
    Add(map, b2, type, b1);       // advance 父 branch
    Add(map, l1, type, b2);       // 葉1 父 advance
    Add(map, l2, type, b2);       // 葉2 父 advance（兩個兄弟）
}
```

對應你的設計：

```text
infantry (root)
  └─ spear (b1)
       └─ spear.advance (b2)
            ├─ spear.qingzhou (l1)
            └─ spear.daji (l2)
```

### Add（第 93～95 行）

```csharp
static void Add(..., string key, TroopType type, string parent, string nextLinear = null) =>
    map[key] = new TroopKindNode(key, type, parent, nextLinear);
```

- `nextLinear = null`：**預設參數**，不傳就當 null（非水軍不用下一階指標）。

---

## 第 97～112 行：TroopKindNode 結構體

```csharp
public readonly struct TroopKindNode
{
    public string Key { get; }
    ...
    public TroopKindNode(string key, TroopType type, string parentKey, string nextLinear)
    {
        Key = key;
        ...
    }
}
```

- `struct`：值類型，輕量資料包（像一筆記錄）。
- `readonly struct`：欄位建立後不變。
- `{ get; }`：只讀屬性，在建構子裡賦值。

---

## 和 Registry 的分工（常見困惑）

| | TroopKindTree | TroopKindRegistry |
|---|---------------|-------------------|
| 存什麼 | 父子關係、階層、水軍下一階 | 每個兵種 **class 的實例** |
| 典型用途 | 升級畫面「可選哪兩個」 | 戰鬥「這是虎豹騎，攻擊力多少、克制誰」 |
| 型別 | `TroopKindNode`（資料） | `AbstractTroopKind`（物件） |

```csharp
// 升級 UI
TroopKindTree.GetChildren("spear.advance");

// 戰鬥
TroopKindRegistry.Get("knight.hubao").Attack;
```

---

## 兵種 class 繼承（你要的結構）

見 `TroopKinds/README.md`：

```text
AbstractTroopKind                    ← 所有兵種共通
└── AbstractNavyTroopKind (abstract) ← 水軍兵科（abstract，不能 new）
    ├── NavySmallTroopKind (sealed)  ← 走舸（具體）
    ├── NavyMediumTroopKind          ← 蒙衝
    ...
```

`abstract` 兵科 class 可寫**整條線共通**的克制/技能；`sealed` 具體兵種再 override 專屬部分。
