# 武將 C++ → C# 對照

## 檔案

| C++ | C# |
|-----|-----|
| `Officer::AbstractOfficer` | `Core/Officers/AbstractOfficer.cs` |
| （具體子類） | `Core/Officers/Officer.cs` |
| `Personality` | `Data/Officers/PersonalityDef.cs` + `PersonalityDatabase.cs` |
| 個性表 | `StreamingAssets/personality_traits.json`（來源 `Docs/PERSONALITY_TRAITS_TABLES.md`） |
| `Item` | `Data/Items/ItemDef.cs`（佔位） |
| `BattleSkill` | `Data/Battle/BattleSkill.cs` |
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

## C++ 標頭未完成部分

原檔末尾 `BattleSkill::shieldForceSkill` 語法不完整，C# 改為 `OfficerBattleSkills` 六個槽位。

請繼續貼：`City`、`Unit`（完整）、`Faction` 等 .h，可同樣轉成 C#。
