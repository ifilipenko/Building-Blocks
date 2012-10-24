namespace BuildingBlocks.Membership.RavenDB.Queries.Criteria
{
    public class UsernameMatch
    {
        public UsernameMatch(string username)
        {
            Username = username;
        }

        public string Username { get; private set; }
    }
}