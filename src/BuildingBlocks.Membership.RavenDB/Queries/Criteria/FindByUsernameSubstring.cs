using BuildingBlocks.Query;

namespace BuildingBlocks.Membership.RavenDB.Queries.Criteria
{
    public class FindByUsernameSubstring : PageCriteria
    {
        public FindByUsernameSubstring(string usernameSubstring, int pageNumber, int pageSize)
            : base(pageNumber, pageSize)
        {
            UsernameSubstring = usernameSubstring;
        }

        public string UsernameSubstring { get; private set; }
    }
}