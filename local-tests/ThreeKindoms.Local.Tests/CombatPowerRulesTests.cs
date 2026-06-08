using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Battle;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class CombatPowerRulesTests
    {
        [Fact]
        public void TryCreateContext_includes_officer_morale_stamina_and_skills()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 5000));
            combat.SetMorale(80);
            combat.SetStamina(70);

            var cmd = new Officer(1);
            cmd.SetStats(atk: 90, intel: 60, lead: 85, pol: 50, charm: 70);
            cmd.SetBattleSkills(new OfficerBattleSkills
            {
                ShieldForce = new BattleSkill { SkillId = 101 }
            });
            combat.SetCommander(cmd);

            var vice = new Officer(2);
            vice.SetStats(atk: 70, intel: 80, lead: 60, pol: 40, charm: 55);
            vice.SetBattleSkills(new OfficerBattleSkills
            {
                ArcheryForce = new BattleSkill { SkillId = 201 }
            });
            combat.SetViceOfficer(vice);

            Assert.True(combat.TryGetCombatPowerContext(out var ctx));
            Assert.Equal(90, ctx.CommanderAbilities.Attack);
            Assert.Equal(80, ctx.ViceAbilities.Intelligence);
            Assert.Equal(80, ctx.Morale);
            Assert.Equal(70, ctx.Stamina);
            Assert.Equal(2, ctx.OfficerSkillIds.Count);
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
        public void CombatPower_deduplicates_officer_skills_in_context()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 1000));
            var cmd = new Officer(1);
            cmd.SetBattleSkills(new OfficerBattleSkills
            {
                ShieldForce = new BattleSkill { SkillId = 101 },
                CavalryForce = new BattleSkill { SkillId = 101 }
            });
            combat.SetCommander(cmd);

            Assert.True(combat.TryGetCombatPowerContext(out var ctx));
            Assert.Single(ctx.OfficerSkillIds);
        }

        static Combat BuildCombat(short morale, short stamina, short attack, short leadership)
        {
            var combat = new Combat(new CombatUnitDef(1, "blade", soldiers: 5000));
            combat.SetMorale(morale);
            combat.SetStamina(stamina);

            var cmd = new Officer(1);
            cmd.SetStats(atk: attack, intel: 60, lead: leadership, pol: 50, charm: 50);
            combat.SetCommander(cmd);
            return combat;
        }
    }
}
