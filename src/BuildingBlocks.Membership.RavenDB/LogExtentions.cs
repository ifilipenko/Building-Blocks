using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using Common.Logging;

namespace BuildingBlocks.Membership.RavenDB
{
    public static class LogExtentions
    {
        public static void UpdatedUserData(this ILog log, User to, UserEntity @from)
        {
            if (!log.IsDebugEnabled)
                return;

            LogFieldChanged(log, "Username", to.Username, @from.Username);
            LogFieldChanged(log, "Email", @from.Email, to.Email);
            LogFieldChanged(log, "Password", @from.Password, to.Password);
            LogFieldChanged(log, "ApplicationName", @from.ApplicationName, to.ApplicationName);
            LogFieldChanged(log, "Comment", @from.Comment, to.Comment);
            LogFieldChanged(log, "ConfirmationToken", @from.ConfirmationToken, to.ConfirmationToken);
            LogFieldChanged(log, "CreateDate", @from.CreateDate, to.CreateDate);
            LogFieldChanged(log, "IsApproved", @from.IsApproved, to.IsApproved);
            LogFieldChanged(log, "IsLockedOut", @from.IsLockedOut, to.IsLockedOut);
            LogFieldChanged(log, "LastActivityDate", @from.LastActivityDate, to.LastActivityDate);
            LogFieldChanged(log, "LastLockoutDate", @from.LastLockoutDate, to.LastLockoutDate);
            LogFieldChanged(log, "LastLoginDate", @from.LastLoginDate, to.LastLoginDate);
            LogFieldChanged(log, "LastPasswordChangedDate", @from.LastPasswordChangedDate, to.LastPasswordChangedDate);
            LogFieldChanged(log, "LastPasswordFailureDate", @from.LastPasswordFailureDate, to.LastPasswordFailureDate);
            LogFieldChanged(log, "PasswordFailuresSinceLastSuccess", @from.PasswordFailuresSinceLastSuccess, to.PasswordFailuresSinceLastSuccess);
            LogFieldChanged(log, "PasswordVerificationToken", @from.PasswordVerificationToken, to.PasswordVerificationToken);
            LogFieldChanged(log, "PasswordVerificationTokenExpirationDate", @from.PasswordVerificationTokenExpirationDate, to.PasswordVerificationTokenExpirationDate);
        }

        public static void FoundedRolesByParameters(this ILog log, IEnumerable<RoleEntity> founded, IEnumerable<string> given)
        {
            founded = founded ?? Enumerable.Empty<RoleEntity>();
            log.Debug(m => m("For user founded roles array [{0}] by given roles array [{1}]", 
                founded.Select(r => r.RoleName).JoinToString(), 
                (given ?? Enumerable.Empty<string>()).JoinToString()));
        }

        private static void LogFieldChanged(ILog log, string property, object from, object to)
        {
            if (!Equals(from, to))
            {
                log.DebugFormat("user property \"{0}\" changed from {1} to {2}", property, from, to);
            }
        }
    }
}