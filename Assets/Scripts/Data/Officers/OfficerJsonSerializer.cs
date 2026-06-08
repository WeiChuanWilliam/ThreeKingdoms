using System.Text.Json;

namespace ThreeKindoms.Data.Officers
{
    internal static class OfficerJsonSerializer
    {
        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNameCaseInsensitive = true,
            IncludeFields = true,
            ReadCommentHandling = JsonCommentHandling.Skip,
            AllowTrailingCommas = true
        };

        public static OfficerDefList DeserializeOfficers(string json) =>
            JsonSerializer.Deserialize<OfficerDefList>(json, Options) ?? new OfficerDefList();

        public static PersonalityTraitList DeserializePersonalities(string json) =>
            JsonSerializer.Deserialize<PersonalityTraitList>(json, Options) ?? new PersonalityTraitList();
    }
}
