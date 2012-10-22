using System;
using System.Collections.Generic;

namespace BuildingBlocks.Membership.Entities
{
    public class Role
    {
        private IList<string> _users;

        public Role()
        {
            _users = new List<string>();
        }

        public virtual Guid RoleId { get; set; }
        public virtual string RoleName { get; set; }
        public virtual string Description { get; set; }

        public virtual IEnumerable<string> Users
        {
            get { return _users; }
            set { _users = (IList<string>)value; }
        }
    }
}