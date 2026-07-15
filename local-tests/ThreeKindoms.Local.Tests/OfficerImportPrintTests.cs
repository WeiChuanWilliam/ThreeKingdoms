using ThreeKindoms.Local.Tests.Runners;
using Xunit;
using Xunit.Abstractions;

namespace ThreeKindoms.Local.Tests
{
    /// <summary>載入 officers.json 並印出每位武將完整摘要（供人工檢查）。</summary>
    public class OfficerImportPrintTests
    {
        readonly ITestOutputHelper _output;

        public OfficerImportPrintTests(ITestOutputHelper output) => _output = output;

        [Fact]
        public void Print_all_officers_from_json()
        {
            GameTestResult result = OfficerTestRunner.Run(
                TestPaths.OfficerPropertiesPath,
                TestPaths.OfficersJsonPath,
                TestPaths.PersonalityTraitsPath);

            _output.WriteLine(result.Report);
            Assert.True(result.Passed, result.Report);
        }
    }
}
