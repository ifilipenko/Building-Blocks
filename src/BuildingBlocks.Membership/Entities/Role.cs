using System;
using System.Collections.Generic;

namespace Mic.Infrastructure.Membership.Entities
{
    public class Role
    {
        public virtual Guid RoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string Description { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}