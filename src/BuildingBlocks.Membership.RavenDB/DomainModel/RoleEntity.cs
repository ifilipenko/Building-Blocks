using System;
using System.Collections.Generic;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class RoleEntity : IEntity<string>
    {
        public RoleEntity()
        {
            Users = new List<UserReference>(0);
        }

        public virtual string Id { get; set; }
        public virtual Guid RoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string Description { get; set; }
        public virtual IList<UserReference> Users { get; set; }
    }
}
