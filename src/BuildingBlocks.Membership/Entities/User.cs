using System;
using System.Collections.Generic;

namespace BuildingBlocks.Membership.Entities
{
    public class User
    {
        private IList<string> _roles;

        public User(Guid userId, string userName, string applicationName, string email)
        {
            UserId = userId;
            Username = userName;
            ApplicationName = applicationName;
            Email = email;
            _roles = new List<string>();
        }

        public Guid UserId { get; private set; }

        public string ApplicationName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

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

        public virtual IEnumerable<string> Roles
        {
            get { return _roles; }
            set { _roles = (IList<string>) value; }
        }

        public void RemoveRole(string roleName)
        {
            _roles.Remove(roleName);
        }

        public void AddRole(string roleName)
        {
            if (_roles.Contains(roleName)) 
                return;

            _roles.Add(roleName);
        }
    }
}