using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class CombatSkillTests
    {
        static Combat Blade(int soldiers = 100) =>
            UnitUtil.CreateCombat(1, "blade", soldiers, commanderOfficerId: 0);

        [Fact]
        public void Combat_unit_battle_skills_deduplicate_by_id()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);
            var combat = Blade();
            Assert.True(combat.AddBattleSkill(101));
            Assert.False(combat.AddBattleSkill(101));
            Assert.Equal(1, combat.CountEquippedSkills());
        }

        [Fact]
        public void Combat_tracks_equipped_skills_in_four_sets()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);
            var combat = Blade();
            combat.AddBattleSkill(101);
            combat.AddStrategySkill(201);
            Assert.Equal(2, combat.CountEquippedSkills());
        }

        [Fact]
        public void Combat_allows_only_one_vice_officer()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);
            var combat = Blade();
            Assert.True(combat.AddViceOfficer(new Officer(2)));
            Assert.False(combat.AddViceOfficer(new Officer(3)));
            Assert.Equal(2, combat.ViceOfficer.RuntimeId);
        }

        [Fact]
        public void SetViceOfficer_replaces_previous_vice()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);
            var combat = Blade();
            combat.SetViceOfficer(new Officer(2));
            combat.SetViceOfficer(new Officer(5));
            Assert.Equal(5, combat.ViceOfficer.RuntimeId);
            Assert.Single(combat.ViceOfficers);
        }
    }
}
