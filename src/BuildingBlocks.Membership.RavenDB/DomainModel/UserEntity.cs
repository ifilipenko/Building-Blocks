using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class UserEntity : IEntity<string>
    {
        public UserEntity()
        {
            Roles = new List<RoleReference>();
        }

        public string Id { get; set; }
        public Guid UserId { get; set; }

        public string Username { get; set; }
        public string Email { get; set; }

        public string Password { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Comment { get; set; }

        public bool IsApproved { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public DateTime? LastPasswordFailureDate { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastLockoutDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public string ConfirmationToken { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime? LastPasswordChangedDate { get; set; }
        public string PasswordVerificationToken { get; set; }
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        public IList<RoleReference> Roles { get; set; }

        public void UpdateUser(User user)
        {
            Username = user.Username;
            Email = user.Email;
            Password = user.Password;
            Comment = user.Comment;
            ConfirmationToken = user.ConfirmationToken;
            CreateDate = user.CreateDate;
            IsApproved = user.IsApproved;
            IsLockedOut = user.IsLockedOut;
            LastActivityDate = user.LastActivityDate;
            LastLockoutDate = user.LastLockoutDate;
            LastLoginDate = user.LastLoginDate;
            LastPasswordChangedDate = user.LastPasswordChangedDate;
            LastPasswordFailureDate = user.LastPasswordFailureDate;
            PasswordFailuresSinceLastSuccess = user.PasswordFailuresSinceLastSuccess;
            PasswordVerificationToken = user.PasswordVerificationToken;
            PasswordVerificationTokenExpirationDate = user.PasswordVerificationTokenExpirationDate;
        }

        public IEnumerable<string> GetRoleIdsToRemove(IEnumerable<RoleEntity> newRoles)
        {
            return Roles
                .Where(exist => newRoles.All(newRole => newRole.Id != exist.Id))
                .Select(r => r.Id)
                .ToList();
        }

        public void AddRoleOrUpdate(RoleEntity role)
        {
            var roles = Roles.Where(r => r.Id == role.Id).ToList();
            if (roles.Any())
            {
                foreach (var roleReference in roles)
                {
                    roleReference.Name = role.RoleName;
                }
            }
            else
            {
                Roles.Add(new RoleReference(role));
            }
        }

        public void RemoveRole(RoleEntity role)
        {
            foreach (var roleReference in Roles.Where(r => r.Id == role.Id).ToList())
            {
                Roles.Remove(roleReference);
            }
        }

        public void RemoveRoleWithId(string roleId)
        {
            foreach (var roleReference in Roles.Where(r => r.Id == roleId).ToList())
            {
                Roles.Remove(roleReference);
            }
        }
    }
}