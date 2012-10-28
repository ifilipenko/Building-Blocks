using BuildingBlocks.Query;

namespace BuildingBlocks.Membership.RavenDB.Queries.Criteria
{
    public class FindByUsernameSubstring : PageCriteria
    {
        public FindByUsernameSubstring(string usernameSubstring, string applicationName, int pageNumber, int pageSize)
            : base(pageNumber, pageSize)
        {
            ApplicationName = applicationName;
            UsernameSubstring = usernameSubstring;
        }

        public string ApplicationName { get; private set; }
        public string UsernameSubstring { get; private set; }
    }
}