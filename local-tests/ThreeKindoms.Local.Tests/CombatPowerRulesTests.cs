using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class CombatPowerRulesTests
    {
        [Fact]
        public void Recalculate_writes_stats_block()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var combat = UnitUtil.CreateCombat(1, "blade", 5000, commanderOfficerId: 0);
            combat.SetMorale(80);
            combat.SetStamina(70);

            var cmd = new Officer(1);
            cmd.SetStats(atk: 90, intel: 60, lead: 85, pol: 50, charm: 70);
            combat.SetCommander(cmd);

            Assert.True(combat.Stats.Attack > 0);
            Assert.True(combat.Stats.Defense > 0);
            Assert.True(combat.Stats.Gongcheng >= 0);
            Assert.True(combat.Stats.Jipo >= 0);
            Assert.True(combat.Stats.Strategy >= 0);
            Assert.True(combat.Stats.Construction >= 0);
        }

        [Fact]
        public void Wounded_do_not_count_toward_fighting_strength_or_power()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var full = UnitUtil.CreateCombat(1, "blade", 1000, commanderOfficerId: 0);
            var hurt = UnitUtil.CreateCombat(1, "blade", 1000, commanderOfficerId: 0);
            var cmd = new Officer(1);
            cmd.SetStats(atk: 80, intel: 60, lead: 70, pol: 50, charm: 50);
            full.SetCommander(cmd);
            hurt.SetCommander(cmd);
            full.SetManpower(1000, woundedCount: 0);
            hurt.SetManpower(1000, woundedCount: 500);

            Assert.Equal(1000, full.EffectiveCombatStrength);
            Assert.Equal(500, hurt.EffectiveCombatStrength);
            Assert.True(full.Stats.Attack > hurt.Stats.Attack);
        }

        [Fact]
        public void Stats_increase_with_morale_and_officer_stats()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var low = BuildCombat(morale: 50, stamina: 50, attack: 50, leadership: 50);
            var high = BuildCombat(morale: 100, stamina: 100, attack: 95, leadership: 95);

            Assert.True(high.Stats.Attack > low.Stats.Attack);
            Assert.True(high.Stats.Defense > low.Stats.Defense);
        }

        [Fact]
        public void CombatBattleFormulas_reads_stats_block()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var attacker = UnitUtil.CreateCombat(1, "blade", 1000, commanderOfficerId: 0);
            var defender = UnitUtil.CreateCombat(1, "blade", 1000, commanderOfficerId: 0);
            var cmd = new Officer(1);
            cmd.SetStats(atk: 80, intel: 60, lead: 70, pol: 50, charm: 50);
            attacker.SetCommander(cmd);
            defender.SetCommander(cmd);

            Assert.Equal(attacker.Stats.Attack, CombatBattleFormulas.CalculateAttack(attacker));
            Assert.Equal(defender.Stats.Defense, CombatBattleFormulas.CalculateDefense(defender));

            var damage = CombatBattleFormulas.CalculateNormalAttackDamage(attacker, defender);
            Assert.False(damage.HasEffect);
        }

        static Combat BuildCombat(short morale, short stamina, short attack, short leadership)
        {
            var combat = UnitUtil.CreateCombat(1, "blade", 5000, commanderOfficerId: 0);
            var cmd = new Officer(1);
            cmd.SetStats(atk: attack, intel: 60, lead: leadership, pol: 50, charm: 50);
            combat.SetCommander(cmd);
            combat.SetMorale(morale);
            combat.SetStamina(stamina);
            return combat;
        }
    }
}
