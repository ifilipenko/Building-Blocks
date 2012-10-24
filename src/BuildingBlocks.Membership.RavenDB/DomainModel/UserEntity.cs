using System;
using System.Collections.Generic;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public class UserEntity : IEntity<string>
    {
        private IList<RoleReference> _roles;

        public UserEntity()
        {
            _roles = new List<RoleReference>();
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

        public IEnumerable<RoleReference> Roles
        {
            get { return _roles; }
            set { _roles = (IList<RoleReference>)value; }
        }
    }
}