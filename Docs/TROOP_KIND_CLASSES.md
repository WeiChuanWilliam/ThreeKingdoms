# 兵種類別（AbstractTroopKind）

## 數值欄位（所有兵種皆有）

| 屬性 | 中文 | 說明 |
|------|------|------|
| `Attack` | 攻擊 | 對一般部隊 |
| `Defense` | 防禦 | |
| `Mobility` | 機動 | 戰場/行軍係數用 |
| `AttackRange` | 攻擊距離 | 普攻／遠程射程（格）；properties: `attack_range` |
| `Jipo` | 急迫 | 削城防耐久、打缺口 |
| `Gongcheng` | 工程 | 對城內守軍傷害 |

## 程式位置

- `Data/Units/TroopKinds/AbstractTroopKind.cs` — 抽象類
- `InfantryCavalryTroopKinds.cs` / `ArcherSiegeNavyTroopKinds.cs` — **37** 個 `sealed` 子類
- `TroopKindRegistry.Get("spear.qingzhou")` — 取得單例定義

## 拼音鍵名修正

| 舊鍵 | 新鍵 | 說明 |
|------|------|------|
| `spear.chinzhou` | `spear.qingzhou` | 青州 |
| `armor.baihau` | `armor.baimao` | 白毦 |

properties 保留舊鍵顯示名；Registry / Tree 以新鍵為準。

## 與 Combat 連接（待做）

```csharp
AbstractTroopKind kind = TroopKindRegistry.Get("knight.hubao");
// combat 編制時記 kind.KindKey，戰鬥讀 kind.Attack 等
```

目前數值為占位，之後可改 `unit.properties` 的 `stat.*` 或 CSV。
