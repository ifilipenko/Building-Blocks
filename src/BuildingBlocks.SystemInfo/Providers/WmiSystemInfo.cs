using System.Linq;
using System.Management;

namespace BuildingBlocks.SystemInfo.Providers
{
    class WmiSystemInfo
    {
        internal CpuInfo GetCpuInfo()
        {
            var cpuInfo = new CpuInfo();
            var query = new SelectQuery("Win32_processor");
            var search = new ManagementObjectSearcher(query);
            
            foreach (ManagementObject info in search.Get())
            {
                cpuInfo.Name = info["Name"].ToString();
                cpuInfo.NumberOfCores = info["NumberOfCores"].ToString();
                cpuInfo.Description = info["Description"].ToString();
            }
            return cpuInfo;
        }

        internal MemoryInfo GetMemoryInfoFromLogicalMemoryConfigurationClass()
        {
            var memoryInfo = new MemoryInfo();
            var query = new SelectQuery("Win32_LogicalMemoryConfiguration");
            var search = new ManagementObjectSearcher(query);
            
            foreach (ManagementObject info in search.Get())
            {
                memoryInfo.TotalPageFileSpace     = info["TotalPageFileSpace"].ToString();
                memoryInfo.TotalPhysicalMemory    = info["TotalPhysicalMemory"].ToString();
                memoryInfo.TotalVirtualMemory     = info["TotalVirtualMemory"].ToString();
                memoryInfo.AvailableVirtualMemory = info["AvailableVirtualMemory"].ToString();
            }              

            return memoryInfo;
        }

        public MemoryInfo GetMemoryInfoFromOperatingSystemClass()
        {
            var memoryInfo = new MemoryInfo();
            var query = new SelectQuery("Win32_OperatingSystem");
            var search = new ManagementObjectSearcher(query);
            foreach (ManagementObject info in search.Get())
            {
                memoryInfo.TotalPageFileSpace     = info["SizeStoredInPagingFiles"].ToString();
                memoryInfo.TotalPhysicalMemory    = info["TotalVisibleMemorySize"].ToString();
                memoryInfo.TotalVirtualMemory     = info["TotalVirtualMemorySize"].ToString();
                memoryInfo.AvailableVirtualMemory = info["FreeVirtualMemory"].ToString();
            }

            return memoryInfo;
        }

        internal string GetMACAddress()
        {
            var managementClass = new ManagementClass("Win32_NetworkAdapterConfiguration");
            var managementObjectCollection = managementClass.GetInstances();
            
            return managementObjectCollection
                .OfType<ManagementObject>()
                .Where(mo => (bool)mo["IPEnabled"])
                .Select(mo => mo["MacAddress"].ToString())
                .Select(mac => mac.Replace(":", string.Empty))
                .FirstOrDefault(mac => !string.IsNullOrEmpty(mac));
        }
    }
}