# 兵種 Class 繼承結構

## 你要的模型（對照 Java）

```text
AbstractTroopKind                 // 最上層 abstract
├── InfantryTroopKind (sealed)    // 第 1 階，尚未分叉
├── AbstractNavyTroopKind (abstract)   // 水軍「兵科」— 不能 new
│   ├── NavySmallTroopKind (sealed)    // 走舸 — 具體
│   ├── NavyMediumTroopKind            // 蒙衝
│   ├── NavyLargeTroopKind             // 樓船
│   └── NavyFinalTroopKind             // 鬥艦
├── AbstractSpearTroopKind (abstract)  // 槍兵線
│   ├── SpearTroopKind (sealed)
│   └── …
└── …（其餘兵科見 TroopKindBranches.cs）
```

- **abstract 兵科 class**：表達「這一條線的共同規則」，可在此寫整線預設克制／技能。
- **sealed 具體 class**：一個兵種一個 class，可 override 專屬技能與對某兵種加傷。

## 檔案

| 檔案 | 內容 |
|------|------|
| `AbstractTroopKind.cs` | 最上層：六圍（properties）、技能、相性 |
| `TroopKindBranches.cs` | 各兵科 `abstract class` |
| `InfantryCavalryTroopKinds.cs` | 步/騎具體 `sealed` |
| `ArcherSiegeNavyTroopKinds.cs` | 弓/器械/水具體 `sealed` |
| `TroopKindRegistry.cs` | 用 key 取實例 |
| `TroopKindTree.cs` | 升級樹（見 Docs/TROOP_KIND_TREE_逐行解釋.md） |

## 用法

```csharp
AbstractTroopKind k = TroopKindRegistry.Get("navy.medium");
// 實際型別是 NavyMediumTroopKind，也是 AbstractNavyTroopKind

if (k is AbstractNavyTroopKind navyUnit) { /* 水軍共通邏輯 */ }
```
