using ThreeKindoms.Data.Skill;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class SkillConfigUtilTests
    {
        [Fact]
        public void Cavalry_kind_gets_B_A_and_signature_S_names()
        {
            Assert.True(SkillConfigUtil.Load(TestPaths.SkillPropertiesPath));
            UnitConfigUtil.Load(TestPaths.UnitPropertiesPath);

            var names = SkillConfigUtil.GetSkillNamesForTroopKind("horseman.baima");
            Assert.Equal(3, names.Count);
            Assert.Equal("強襲", names[0]);
            Assert.Equal("衝鋒", names[1]);
            Assert.Equal("騎射", names[2]);
            Assert.Equal("騎射", SkillConfigUtil.GetSignatureSkillName("horseman.baima"));
        }

        [Fact]
        public void Archary_key_uses_requested_spelling_and_wudan_signature()
        {
            Assert.True(SkillConfigUtil.Load(TestPaths.SkillPropertiesPath));
            var names = SkillConfigUtil.GetSkillNamesForTroopKind("bow.wudan");
            Assert.Equal(new[] { "齊射", "火矢", "破軍" }, names);
            Assert.Equal("齊射", SkillConfigUtil.GetSharedSkillName(TroopType.Archer, "B"));
        }

        [Fact]
        public void Siege_kind_has_only_one_skill()
        {
            Assert.True(SkillConfigUtil.Load(TestPaths.SkillPropertiesPath));
            var names = SkillConfigUtil.GetSkillNamesForTroopKind("siege.stone");
            Assert.Single(names);
            Assert.Equal("投石", names[0]);
        }
    }
}
