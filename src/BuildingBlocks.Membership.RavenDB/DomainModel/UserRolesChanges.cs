using System.Collections.Generic;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class UserRolesChanges
    {
        public IEnumerable<string> RoleIdsToRemove { get; set; }
        public IEnumerable<string> RoleIdsToAdd { get; set; }
    }
}