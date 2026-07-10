using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Officers;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class OfficerSchemaTests
    {
        [Fact]
        public void OfficerDef_loads_birth_year_and_death_year()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            OfficerDatabase.Load(
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath);

            Officer guan = OfficerDatabase.TryGet(2);
            Assert.NotNull(guan);
            Assert.Equal(160, guan.BirthYear);
            Assert.Equal(219, guan.DeathYear);
            Assert.Equal(59, guan.AgeAtDeath);
            Assert.Equal(TroopAptitudeGrade.S, guan.GetTroopAptitude(TroopType.Infantry));
            Assert.Equal(TroopAptitudeGrade.S, guan.GetTroopAptitude(TroopType.Cavalry));
            Assert.Equal(TroopAptitudeGrade.B, guan.GetTroopAptitude(TroopType.Archer));
        }

        [Fact]
        public void Signature_troop_requires_S_aptitude_and_tech()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            OfficerSignatureTroopRules.EnsureBuilt();

            var officer = new Officer(1);
            officer.SetTroopAptitude(new OfficerTroopAptitude
            {
                Cavalry = TroopAptitudeGrade.A
            });

            Assert.True(OfficerSignatureTroopRules.IsSignatureTroop("horseman.baima"));
            Assert.False(OfficerSignatureTroopRules.CanOfficerLeadTroopKind(
                officer, "horseman.baima", _ => true));

            officer.SetTroopAptitude(new OfficerTroopAptitude { Cavalry = TroopAptitudeGrade.S });
            Assert.False(OfficerSignatureTroopRules.CanOfficerLeadTroopKind(
                officer, "horseman.baima", _ => false));
            Assert.True(OfficerSignatureTroopRules.CanOfficerLeadTroopKind(
                officer, "horseman.baima", tech => tech == "tech.horseman_baima"));

            Assert.True(OfficerSignatureTroopRules.CanOfficerLeadTroopKind(
                officer, "blade", _ => false));
        }

        [Fact]
        public void Officer_item_ids_use_set_semantics()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            var officer = new Officer(1);
            OfficerItemLoader.ApplyFromIds(officer, new[] { 10, 10, 20, 30 });

            Assert.Equal(3, officer.ItemIds.Count);
        }

        [Fact]
        public void RemovePersonality_syncs_ids_and_defs_then_can_add_new()
        {
            var officer = new Officer(1);
            officer.AddPersonality(10, "old_trait", "舊個性");

            Assert.True(officer.RemovePersonality(10));
            Assert.Empty(officer.Personalities);

            officer.AddPersonality(20, "new_trait", "新個性");

            Assert.Single(officer.Personalities);
            Assert.True(officer.HasPersonalityId(20));
            foreach (PersonalityDef p in officer.Personalities)
            {
                if (p.Id == 20)
                    Assert.Equal("新個性", p.DisplayName);
            }
        }

        [Fact]
        public void Performance_drops_with_injury_and_low_stamina()
        {
            var officer = new Officer(1);
            officer.SetStats(100, 100, 100, 100, 100, 100);
            officer.SetInjury(OfficerInjuryState.Severe);
            officer.SetStamina(0);
            Assert.True(officer.AttackPerform < 100);
        }

        [Fact]
        public void RollRandom_returns_value_within_inclusive_range()
        {
            var officer = new Officer(1);
            for (int i = 0; i < 50; i++)
            {
                int roll = officer.RollRandom(3, 7, 0);
                Assert.InRange(roll, 3, 7);
            }
        }

        [Fact]
        public void Load_keeps_def_and_runtime_pools_with_same_ids()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            OfficerDatabase.Load(
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath);

            Assert.True(OfficerDatabase.IsLoaded);
            Assert.Equal(12, OfficerDatabase.DefCount);
            Assert.Equal(12, OfficerDatabase.Count);

            OfficerDef guanDef = OfficerDatabase.TryGetDef(2);
            Officer guan = OfficerDatabase.TryGet(2);
            Assert.NotNull(guanDef);
            Assert.NotNull(guan);
            Assert.Equal(97, guanDef.attack);
            Assert.Equal((byte)97, guan.Attack);

            guan.SetStats(50, 50, 50, 50, 50, 100);
            Assert.Equal(97, guanDef.attack);
            Assert.Equal((byte)50, guan.Attack);

            Assert.Equal("玄德", OfficerDatabase.TryGet(1).AliasName);
            Assert.Equal("玄德", OfficerDatabase.TryGetDef(1).aliasName);

            OfficerDatabase.Clear();
            Assert.Empty(OfficerDatabase.Defs);
            Assert.Empty(OfficerDatabase.Officers);
        }
    }
}
