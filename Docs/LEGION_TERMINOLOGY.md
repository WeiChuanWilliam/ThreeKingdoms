# Legion／兵團／軍團 用語

## 定案

| 英文（程式） | 中文 | 說明 |
|-------------|------|------|
| **Legion** | **兵團** | 地圖上出征的部隊編制（`Legion` 類、`UnitKind.Legion`） |
| （待定英文） | **軍團** | 後方大城市委任、電腦代管預設策略；**尚未實作** |

**不要**再把 `Legion` 翻成「軍團」。舊文件若寫「軍團＝Legion」請改為「兵團」。

## 勢力 id、`Belong`、`LegionLeaderId`（暫定）

| 欄位 | 暫定語意 |
|------|----------|
| **勢力 id**／`Belong` | **＝執掌該勢力的武將 defId**（劉備軍＝劉備 id＝1） |
| `LegionLeaderId` | **暫定與 `Belong` 相同**；日後可改回「所屬兵團主將 defId」 |

`SetBelong` 時會同步寫入 `LegionLeaderId`。

範例：

- 劉備：`Belong`＝1，`LegionLeaderId`＝1  
- 關羽（同勢力）：`Belong`＝1，`LegionLeaderId`＝1  

## 出戰 `IsDeployed`

| 任職於 | 出戰？ |
|--------|--------|
| **Combat**／**Transport** | **是**（`IsDeployed = true`） |
| **Legion**（兵團） | **否** |
| 無部隊（城內待命／在野） | **否** |

組隊指派主將／副將時由 `Unit` 自動 `SetDeployed`；卸任則清回 false。

## 相關程式

- `Assets/Scripts/Core/Units/Legion.cs`
- `AbstractOfficer.Belong`／`LegionLeaderId`／`IsDeployed`
- `Officer.SetBelong`／`SetLegionLeader`／`SetDeployed`
- `UnitNamingSettings` 後綴：`兵團`
