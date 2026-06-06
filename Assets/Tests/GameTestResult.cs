namespace ThreeKindoms.Tests
{
    public readonly struct GameTestResult
    {
        public string Name { get; }
        public bool Passed { get; }
        public string Report { get; }
        public int OkCount { get; }
        public int ErrorCount { get; }

        public GameTestResult(string name, bool passed, string report, int okCount = 0, int errorCount = 0)
        {
            Name = name;
            Passed = passed;
            Report = report ?? "";
            OkCount = okCount;
            ErrorCount = errorCount;
        }
    }
}
