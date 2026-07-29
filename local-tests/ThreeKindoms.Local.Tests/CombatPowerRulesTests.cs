using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class CombatPowerRulesTests
    {
        [Fact]
        public void TryCreateContext_includes_officer_morale_stamina_and_troop_stats()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var combat = UnitUtil.CreateCombat(1, "blade", 5000, commanderOfficerId: 0);
            combat.SetMorale(80);
            combat.SetStamina(70);

            var cmd = new Officer(1);
            cmd.SetStats(atk: 90, intel: 60, lead: 85, pol: 50, charm: 70);
            combat.SetCommander(cmd);

            var vice = new Officer(2);
            vice.SetStats(atk: 70, intel: 80, lead: 60, pol: 40, charm: 55);
            combat.SetViceOfficer(vice);

            Assert.True(combat.TryGetCombatPowerContext(out var ctx));
            Assert.Equal(90, ctx.Commander.EffectiveAttack);
            Assert.Equal(80, ctx.Vice.EffectiveIntelligence);
            Assert.Equal(80, ctx.Morale);
            Assert.Equal(70, ctx.Stamina);
            Assert.True(ctx.EffectiveTroopStats.Attack > 0);
            Assert.Equal(5000, ctx.EffectiveManpower);
        }

        [Fact]
        public void CombatPower_increases_with_morale_and_officer_stats()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var low = BuildCombat(morale: 50, stamina: 50, attack: 50, leadership: 50);
            var high = BuildCombat(morale: 100, stamina: 100, attack: 95, leadership: 95);

            Assert.True(high.CombatPower > low.CombatPower);
        }

        [Fact]
        public void CombatBattleFormulas_attack_defense_and_normal_attack_pipeline()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var attacker = UnitUtil.CreateCombat(1, "blade", 1000, commanderOfficerId: 0);
            var defender = UnitUtil.CreateCombat(1, "blade", 1000, commanderOfficerId: 0);

            Assert.Equal(attacker.EffectiveAttack, attacker.CalculateAttack());
            Assert.Equal(defender.EffectiveDefense, defender.CalculateDefense());

            var damage = CombatBattleFormulas.CalculateNormalAttackDamage(attacker, defender);
            Assert.False(damage.HasEffect);

            var resolved = CombatBattleFormulas.ResolveNormalAttack(attacker, defender);
            Assert.False(resolved.HasEffect);
            Assert.Equal(1000, defender.Soldiers);
        }

        static Combat BuildCombat(short morale, short stamina, short attack, short leadership)
        {
            var combat = UnitUtil.CreateCombat(1, "blade", 5000, commanderOfficerId: 0);
            combat.SetMorale(morale);
            combat.SetStamina(stamina);

            var cmd = new Officer(1);
            cmd.SetStats(atk: attack, intel: 60, lead: leadership, pol: 50, charm: 50);
            combat.SetCommander(cmd);
            return combat;
        }
    }
}
