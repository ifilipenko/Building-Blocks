namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class UserReference
    {
        private UserReference()
        {
        }

        public UserReference(UserEntity user)
        {
            Id = user.Id;
            Name = user.Username;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}