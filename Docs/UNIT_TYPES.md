# Unit 繼承結構（父類 + 三子類）

```text
Unit（父類，abstract）
├── Legion     軍團
├── Combat     戰鬥單位
└── Transport  運輸單位
```

對應 C++：`Unit::AbstractUnit` → C# 類名 **`Unit`**。

**沒有** `ActualUnit` 包裝層；地圖上直接用 `Legion` / `Combat` / `Transport`，變數型別可宣告為 `Unit`。

```csharp
Unit army = new Legion("主力", escort, factionId);
army.Location.BindToWorld(...);
```

`UnitLocationBinding` 僅負責格子連動（非 C++ 標頭）。
