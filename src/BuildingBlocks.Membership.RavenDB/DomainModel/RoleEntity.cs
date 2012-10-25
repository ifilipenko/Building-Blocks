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

        public string Id { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; }
        public string ApplicationName { get; set; }
        public string Description { get; set; }
        public IList<UserReference> Users { get; set; }

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
