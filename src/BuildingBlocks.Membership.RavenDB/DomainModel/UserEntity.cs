using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class UserEntity : IEntity<string>
    {
        private IList<string> _roles;

        public UserEntity()
        {
            Roles = new List<string>();
        }

        public string Id { get; set; }
        public Guid UserId { get; set; }

        public string Username { get; set; }
        public string ApplicationName { get; set; }
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

        public IEnumerable<string> Roles
        {
            get { return _roles; }
            set { _roles = (IList<string>) value ?? new List<string>(0); }
        }

        public void UpdateUser(User user)
        {
            Username = user.Username;
            Email = user.Email;
            Password = user.Password;
            ApplicationName = user.ApplicationName;
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

        public IEnumerable<string> GetRolesToRemove(IEnumerable<string> newRoles)
        {
            return Roles
                .Except(newRoles)
                .ToList();
        }

        public bool AddRole(string rolename)
        {
            if (!_roles.Contains(rolename))
            {
                _roles.Add(rolename);
                return true;
            }
             return false;
        }

        public bool RemoveRole(string roleName)
        {
            return _roles.Remove(roleName);
        }

        public override string ToString()
        {
            return string.Format("Username: {0}", Username);
        }
    }
}