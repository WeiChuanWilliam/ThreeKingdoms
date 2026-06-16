# Legion／兵團／軍團 用語

## 定案

| 英文（程式） | 中文 | 說明 |
|-------------|------|------|
| **Legion** | **兵團** | 地圖上出征的部隊編制（`Legion` 類、`UnitKind.Legion`） |
| （待定英文） | **軍團** | 後方大城市委任、電腦代管預設策略；**尚未實作** |

**不要**再把 `Legion` 翻成「軍團」。舊文件若寫「軍團＝Legion」請改為「兵團」。

## 武將 `LegionLeaderId`（Leader）

與 **`Belong`（勢力）** 不同：

| 欄位 | 意思 |
|------|------|
| `belong` | 武將屬於哪個**勢力**（劉備軍、曹操軍…） |
| `legionLeaderId` | 武將所屬**兵團**的**主將** defId |

範例：劉備勢力，關羽率兵團出征：

- 關羽：`belong`＝劉備勢力 id；`legionLeaderId`＝關羽自己的 id（自領兵團）
- 副將張飛（同兵團）：`belong`＝劉備；`legionLeaderId`＝關羽 id

指派主將／副將時由編組邏輯呼叫 `SetLegionLeader`（待與 `Unit` 編組一併接線）。

## 相關程式

- `Assets/Scripts/Core/Units/Legion.cs`
- `AbstractOfficer.LegionLeaderId` / `Officer.SetLegionLeader`
- `UnitNamingSettings` 後綴：`兵團`
