using System.Collections.Generic;

namespace BuildingBlocks.SystemInfo
{
    public class SoftwareInfo
    {
        public OperationSystemInfo OS { get; set; }
        public IEnumerable<ProcessInfo> Processes { get; set; }
    }
}