using System;
using System.Collections.Generic;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class RoleEntity : IEntity<string>
    {
        public string Id { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return string.Format("RoleName: {0}", RoleName);
        }
    }
}
