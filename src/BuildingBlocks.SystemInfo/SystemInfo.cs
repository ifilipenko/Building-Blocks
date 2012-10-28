namespace BuildingBlocks.SystemInfo
{
    public class SystemInfo
    {
        public AppStartInfo AppStartInfo { get; set; }
        public UserInfo User { get; set; }
        public HardwareInfo Hardware { get; set; }
        public SoftwareInfo Software { get; set; }
        public NetworkInfo Network { get; set; }
    }
}