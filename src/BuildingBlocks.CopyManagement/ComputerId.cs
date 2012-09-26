using System;

namespace BuildingBlocks.CopyManagement
{
    public static class ComputerId
    {
        private static readonly Lazy<string> _value;

        static ComputerId()
        {
            _value = new Lazy<string>(ComputeValue, true);
        }

        public static string Value
        {
            get { return _value.Value; }
        }

        private static string ComputeValue()
        {
            return "CPU >> " + HardwareIdentifiers.CpuId() +
                   "\nBIOS >> " + HardwareIdentifiers.BiosId() +
                   "\nBASE >> " + HardwareIdentifiers.MotherboardId();
        }
    }
}