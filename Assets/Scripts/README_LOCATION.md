# 地格 Location C++ → C# 對照

## 檔案

| C++ | C# |
|-----|-----|
| `Location::AbstractLocation` | `Core/Locations/AbstractLocation.cs` |
| （具體類） | `Core/Locations/MapLocation.cs` |
| `LocationFlag` | `Data/Locations/LocationFlags.cs` |
| `Building::AbstractBuilding` | `Core/Buildings/AbstractBuilding.cs` |
| `Unit::GroupUnit` | `Core/Units/GroupUnit.cs` |
| 整圖索引 | `Core/Locations/LocationGrid.cs` |

## 座標

| C++ | C# | 六角 Spike |
|-----|-----|------------|
| `row`, `column` | `Row`, `Column` | `HexCoord(column, row)` 即 (q,r) |

## 方法（C++ `boolean` → C# `bool`）

| C++ | C# | 備註 |
|-----|-----|------|
| `unitMoveed` | `UnitMoved` | 拼字修正 |
| `setOnFire` 等 | 同名 PascalCase | `MapLocation` 內有 Spike 預設邏輯 |

## 與其他系統

```text
LocationGrid (每 hex 一格 MapLocation)
  ├─ terrain  → Terrain::AbstractTerrain
  ├─ building → AbstractBuilding
  ├─ fightingUnit / groupUnit
  └─ belonged   → 勢力

MapUnit（部隊）在 hex 上移動時呼叫 UnitMoveIn / UnitMoveOut
```

## C++ 原檔注意

- `AbstractBuildings` / `AbstractBuilding` 混用 → C# 統一 `AbstractBuilding`
- `virtual ... const{}` 空實作 → C# `abstract` + `MapLocation` 預設

請繼續貼：`AbstractBuilding` 完整標頭、城池 `City`、若有的 `GroupUnit.h`。
