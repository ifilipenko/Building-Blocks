using System.IO;

namespace BuildingBlocks.SystemInfo
{
    public class DriveInfo
    {
        public string Name { get; set; }
        public string VolumeLabel { get; set; }
        public DriveType DriveType { get; set; }
        public string Format { get; set; }
        public double AvailableFreeSpaceMb { get; set; }
        public double TotalFreeSpaceMb { get; set; }
        public double TotalSizeMb { get; set; }
    }
}