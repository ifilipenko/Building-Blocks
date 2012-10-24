namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class RoleReference
    {
        private RoleReference()
        {
        }

        public RoleReference(RoleEntity role)
        {
            Id = role.Id;
            Name = role.RoleName;
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}