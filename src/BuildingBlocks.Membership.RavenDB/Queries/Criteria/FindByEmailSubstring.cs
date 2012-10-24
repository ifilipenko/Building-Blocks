using BuildingBlocks.Query;

namespace BuildingBlocks.Membership.RavenDB.Queries.Criteria
{
    public class FindByEmailSubstring : PageCriteria
    {
        public FindByEmailSubstring(string emailSubstring, int pageNumber, int pageSize)
            : base(pageNumber, pageSize)
        {
            Email = emailSubstring;
        }

        public string Email { get; private set; }
    }
}
