namespace BuildingBlocks.SystemInfo
{
    public class UserInfo
    {
        public string UserName { get; set; }
        public string[] UserGroups { get; set; }
        public bool IsAdmin { get; set; }
        public string UserDomainName { get; set; }
        public string AuthenticationType { get; set; }
        public bool IsSystemUser { get; set; }
        public bool IsGuest { get; set; }
    }
}