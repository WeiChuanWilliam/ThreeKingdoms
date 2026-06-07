using System.Collections.Generic;
using ThreeKindoms.Core.Officers;
using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Battle;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class CombatSkillAssemblerTests
    {
        [Fact]
        public void CollectMergedOfficerSkillIds_deduplicates_commander_and_vice()
        {
            var combat = new Combat(new CombatUnitDef(1, "infantry", soldiers: 100));

            var cmd = new Officer(1);
            cmd.SetBattleSkills(new OfficerBattleSkills
            {
                ShieldForce = new BattleSkill { SkillId = 101 },
                CavalryForce = new BattleSkill { SkillId = 102 }
            });
            combat.SetCommander(cmd);

            var vice = new Officer(2);
            vice.SetBattleSkills(new OfficerBattleSkills
            {
                SpearForce = new BattleSkill { SkillId = 201 },
                ArcheryForce = new BattleSkill { SkillId = 102 }
            });
            combat.SetViceOfficer(vice);

            var merged = new HashSet<int>();
            CombatSkillAssembler.CollectMergedOfficerSkillIds(combat, merged);

            Assert.Equal(3, merged.Count);
            Assert.Contains(101, merged);
            Assert.Contains(102, merged);
            Assert.Contains(201, merged);
        }

        [Fact]
        public void RefreshEquippedSkills_updates_officer_skill_id_set()
        {
            var combat = new Combat(new CombatUnitDef(1, "infantry", soldiers: 100));

            var cmd = new Officer(1);
            cmd.SetBattleSkills(new OfficerBattleSkills
            {
                MarineForce = new BattleSkill { SkillId = 301 }
            });
            combat.SetCommander(cmd);

            CombatSkillAssembler.RefreshEquippedSkills(combat);

            Assert.Single(combat.OfficerSkillIds);
            Assert.Contains(301, combat.OfficerSkillIds);
            Assert.Equal(0, combat.CountEquippedSkills());
        }

        [Fact]
        public void Combat_allows_only_one_vice_officer()
        {
            var combat = new Combat(new CombatUnitDef(1, "infantry", soldiers: 100));
            Assert.True(combat.AddViceOfficer(new Officer(2)));
            Assert.False(combat.AddViceOfficer(new Officer(3)));
            Assert.Equal(2, combat.ViceOfficer.RuntimeId);
        }

        [Fact]
        public void SetViceOfficer_replaces_previous_vice()
        {
            var combat = new Combat(new CombatUnitDef(1, "infantry", soldiers: 100));
            combat.SetViceOfficer(new Officer(2));
            combat.SetViceOfficer(new Officer(5));
            Assert.Equal(5, combat.ViceOfficer.RuntimeId);
            Assert.Single(combat.ViceOfficers);
        }
    }
}
