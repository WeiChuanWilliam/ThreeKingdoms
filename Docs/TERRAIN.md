# 地形 Terrain C++ → C# 對照

## 命名空間

| C++ | C# |
|-----|-----|
| `Terrain::AbstractTerrain` | `ThreeKindoms.Core.Terrain.AbstractTerrain` |

## 檔案

| 內容 | 路徑 |
|------|------|
| 地形基底（abstract，唯讀） | `Core/Terrain/AbstractTerrain.cs` |
| 24 種地形子類（參數 hardcode） | `Core/Terrain/Terrains.cs` |
| 單例持有／以 id 查詢 | `Core/Terrain/TerrainRegistry.cs` |
| 類型 id（enum） | `Core/CellData.cs` 的 `TerrainType` |
| 類型能力旗標 | `Data/Terrain/TerrainFlags.cs` |

不再有外部參數表（`terrain.properties`、`terrains.json` 已移除）；要調整數值直接改 `Terrains.cs`。

## 設計

- 地形是**類型**不是格子：每種地形全域只有一個實例，由 `TerrainRegistry` 建立並持有（子類建構子為 `internal`）。
- 建築、火、陷阱等狀態跟著 **Location** 走；Location 以地形 id 取得地形資料，再決定能不能燒、能不能建。
- 城池（城池、關隘、要塞、岩砦；較不易著火）與城寨（港灣、縣城、陣、寨）只描述「這格是什麼地」；據點本身的資料（佔用格、中心格、耐久、駐紮兵團）在 Location 的建築上，見 [SETTLEMENT_SITES.md](SETTLEMENT_SITES.md) §6。

```csharp
AbstractTerrain t = TerrainRegistry.Get(cell.Terrain); // 以 TerrainType
AbstractTerrain t2 = TerrainRegistry.Get(23);          // 以 id → 不可通行
```

新增地形：`TerrainType` 加一個值 → `Terrains.cs` 加一個子類（實作 `CalculateMovement`）→ `TerrainRegistry` 陣列對應位置加 `new XxxTerrain()`（順序須與 enum 一致）。

## 移動

- 部隊自帶行動力：`Unit.MovementPoints`（本日剩餘）、`Unit.DailyMovementPoints`（每日補滿量，預設讀 `unit.properties` 的 `movement.daily_points`）。日出時 `RefillMovementPoints()`。
- 為何是「離開」扣而非「進入」扣：讓走位有代價。為了擊破敵人踏進濕地，下一回合自己會深陷其中；若改成進格扣，則會變成這回合打不到、下回合打完又能輕鬆脫離。
- **離開**一格時扣行動力：`AbstractTerrain.CalculateMovement(unit)`（抽象，每個子類實作）。一般地形呼叫基底 `SpendMoveCost`：`MoveCost` 依部隊行軍倍率折算後扣除，不足則不扣並回傳 false；不可通行一律回傳 false。
- 尋路（`HexPathfinder.FindPath`）的邊權同樣是「離開目前格的 `MoveCost`」，目標格須 `HexGrid.CanEnter`（格可通行且地形可達）。
- `ShipRequirement` 目前只是資料；部隊尚無船級，移動時還不檢查。

## 地形一覽

乘船：-1 一般部隊、0 走舸以上、1 蒙衝以上、2 樓船以上。士氣／體力負值＝回復。

