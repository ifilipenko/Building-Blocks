using BuildingBlocks.Query;

namespace BuildingBlocks.Membership.RavenDB.Queries.Criteria
{
    public class FindByEmailSubstring : PageCriteria
    {
        public FindByEmailSubstring(string emailSubstring, string applicationName, int pageNumber, int pageSize)
            : base(pageNumber, pageSize)
        {
            ApplicationName = applicationName;
            EmailSubstring = emailSubstring;
        }

        public string ApplicationName { get; private set; }
        public string EmailSubstring { get; private set; }
    }
}
