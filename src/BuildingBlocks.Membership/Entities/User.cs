using System;
using System.Collections.Generic;

namespace BuildingBlocks.Membership.Entities
{
    public class User
    {
        private IList<Role> _roles;

        public User()
        {
            _roles = new List<Role>();
        }

        public virtual Guid UserId { get; set; }

        public virtual string Username { get; set; }
        public virtual string Email { get; set; }

        public virtual string Password { get; set; }

        public virtual string FirstName { get; set; }
        public virtual string LastName { get; set; }

        public virtual string Comment { get; set; }

        public virtual bool IsApproved { get; set; }
        public virtual int PasswordFailuresSinceLastSuccess { get; set; }
        public virtual DateTime? LastPasswordFailureDate { get; set; }
        public virtual DateTime? LastActivityDate { get; set; }
        public virtual DateTime? LastLockoutDate { get; set; }
        public virtual DateTime? LastLoginDate { get; set; }
        public virtual string ConfirmationToken { get; set; }
        public virtual DateTime? CreateDate { get; set; }
        public virtual bool IsLockedOut { get; set; }
        public virtual DateTime? LastPasswordChangedDate { get; set; }
        public virtual string PasswordVerificationToken { get; set; }
        public virtual DateTime? PasswordVerificationTokenExpirationDate { get; set; }

        public virtual IEnumerable<Role> Roles
        {
            get { return _roles; }
            set { _roles = (IList<Role>)value; }
        }
    }
}