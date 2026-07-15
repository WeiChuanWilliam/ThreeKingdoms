# 武將 C++ → C# 對照

## 檔案

| C++ | C# |
|-----|-----|
| `Officer::AbstractOfficer` | `Core/Officers/AbstractOfficer.cs` |
| （具體子類） | `Core/Officers/Officer.cs` |
| `Personality` | `Data/Officers/PersonalityDef.cs` + `PersonalityDatabase.cs` |
| 個性表 | `StreamingAssets/personality_traits.json`（來源 `Docs/PERSONALITY_TRAITS_TABLES.md`） |
| `Item` | `Data/Items/ItemDef.cs`（佔位） |
| 戰法（部隊四槽） | `Core/Units/Combat.cs` + `Data/Skill/Skill.cs` |
| `Unit::AbstractUnit` | `Core/Units/AbstractUnit.cs` |
| `Unit::ActualUnit` | `Core/Units/MapUnit.cs` |
| 表資料 | `Data/Officers/OfficerDef.cs` + `StreamingAssets/officers.json` |

## 欄位對照

| C++ | C# |
|-----|-----|
| `attack` | `attack`（武力） |
| `policy` | `policy`（政治） |
| `officerFlag` 位元欄 | `OfficerFlag` struct |
| `belong` | `belong` |
| `pictureBuffer` | `byte[] pictureBuffer` |

## 載入範例（Unity Play 後 Console）

```csharp
var db = OfficerDatabase.LoadFromStreamingAssets();
var guan = db.GetOrCreateRuntime(1);
Debug.Log(guan.FullName + " 武 " + guan.Attack);
```

## 戰法歸屬

武將（`OfficerDef` / `Officer`）**無**戰法欄位；戰法僅裝備於戰鬥部隊 `Combat` 四槽，由 `UnitUtil.CreateCombat` 組隊後再 `AddBattleSkill` 等設定。

請繼續貼：`City`、`Unit`（完整）、`Faction` 等 .h，可同樣轉成 C#。
