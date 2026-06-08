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
            OfficerDatabase db = OfficerDatabase.LoadFromFile(
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath);

            Officer guan = db.GetOrCreateRuntime(1);
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
    }
}
