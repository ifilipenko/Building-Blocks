namespace BuildingBlocks.SystemInfo
{
    public class AppStartInfo
    {
        public string ClrVersion { get; set; }
        public long WorkingSet { get; set; }
        public string CommandLine { get; set; }
        public string CurrentDirectory { get; set; }
    }
}