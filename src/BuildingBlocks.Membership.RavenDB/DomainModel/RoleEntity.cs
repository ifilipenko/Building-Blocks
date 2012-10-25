using System;
using System.Collections.Generic;
using System.Linq;
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

        public void RemoveUser(UserEntity user)
        {
            foreach (var userReference in Users.Where(u => u.Id == user.Id).ToList())
            {
                Users.Remove(userReference);
            }
        }

        public void AddUserOrUpdate(UserEntity user)
        {
            var existsUsers = Users.Where(u => u.Id == user.Id).ToList();
            if (existsUsers.Any())
            {
                foreach (var userReference in existsUsers)
                {
                    userReference.Name = user.Username;
                }
            }
            else
            {
                Users.Add(new UserReference(user));
            }
        }
    }
}
