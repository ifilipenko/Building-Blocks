using System;
using System.Management;
using Common.Logging;

namespace BuildingBlocks.CopyManagement
{
    public static class HardwareIdentifiers
    {
        private static ILog _log = LogManager.GetLogger(typeof (HardwareIdentifiers));

        public static string CpuId()
        {
            var result = GetIdentifier("Win32_Processor", "UniqueId");
            if (result == "")
            {
                result = GetIdentifier("Win32_Processor", "ProcessorId");
                if (result == "")
                {
                    result = GetIdentifier("Win32_Processor", "Name");
                    if (result == "")
                    {
                        result = GetIdentifier("Win32_Processor", "Manufacturer");
                    }
                    result += GetIdentifier("Win32_Processor", "MaxClockSpeed");
                }
            }
            return result;
        }

        public static string BiosId()
        {
            return GetIdentifier("Win32_BIOS", "Manufacturer")
                   + GetIdentifier("Win32_BIOS", "SMBIOSBIOSVersion")
                   + GetIdentifier("Win32_BIOS", "IdentificationCode")
                   + GetIdentifier("Win32_BIOS", "SerialNumber")
                   + GetIdentifier("Win32_BIOS", "ReleaseDate")
                   + GetIdentifier("Win32_BIOS", "Version");
        }

        public static string DiskId()
        {
            return GetIdentifier("Win32_DiskDrive", "Model")
                   + GetIdentifier("Win32_DiskDrive", "Manufacturer")
                   + GetIdentifier("Win32_DiskDrive", "Signature")
                   + GetIdentifier("Win32_DiskDrive", "TotalHeads");
        }

        public static string MotherboardId()
        {
            return GetIdentifier("Win32_BaseBoard", "Model")
                   + GetIdentifier("Win32_BaseBoard", "Manufacturer")
                   + GetIdentifier("Win32_BaseBoard", "Name")
                   + GetIdentifier("Win32_BaseBoard", "SerialNumber");
        }

        public static string PrimaryVideoControllerId()
        {
            return GetIdentifier("Win32_VideoController", "DriverVersion")
                   + GetIdentifier("Win32_VideoController", "Name");
        }

        public static string FirstEnabledNetworkCardId()
        {
            return GetIdentifier("Win32_NetworkAdapterConfiguration",
                                 "MACAddress", "IPEnabled");
        }

        public static string GetIdentifier(string wmiClass, string wmiProperty, string wmiMustBeTrue)
        {
            if (_log.IsTraceEnabled)
            {
                _log.Trace(m => m(string.Format("wmiClass:{0}, wmiProperty:{1}, wmiMustBeTrue:{2}", wmiClass, wmiProperty, wmiMustBeTrue)));
            }

            string result = "";
            var managementClass = new ManagementClass(wmiClass);
            var moc = managementClass.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mo[wmiMustBeTrue].ToString() == "True")
                {
                    //Only get the first one
                    if (result == "")
                    {
                        try
                        {
                            result = mo[wmiProperty].ToString();
                            break;
                        }
                        catch(Exception ex)
                        {
                            _log.Warn(ex);
                        }
                    }
                }
            }
            return result;
        }

        public static string GetIdentifier(string wmiClass, string wmiProperty)
        {
            if (_log.IsTraceEnabled)
            {
                _log.Trace(m => m(string.Format("wmiClass:{0}, wmiProperty:{1}", wmiClass, wmiProperty)));
            }

            string result = "";
            var mc = new ManagementClass(wmiClass);
            var moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                //Only get the first one
                if (result == "")
                {
                    try
                    {
                        result = mo[wmiProperty].ToString();
                        break;
                    }
                    catch (Exception ex)
                    {
                        _log.Warn(ex);
                    }
                }
            }
            return result;
        }
    }
}