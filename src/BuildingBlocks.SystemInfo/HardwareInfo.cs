using System.Collections.Generic;

namespace BuildingBlocks.SystemInfo
{
    public class HardwareInfo
    {
        public CpuInfo Processor { get; set; }
        public MemoryInfo MemoryInfo { get; set; }
        public IEnumerable<DriveInfo> Drives { get; set; }
        public IEnumerable<ScreenInfo> Monitors { get; set; }
    }
}