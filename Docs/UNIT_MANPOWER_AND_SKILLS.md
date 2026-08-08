## 兵力與傷兵

- `Soldiers`：總兵力（含傷兵）
- `Wounded`：傷兵（仍屬部隊，**不計入**戰鬥人力）
- `EffectiveCombatStrength`／`FightingStrength`＝總兵 − 傷兵

```
UnitManpower.FightingStrength(soldiers, wounded);
UnitManpower.MinSoldiers; // 10
```

## 戰鬥部隊屬性兩段式

```
武將狀態變（傷病／體力／換將）
  → RefreshOfficerFive
  → 寫入 Combat.OfficerFive（統／武／智／政／魅）← 隨時更新的物件

戰鬥或玩家查看
  → RecalculateStats / 讀 Stats（dirty 時重算）
  → Combat.Stats（CombatTroopStatBlock：攻／防／擊破／破城／策略／建設）
```

| 物件 | 時機 | 內容 |
|------|------|------|
| `OfficerFive` | 武將狀態變更即寫回 | 統合五維 |
| `Stats` | 戰鬥／查看 | 最終戰鬥力六項 |

兵種表 `Troop*` 仍為兵種定義原始值（與上兩者不同層）。

## 技能

戰法 API 暫暫停；見 [`SKILL_DESIGN.md`](SKILL_DESIGN.md)。

部隊由 `UnitUtil.CreateCombat`／`DeployCombat` 直接產生執行時 `Combat`。

## 組隊入口

`UnitUtil.CreateCombat(factionId, troopKindKey, soldiers, commanderId, viceIds…)`
