using System;

namespace BuildingBlocks.Store.RavenDB
{
    public class RavenDbSessionSettings
    {
        const int MaxStaleResultsWhaitSeconds = 30;
        private static readonly TimeSpan DefaultStaleResultsWhait = TimeSpan.FromSeconds(5);

        public RavenDbSessionSettings()
        {
            StaleResultWhaitMode = StaleResultWhaitMode.AtLastWrite;
            StaleResultsWhait = DefaultStaleResultsWhait;
        }

        public StaleResultWhaitMode StaleResultWhaitMode { get; private set; }
        public TimeSpan? StaleResultsWhait { get; private set; }

        public void SetStaleResultsWhait(StaleResultWhaitMode mode, TimeSpan? whait = null)
        {
            if (whait.HasValue && whait.Value.TotalSeconds > MaxStaleResultsWhaitSeconds)
            {
                StaleResultsWhait = DefaultStaleResultsWhait;
            }
            else
            {
                StaleResultsWhait = whait;
            }
            StaleResultWhaitMode = mode;
        }
    }

    public enum StaleResultWhaitMode
    {
        AtNow,
        AllNonStale,
        AtLastWrite
    }
}