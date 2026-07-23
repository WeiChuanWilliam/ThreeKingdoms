# 兵力、傷兵、技能（依你最新說明）

## Unit 父類（所有子類共有）

| 欄位 | 說明 |
|------|------|
| `commanderOfficerId` | 主將 → officers 表 id |
| `viceOfficerIds` | 副將 id 列表 |
| `morale` / `stamina` | 士氣、體力 0～100 |
| `soldiers` / `wounded` | 總兵力、傷兵數 |
| `CalculateFoodConsumption()` | 抽象；子類實作 |

## 兵力規則

- **最少 10 人**：`soldiers < 10` → `IsAnnihilated` 團滅  
- **有效戰力**：`healthy + wounded × 0.5`  
  - 例：1000 人、500 傷 → `500 + 250 = 750`

```csharp
UnitManpower.EffectiveCombatStrength(soldiers, wounded);
UnitManpower.MinSoldiers; // 10
```

## 技能：為何不單獨 `UnitCombatAbilitySlots`？

每支部隊 **自己帶** 技能 id 集合，放在子類欄位上：

**Combat**

- `AddBattleSkill` / `RemoveBattleSkill` 等（不暴露 HashSet 本身）  

**Transport**

- `AddStrategySkill` / `RemoveStrategySkill`  

**Legion**

- 本身不帶戰鬥技能表；戰法在隸屬的 `Combat` 上  

部隊由 `UnitUtil.CreateCombat`／`DeployCombat` 直接產生執行時 `Combat`（**無**類似武將的 Def／執行時雙層）。不讀部隊 JSON。

> **兵種戰法（B／A／S）**：見 [`SKILL_DESIGN.md`](SKILL_DESIGN.md)。行為寫死在 code；`skill.properties` 只當規劃備忘。戰法自有射程（≠ 普攻）、結算吃部隊攻擊係數；與上列四槽 API 尚未合併。

## 組隊入口

`UnitUtil.CreateCombat(factionId, troopKindKey, soldiers, commanderId, viceIds…)`
