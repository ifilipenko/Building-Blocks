using System;

namespace BuildingBlocks.SystemInfo
{
    public class OperationSystemInfo
    {
        public string Version { get; set; }
        public TimeSpan UpTime { get; set; }

        public string SystemDirectory { get; set; }
        public string PATHVariable { get; set; }
    }
}