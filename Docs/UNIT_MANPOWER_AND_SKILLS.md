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

- 本身不帶戰鬥技能表；靠 `Escort`（Combat）  

`UnitDef` 是**遊戲內組隊**用的記憶體草稿（玩家選武將、兵力、金錢後填入），再 `new Combat(def)`。不讀部隊 JSON。

## UnitDef

- 耗糧在執行期 `unit.CalculateFoodConsumption()`，不在 Def 寫死
