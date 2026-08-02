## 兵力與傷兵

- `Soldiers`：總兵力（含傷兵）
- `Wounded`：傷兵（仍屬部隊，**不計入**戰鬥人力）
- `EffectiveCombatStrength`／`FightingStrength`＝總兵 − 傷兵

```
UnitManpower.FightingStrength(soldiers, wounded);
UnitManpower.MinSoldiers; // 10
```

## 戰鬥部隊屬性（`CombatStatMath` → `Combat.Stats`）

```
主／副將 → 部隊整體五維（統／武／智／政／魅）
         → ＋兵力／士氣／體力
         → CombatTroopStatBlock（攻／防／擊破／破城／策略／建設）
         → 寫入 Combat.Stats
```

| `Stats` 欄位 | 中文 |
|------|------|
| `Attack` | 攻擊 |
| `Defense` | 防禦 |
| `Jipo` | 擊破 |
| `Gongcheng` | 破城 |
| `Strategy` | 策略 |
| `Construction` | 建設 |

五維為計算用（`UnitLeadership` 等），最終只存 `Stats` 一包。兵種表 `Troop*` 仍為兵種定義原始值。

## 技能

戰法 API 暫暫停；見 [`SKILL_DESIGN.md`](SKILL_DESIGN.md)。

部隊由 `UnitUtil.CreateCombat`／`DeployCombat` 直接產生執行時 `Combat`。

## 組隊入口

`UnitUtil.CreateCombat(factionId, troopKindKey, soldiers, commanderId, viceIds…)`
