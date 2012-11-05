using System.Linq;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public static class UserEntityMappingHelpers
    {
        public static User ToUser(this UserEntity entity)
        {
            var user = new User(entity.UserId, entity.Username, entity.Email, entity.ApplicationName)
                {
                    Password = entity.Password,
                    Roles    = entity.Roles.ToList(),
                    Comment = entity.Comment,
                    ConfirmationToken = entity.ConfirmationToken,
                    CreateDate = entity.CreateDate,
                    IsApproved = entity.IsApproved,
                    IsLockedOut = entity.IsLockedOut,
                    LastActivityDate = entity.LastActivityDate,
                    LastLockoutDate = entity.LastLockoutDate,
                    LastLoginDate = entity.LastLoginDate,
                    LastPasswordChangedDate = entity.LastPasswordChangedDate,
                    LastPasswordFailureDate = entity.LastPasswordFailureDate,
                    PasswordFailuresSinceLastSuccess = entity.PasswordFailuresSinceLastSuccess,
                    PasswordVerificationToken = entity.PasswordVerificationToken,
                    PasswordVerificationTokenExpirationDate = entity.PasswordVerificationTokenExpirationDate
                };
            return user;
        }

        public static UserEntity ToEntityWithoutRoles(this User user)
        {
            return new UserEntity
                {
                    UserId = user.UserId,
                    Username = user.Username,
                    ApplicationName = user.ApplicationName,
                    Email = user.Email,
                    Password = user.Password,
                    Comment = user.Comment,
                    ConfirmationToken = user.ConfirmationToken,
                    CreateDate = user.CreateDate,
                    IsApproved = user.IsApproved,
                    IsLockedOut = user.IsLockedOut,
                    LastActivityDate = user.LastActivityDate,
                    LastLockoutDate = user.LastLockoutDate,
                    LastLoginDate = user.LastLoginDate,
                    LastPasswordChangedDate = user.LastPasswordChangedDate,
                    LastPasswordFailureDate = user.LastPasswordFailureDate,
                    PasswordFailuresSinceLastSuccess = user.PasswordFailuresSinceLastSuccess,
                    PasswordVerificationToken = user.PasswordVerificationToken,
                    PasswordVerificationTokenExpirationDate = user.PasswordVerificationTokenExpirationDate
                };
        }
    }
}