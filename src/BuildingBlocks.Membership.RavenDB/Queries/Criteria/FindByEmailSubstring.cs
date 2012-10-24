using BuildingBlocks.Query;

namespace BuildingBlocks.Membership.RavenDB.Queries.Criteria
{
    public class FindByEmailSubstring : PageCriteria
    {
        public FindByEmailSubstring(string emailSubstring, int pageNumber, int pageSize)
            : base(pageNumber, pageSize)
        {
            EmailSubstring = emailSubstring;
        }

        public string EmailSubstring { get; private set; }
    }
}
