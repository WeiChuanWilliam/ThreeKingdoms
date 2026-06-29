namespace ThreeKindoms.Core
{
    public enum DayPhase
    {
        Sunrise,
        Daytime,
        Sunset
    }

    /// <summary>遊戲日曆：總日數、時段與旬內日序。</summary>
    public sealed class GameClock
    {
        /// <summary>自開局起累計日數（從 1 起算）。</summary>
        public int TotalDay { get; private set; } = 1;

        /// <summary>當日時段（日出、日間、日落）。</summary>
        public DayPhase Phase { get; private set; } = DayPhase.Sunrise;

        /// <summary>時鐘是否暫停（Spike 預設暫停）。</summary>
        public bool IsPaused { get; private set; } = true;

        /// <summary>旬內第幾日（1～10）。</summary>
        public int DayInXun => (TotalDay - 1) % 10 + 1;

        /// <summary>是否為旬首日。</summary>
        public bool IsXunFirstDay => DayInXun == 1;

        /// <summary>暫停時鐘推進。</summary>
        public void Pause() => IsPaused = true;

        /// <summary>恢復時鐘推進。</summary>
        public void Resume() => IsPaused = false;

        /// <summary>日出：進入新一日開始（Spike：手動呼叫）。</summary>
        public void AdvanceToNextSunrise()
        {
            if (TotalDay > 1 || Phase != DayPhase.Sunrise)
                TotalDay++;
            Phase = DayPhase.Sunrise;
            IsPaused = true;
        }

        /// <summary>進入日間時段。</summary>
        public void BeginDaytime() => Phase = DayPhase.Daytime;
    }
}
