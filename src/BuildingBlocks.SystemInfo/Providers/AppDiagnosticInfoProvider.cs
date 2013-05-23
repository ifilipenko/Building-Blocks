using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Windows.Forms;

namespace BuildingBlocks.SystemInfo.Providers
{
    public class AppDiagnosticInfoProvider
    {
        public SystemInfo GetFullSystemInfo()
        {
            var systemInfo = new SystemInfo
            {
                AppStartInfo = GetAppStartInfo(),
                User         = GetUserInfo(),
                Software     = GetSoftwareInfo(),
                Network      = GetNetworkInfo(),
                Hardware     = GetHardwareInfo()
            };
            return systemInfo;
        }

        public HardwareInfo GetHardwareInfo()
        {
            var wmiSystemInfo = new WmiSystemInfo();
            var hardwareInfo = new HardwareInfo
            {
                Monitors   = GetModitorsInfo(),
                Processor  = wmiSystemInfo.GetCpuInfo(),
                MemoryInfo = GetMemoryInfo(),
                Drives     = GetDrivesInfo()
            };
            return hardwareInfo;
        }

        public IEnumerable<ScreenInfo> GetModitorsInfo()
        {
            return Screen.AllScreens
                .Select(scr => new ScreenInfo
                {
                    Name         = scr.DeviceName,
                    Primary      = scr.Primary,
                    ResolutionX  = scr.Bounds.Width,
                    ResolutionY  = scr.Bounds.Height,
                    WorkingAreaX = scr.WorkingArea.Width,
                    WorkingAreaY = scr.WorkingArea.Height
                })
                .ToList();
        }

        public IEnumerable<DriveInfo> GetDrivesInfo()
        {
            const double bytesPerMegabyte = 1024 * 1024;

            return System.IO.DriveInfo.GetDrives()
                .Where(d => d.IsReady)
                .Select(d => new DriveInfo
                {
                    Name                 = d.Name,
                    VolumeLabel          = d.VolumeLabel,
                    DriveType            = d.DriveType,
                    Format               = d.DriveFormat,
                    AvailableFreeSpaceMb = d.AvailableFreeSpace/bytesPerMegabyte,
                    TotalFreeSpaceMb     = d.TotalFreeSpace/bytesPerMegabyte,
                    TotalSizeMb          = d.TotalSize/bytesPerMegabyte
                })
                .ToList();
        }

        public MemoryInfo GetMemoryInfo()
        {
            var wmiSystemInfo = new WmiSystemInfo();
            try
            {
                return wmiSystemInfo.GetMemoryInfoFromLogicalMemoryConfigurationClass();
            }
            catch (ManagementException)
            {
                try
                {
                    return wmiSystemInfo.GetMemoryInfoFromOperatingSystemClass();
                }
                catch
                {
                    return new MemoryInfo();
                }
            }
        }

        public NetworkInfo GetNetworkInfo()
        {
            var wmiSystemInfo = new WmiSystemInfo();

            var networkInfo = new NetworkInfo();
            networkInfo.ComputerName = Environment.MachineName;
            networkInfo.MacAddress = wmiSystemInfo.GetMACAddress();
            networkInfo.IpAddress = GetIpAddress();
            return networkInfo;
        }

        public string GetIpAddress()
        {
            try
            {
                var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
                var ipAddress = (from addr in hostEntry.AddressList
                                 where addr.AddressFamily == AddressFamily.InterNetwork
                                 select addr.ToString()).FirstOrDefault();
                return ipAddress;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public SoftwareInfo GetSoftwareInfo()
        {
            var softwareInfo = new SoftwareInfo
            {
                OS        = GetOsInfo(),
                Processes = GetProcesses().ToList()
            };
            return softwareInfo;
        }

        public IEnumerable<ProcessInfo> GetProcesses()
        {
            foreach (var process in Process.GetProcesses())
            {
                ProcessInfo processInfo;
                try
                {
                    processInfo = new ProcessInfo
                                      {
                                          Id            = process.Id.ToString(),
                                          ExecutionTime = DateTime.Now - process.StartTime,
                                          ProcessName   = process.ProcessName,
                                          ProcessPath   = process.StartInfo.FileName,
                                          ProcessUser   = process.StartInfo.UserName
                                      };
                }
                catch
                {
                    processInfo = null;
                }

                if (processInfo != null)
                {
                    yield return processInfo;
                }
            }
        }

        public OperationSystemInfo GetOsInfo()
        {
            var osInfo = new OperationSystemInfo
                             {
                                 Version = Environment.OSVersion.ToString(),
                                 UpTime = new TimeSpan(Environment.TickCount),
                                 SystemDirectory = Environment.SystemDirectory,
                                 PATHVariable = Environment.GetEnvironmentVariable("PATH")
                             };
            return osInfo;
        }

        public AppStartInfo GetAppStartInfo()
        {
            var appStartInfo = new AppStartInfo
            {
                ClrVersion       = Environment.Version.ToString(),
                WorkingSet       = Environment.WorkingSet,
                CommandLine      = Environment.CommandLine,
                CurrentDirectory = Environment.CurrentDirectory
            };
            return appStartInfo;
        }

        public UserInfo GetUserInfo()
        {
            var userInfo = new UserInfo
            {
                UserName       = Environment.UserName,
                UserDomainName = Environment.UserDomainName
            };

            var windowsIdentity = WindowsIdentity.GetCurrent();
            if (windowsIdentity != null)
            {
                var principal               = new WindowsPrincipal(windowsIdentity);
                userInfo.IsGuest            = windowsIdentity.IsGuest;
                userInfo.IsSystemUser       = windowsIdentity.IsSystem;
                userInfo.AuthenticationType = windowsIdentity.AuthenticationType;
                userInfo.IsAdmin            = principal.IsInRole(WindowsBuiltInRole.Administrator);
                if (windowsIdentity.Groups != null)
                {
                    userInfo.UserGroups = windowsIdentity.Groups
                        .Select(g => (NTAccount) g.Translate(typeof (NTAccount)))
                        .Select(g => g.Value)
                        .ToArray();
                }
            }
            return userInfo;
        }
    }
}