using System.Web.Security;
using BuildingBlocks.Membership.Entities;
using Common.Logging;

namespace BuildingBlocks.Membership
{
    internal static class LogExtentions
    {
        public static void UserLockedBecousePasswordVerificationAttemptsIsOver(this ILog log, User user)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Password failures attemps is over, user with name \"{0}\" is locked", user.Username);
            }
        }

        public static void PasswordFailuresIncreased(this ILog log, User user)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Password failures attemps for user \"{0}\" was set to {1}", user.Username, user.PasswordFailuresSinceLastSuccess);
            }
        }

        public static void UserNotFoundByKey(this ILog log, object providerUserKey)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("User not found by key \"{0}\"", providerUserKey);
            }
        }

        public static void UserNotFoundByName(this ILog log, string username)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("User not found by name \"{0}\"", username);
            }
        }

        public static void RoleNotFoundByName(this ILog log, string roleName)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Role not found by name \"{0}\"", roleName);
            }
        }

        public static void FoundedUsersCont(this ILog log, int count)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("Founded {0} users", count);
            }
        }

        public static void LogUserCreateStatus(this ILog log, string username, MembershipCreateStatus status)
        {
            if (log.IsDebugEnabled)
            {
                log.DebugFormat("User with name \"{0}\" creation failed with status [{1}]", username, status);
            }
        }

        public static void LogUserStateForValidationOperation(this ILog log, string username, User user)
        {
            if (!log.IsDebugEnabled) 
                return;

            if (user == null)
            {
                log.DebugFormat("User with name \"{0}\" is not exists", username);
            }
            else
            {
                log.DebugFormat("User with name \"{0}\" sucessfully founded", username);
                if (!user.IsApproved)
                {
                    log.DebugFormat("User with name \"{0}\" is not approved", username);
                }
                if (user.IsLockedOut)
                {
                    log.DebugFormat("User with name \"{0}\" is locked", username);
                }
            }
        }
    }
}