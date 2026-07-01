using ThreeKindoms.Core.Officers;
using ThreeKindoms.Data.Officers;
using ThreeKindoms.Data.Units;
using Xunit;

namespace ThreeKindoms.Local.Tests
{
    public class OfficerSchemaTests
    {
        [Fact]
        public void OfficerDef_loads_birth_year_and_lifespan()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            OfficerDatabase.LoadCatalogAndMaterializeAll(
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath);

            Officer guan = OfficerDatabase.TryCreateRuntime(1);
            Assert.NotNull(guan);
            Assert.Equal(160, guan.BirthYear);
            Assert.Equal(59, guan.Lifespan);
            Assert.Equal(219, guan.DeathYear);
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
        public void Officer_relations_respect_caps_from_properties()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            var officer = new Officer(99);
            officer.SetGender(OfficerGender.Male);
            officer.SetRelations(new OfficerRelations
            {
                BelovedOfficerIds = new[] { 1, 2, 3, 4, 5, 6, 7 },
                SpouseOfficerIds = new[] { 10, 11, 12, 13 }
            });

            Assert.Equal(5, officer.BelovedOfficerIds.Count);
            Assert.Equal(3, officer.SpouseOfficerIds.Count);
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
                int roll = officer.RollRandom(3, 7);
                Assert.InRange(roll, 3, 7);
            }
        }

        [Fact]
        public void Relations_sync_bidirectional_after_database_load()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            OfficerDatabase.LoadCatalogAndMaterializeAll(
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath);
            OfficerDatabase.SyncAllRelations();

            Officer guan = OfficerDatabase.TryGetRuntime(1);
            Officer zhang = OfficerDatabase.TryGetRuntime(2);
            Officer zhuge = OfficerDatabase.TryGetRuntime(3);

            Assert.Contains(2, guan.BelovedOfficerIds);
            Assert.Contains(3, guan.BelovedOfficerIds);
            Assert.Contains(1, zhang.BelovedOfficerIds);
            Assert.Contains(1, zhuge.BelovedOfficerIds);
            Assert.Contains(2, guan.SwornBrotherIds);
            Assert.Contains(1, zhang.SwornBrotherIds);
            Assert.Contains(1, zhuge.SwornBrotherIds);
        }

        [Fact]
        public void Catalog_is_static_and_scenario_list_controls_runtime_pool()
        {
            OfficerConfigUtil.Load(TestPaths.OfficerPropertiesPath);
            OfficerDatabase.LoadCatalog(
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath);

            Assert.True(OfficerDatabase.IsCatalogLoaded);
            Assert.Equal(3, OfficerDatabase.Defs.Count);

            OfficerDatabase.MaterializeFromScenarioFile(TestPaths.ScenarioOfficersPath);
            Assert.Equal(3, OfficerDatabase.RuntimeCount);
            Assert.NotNull(OfficerDatabase.TryGetRuntime(1));

            OfficerDatabase.MaterializeFromIds(new[] { 1 });
            Assert.Equal(1, OfficerDatabase.RuntimeCount);
            Assert.Null(OfficerDatabase.TryGetRuntime(2));

            OfficerDatabase.ClearRuntime();
            Assert.Empty(OfficerDatabase.Runtime);
        }
    }
}
