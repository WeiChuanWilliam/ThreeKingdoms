namespace ThreeKindoms.Core
{
    public enum DayPhase
    {
        Sunrise,
        Daytime,
        Sunset
    }

    public sealed class GameClock
    {
        public int TotalDay { get; private set; } = 1;
        public DayPhase Phase { get; private set; } = DayPhase.Sunrise;
        public bool IsPaused { get; private set; } = true;

        public int DayInXun => (TotalDay - 1) % 10 + 1;
        public bool IsXunFirstDay => DayInXun == 1;

        public void Pause() => IsPaused = true;
        public void Resume() => IsPaused = false;

        /// <summary>日出：進入新一日開始（Spike：手動呼叫）。</summary>
        public void AdvanceToNextSunrise()
        {
            if (TotalDay > 1 || Phase != DayPhase.Sunrise)
                TotalDay++;
            Phase = DayPhase.Sunrise;
            IsPaused = true;
        }

        public void BeginDaytime() => Phase = DayPhase.Daytime;
    }
}
