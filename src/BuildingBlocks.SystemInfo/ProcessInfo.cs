using System;

namespace BuildingBlocks.SystemInfo
{
    public class ProcessInfo
    {
        public string Id { get; set; }
        public TimeSpan ExecutionTime { get; set; }
        public string ProcessName { get; set; }
        public string ProcessUser { get; set; }
        public string ProcessPath { get; set; }
    }
}