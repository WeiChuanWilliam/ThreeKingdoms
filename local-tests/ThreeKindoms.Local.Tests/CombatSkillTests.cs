using ThreeKindoms.Core.Units;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    /// <summary>戰法 API 已暫停；此檔暫保留空測以免誤跑舊流程。</summary>
    public class CombatSkillTests
    {
        [Fact]
        public void Skill_api_paused_equipped_count_is_zero()
        {
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);
            var combat = UnitUtil.CreateCombat(1, "blade", 1000, commanderOfficerId: 0);
            Assert.Equal(0, combat.CountEquippedSkills());
        }
    }
}
