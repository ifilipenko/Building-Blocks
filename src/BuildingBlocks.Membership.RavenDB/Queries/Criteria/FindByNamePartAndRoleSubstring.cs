using BuildingBlocks.Query;

namespace BuildingBlocks.Membership.RavenDB.Queries.Criteria
{
    public class FindByNamePartAndRoleSubstring : PageCriteria
    {

        public FindByNamePartAndRoleSubstring(string usernameToMatch, string applicationName, string roleName, int pageNumber, int pageSize) 
            : base(pageNumber, pageSize)
        {
            UsernameSubstring = usernameToMatch;
            ApplicationName = applicationName;
            RoleName = roleName;
        }

        public string UsernameSubstring { get; private set; }
        public string ApplicationName { get; private set; }
        public string RoleName { get; private set; }
    }
}