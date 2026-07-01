using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ThreeKindoms.Data.Officers
{
    /// <summary>劇本武將清單 JSON：指定本劇本要從圖鑑 materialize 哪些 defId。</summary>
    public sealed class ScenarioOfficerListDocument
    {
        public string scenarioId { get; set; } = "";
        public int[] officerIds { get; set; } = Array.Empty<int>();
    }

    public static class ScenarioOfficerListLoader
    {
        public static IReadOnlyList<int> LoadOfficerIds(string absolutePath)
        {
            if (!File.Exists(absolutePath))
                return Array.Empty<int>();

            var doc = JsonSerializer.Deserialize<ScenarioOfficerListDocument>(
                File.ReadAllText(absolutePath),
                OfficerJsonSerializer.Options);

            return doc?.officerIds ?? Array.Empty<int>();
        }
    }
}