| id | TerrainType | 名稱 | 離格消耗 | 火 n | 士氣 | 體力 | 乘船 | 可達 | 可建 | 可陷 | 要塞 | 可燃 |
|---:|-------------|------|---:|---:|---:|---:|:-:|:-:|:-:|:-:|:-:|:-:|
| 0 | MainRoad | 官道 | 3 | 0 | 0 | 0 | -1 | ✓ | ✓ | ✓ | ✓ | |
| 1 | MainPath | 主徑 | 4 | 1 | 0 | 1 | -1 | ✓ | ✓ | ✓ | ✓ | ✓ |
| 2 | ForestPath | 林道 | 6 | 3 | 0 | 1 | -1 | ✓ | | ✓ | | ✓ |
| 3 | Plain | 平地 | 5 | 1 | 0 | 1 | -1 | ✓ | ✓ | ✓ | ✓ | ✓ |
| 4 | Grassland | 草原 | 5 | 2 | 0 | 1 | -1 | ✓ | ✓ | ✓ | ✓ | ✓ |
| 5 | Forest | 森林 | 8 | 3 | 1 | 1 | -1 | ✓ | | | | ✓ |
| 6 | MountainForest | 山林 | 16 | 3 | 2 | 3 | -1 | ✓ | | | | ✓ |
| 7 | Jungle | 叢林 | 12 | 2 | 3 | 3 | -1 | ✓ | | ✓ | | ✓ |
| 8 | LowMountain | 低山 | 8 | 1 | 1 | 2 | -1 | ✓ | ✓ | ✓ | | ✓ |
| 9 | MidMountain | 中山 | 12 | 0 | 2 | 3 | -1 | ✓ | | ✓ | | |
| 10 | HighMountain | 高山 | 16 | 0 | 3 | 3 | -1 | ✓ | | | | |
| 11 | Wetland | 濕地 | 12 | 0 | 2 | 2 | -1 | ✓ | | | | |
| 12 | Sand | 沙地 | 6 | 0 | 0 | 0 | -1 | ✓ | ✓ | ✓ | ✓ | |
| 13 | MountainPath | 山路 | 6 | 1 | 0 | 2 | -1 | ✓ | | ✓ | | ✓ |
| 14 | PlankRoad | 棧道 | 8 | 0 | 3 | 1 | -1 | ✓ | | | | |
| 15 | PoisonSwamp | 毒沼 | 12 | 0 | 3 | 2 | -1 | ✓ | | | | |
| 16 | River | 河流 | 8 | 0 | 1 | 1 | -1 | ✓ | | | | |
| 17 | GreatRiver | 大河 | 4 | 0 | 0 | 0 | 0 | ✓ | ✓ | ✓ | | |
| 18 | Rapids | 急流 | 5 | 0 | 0 | 0 | 1 | ✓ | | | | |
| 19 | FrozenRiver | 凍河 | 6 | 0 | 0 | 0 | -1 | ✓ | | | | |
| 20 | Coast | 沿海 | 4 | 0 | 0 | 0 | 2 | ✓ | ✓ | ✓ | | |
| 21 | City | 城池 | 0 | 1 | -1 | -1 | -1 | ✓ | | | | ✓ |
| 22 | Stockade | 城寨 | 0 | 2 | 0 | 0 | -1 | ✓ | | | | ✓ |
| 23 | Impassable | 不可通行 | 255 | 0 | 0 | 0 | -1 | | | | | |

## AbstractTerrain 成員

| 成員 | 說明 |
|------|------|
| `TerrainName` / `TerrainId` | 名稱與類型 id（= `TerrainType` 數值） |
| `TerrainFlags` | 可達／可建／可陷／要塞／可燃（可建＝能蓋普通建物如陣、寨；要塞＝能蓋要塞、岩砦等花大量金錢時間的建物） |
| `MoveCost` | 離開此格所需行動力 |
| `FireEffect` | 火計 n |
| `MoralePenalty` / `StaminaPenalty` | 士氣／體力懲罰（負值＝回復） |
| `ShipRequirement` | 進入所需最低船級 |
| `CalculateMovement(Unit)` | 抽象：計算離格行動力並從部隊扣除 |

## 與格子／部隊的分界

| 問題 | 看誰 |
|------|------|
| 這種地能不能燒／能不能設陷阱？ | `TerrainFlags` |
| 這一格現在有沒有火／陷阱？ | `LocationFlags` |
| 這支部隊腳下有沒有？ | `unit.IsOnFire`／`IsOnTrap` |

詳見 [UNIT_STATUS_AND_TERRAIN_HAZARD.md](UNIT_STATUS_AND_TERRAIN_HAZARD.md)。

## 六角地圖 JSON

`terrain` 欄位填 `TerrainType` 名稱（不分大小寫，如 `"MainRoad"`、`"forest"`）或數值 id；無法辨識時視為平地。行動力消耗一律來自地形，地圖 JSON 不再有 `enterCost`。
