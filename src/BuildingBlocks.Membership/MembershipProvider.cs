using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership
{
    public class MembershipProvider : System.Web.Security.MembershipProvider
    {
        private string _applicationName;
        private const int TokenSizeInBytes = 16;
        private readonly Lazy<IUserRepository> _userRepository;

        public MembershipProvider()
        {
            _userRepository = new Lazy<IUserRepository>(() => RepositoryFactory.Current.CreateUserRepository(), true);
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository.Value; }
        }

        public override string ApplicationName
        {
            get
            {
                return _applicationName ?? GetType().Assembly.GetName().Name;
            }
            set 
            {
                _applicationName = value;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return 5; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { return 0; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return String.Empty; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.Success;

            if (string.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }
            if (string.IsNullOrEmpty(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }
            if (string.IsNullOrEmpty(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            var hashedPassword = Crypto.HashPassword(password);
            if (hashedPassword.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            var userRepository = RepositoryFactory.Current.CreateUserRepository();
            if (userRepository.HasUserWithName(username))
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (userRepository.HasUserWithEmail(email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = username,
                Password = hashedPassword,
                IsApproved = isApproved,
                Email = email,
                CreateDate = DateTime.UtcNow,
                LastPasswordChangedDate = DateTime.UtcNow,
                PasswordFailuresSinceLastSuccess = 0,
                LastLoginDate = DateTime.UtcNow,
                LastActivityDate = DateTime.UtcNow,
                LastLockoutDate = DateTime.UtcNow,
                IsLockedOut = false,
                LastPasswordFailureDate = DateTime.UtcNow
            };

            userRepository.AddUser(newUser);
            return new MembershipUser(
                System.Web.Security.Membership.Provider.Name,
                newUser.Username,
                newUser.UserId,
                newUser.Email,
                null,
                null,
                newUser.IsApproved,
                newUser.IsLockedOut,
                newUser.CreateDate.Value,
                newUser.LastLoginDate.Value,
                newUser.LastActivityDate.Value,
                newUser.LastPasswordChangedDate.Value,
                newUser.LastLockoutDate.Value
            );
        }

        public string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values)
        {
            return CreateAccount(userName, password, requireConfirmation);
        }

        public override bool ValidateUser(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var user = FindUserByName(username);
            if (user == null || !user.IsApproved || user.IsLockedOut)
                return false;

            var hashedPassword = user.Password;
            var verificationSucceeded = hashedPassword != null && Crypto.VerifyHashedPassword(hashedPassword, password);
            if (verificationSucceeded)
            {
                user.PasswordFailuresSinceLastSuccess = 0;
                user.LastLoginDate = DateTime.UtcNow;
                user.LastActivityDate = DateTime.UtcNow;
            }
            else
            {
                var failures = user.PasswordFailuresSinceLastSuccess;
                if (failures < MaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                }
                else if (failures >= MaxInvalidPasswordAttempts)
                {
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    user.LastLockoutDate = DateTime.UtcNow;
                    user.IsLockedOut = true;
                }
            }

            UserRepository.SaveUser(user);
            return verificationSucceeded;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var user = FindUserByName(username);
            if (user == null)
                return null;

            if (userIsOnline)
            {
                user.LastActivityDate = DateTime.UtcNow;
                UserRepository.SaveUser(user);
            }

            return new MembershipUser(
                System.Web.Security.Membership.Provider.Name,
                user.Username,
                user.UserId,
                user.Email,
                null,
                null,
                user.IsApproved,
                user.IsLockedOut,
                user.CreateDate.Value,
                user.LastLoginDate.Value,
                user.LastActivityDate.Value,
                user.LastPasswordChangedDate.Value,
                user.LastLockoutDate.Value
            );
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (!(providerUserKey is Guid))
                return null;

            var user = UserRepository.FindUserById((Guid) providerUserKey);
            if (user == null)
                return null;

            if (userIsOnline)
            {
                user.LastActivityDate = DateTime.UtcNow;
                UserRepository.SaveUser(user);
            }

            return new MembershipUser(
                System.Web.Security.Membership.Provider.Name,
                user.Username,
                user.UserId,
                user.Email,
                null,
                null,
                user.IsApproved,
                user.IsLockedOut,
                user.CreateDate.Value,
                user.LastLoginDate.Value,
                user.LastActivityDate.Value,
                user.LastPasswordChangedDate.Value,
                user.LastLockoutDate.Value
            );
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(oldPassword) || string.IsNullOrEmpty(newPassword))
                return false;
            var user = FindUserByName(username);
            if (user == null)
                return false;

            var hashedPassword = user.Password;
            var verificationSucceeded = (hashedPassword != null &&
                                         Crypto.VerifyHashedPassword(hashedPassword, oldPassword));
            if (verificationSucceeded)
            {
                user.PasswordFailuresSinceLastSuccess = 0;
            }
            else
            {
                var failures = user.PasswordFailuresSinceLastSuccess;
                if (failures < MaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                }
                else if (failures >= MaxInvalidPasswordAttempts)
                {
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    user.LastLockoutDate = DateTime.UtcNow;
                    user.IsLockedOut = true;
                }
                UserRepository.SaveUser(user);
                return false;
            }

            var newHashedPassword = Crypto.HashPassword(newPassword);
            if (newHashedPassword.Length > 128)
                return false;

            user.Password = newHashedPassword;
            user.LastPasswordChangedDate = DateTime.UtcNow;
            UserRepository.SaveUser(user);

            return true;
        }

        public override bool UnlockUser(string userName)
        {
            var user = FindUserByName(userName);
            if (user == null)
                return false;

            user.IsLockedOut = false;
            user.PasswordFailuresSinceLastSuccess = 0;
            UserRepository.SaveUser(user);

            return true;
        }

        public override int GetNumberOfUsersOnline()
        {
            var userIsOnlineTimeWindow = (double) System.Web.Security.Membership.UserIsOnlineTimeWindow;
            var dateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(userIsOnlineTimeWindow));
            return UserRepository.GetUsersCountWithLastActivityDateGreaterThen(dateActive);
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (string.IsNullOrEmpty(username))
                return false;

            var user = FindUserByName(username);
            if (user == null)
                return false;

            UserRepository.DeleteUser(user);
            return true;
        }

        public override string GetUserNameByEmail(string email)
        {
            var user = UserRepository.FindUserByEmail(email);
            return user == null ? string.Empty : user.Username;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var page = UserRepository.GetUsersPageByEmail(emailToMatch, pageIndex, pageSize);
            totalRecords = (int) page.TotalItemCount;

            var membershipUsers = new MembershipUserCollection();
            foreach (var user in page.Items)
            {
                var membershipUser = new MembershipUser(
                    System.Web.Security.Membership.Provider.Name, 
                    user.Username, 
                    user.UserId, 
                    user.Email, 
                    null, 
                    null, 
                    user.IsApproved, 
                    user.IsLockedOut, 
                    user.CreateDate.Value, 
                    user.LastLoginDate.Value, 
                    user.LastActivityDate.Value, 
                    user.LastPasswordChangedDate.Value, 
                    user.LastLockoutDate.Value
                );
                membershipUsers.Add(membershipUser);
            }
            return membershipUsers;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var page = UserRepository.GetUsersPageByUsername(usernameToMatch, pageIndex, pageSize);
            totalRecords = (int) page.TotalItemCount;

            var membershipUsers = new MembershipUserCollection();
            foreach (var user in page.Items)
            {
                var membershipUser = new MembershipUser(
                    System.Web.Security.Membership.Provider.Name,
                    user.Username,
                    user.UserId,
                    user.Email,
                    null,
                    null,
                    user.IsApproved,
                    user.IsLockedOut,
                    user.CreateDate.Value,
                    user.LastLoginDate.Value,
                    user.LastActivityDate.Value,
                    user.LastPasswordChangedDate.Value,
                    user.LastLockoutDate.Value
                );
                membershipUsers.Add(membershipUser);
            }
            return membershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            var page = UserRepository.GetUsersPage(pageIndex, pageSize);
            totalRecords = (int) page.TotalItemCount;

            var membershipUsers = new MembershipUserCollection();
            foreach (var user in page.Items)
            {
                var membershipUser = new MembershipUser(
                    System.Web.Security.Membership.Provider.Name,
                    user.Username,
                    user.UserId,
                    user.Email,
                    null,
                    null,
                    user.IsApproved,
                    user.IsLockedOut,
                    user.CreateDate.Value,
                    user.LastLoginDate.Value,
                    user.LastActivityDate.Value,
                    user.LastPasswordChangedDate.Value,
                    user.LastLockoutDate.Value
                );
                membershipUsers.Add(membershipUser);
            }
            return membershipUsers;
        }

        public string CreateAccount(string userName, string password, bool requireConfirmationToken)
        {
            if (string.IsNullOrEmpty(userName))
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidUserName);
            if (string.IsNullOrEmpty(password))
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidPassword);

            var hashedPassword = Crypto.HashPassword(password);
            if (hashedPassword.Length > 128)
                throw new MembershipCreateUserException(MembershipCreateStatus.InvalidPassword);
            if (UserRepository.HasUserWithName(userName))
                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);

            var token = string.Empty;
            if (requireConfirmationToken)
            {
                token = GenerateToken();
            }

            var newUser = new User
            {
                UserId = Guid.NewGuid(),
                Username = userName,
                Password = hashedPassword,
                IsApproved = !requireConfirmationToken,
                Email = string.Empty,
                CreateDate = DateTime.UtcNow,
                LastPasswordChangedDate = DateTime.UtcNow,
                PasswordFailuresSinceLastSuccess = 0,
                LastLoginDate = DateTime.UtcNow,
                LastActivityDate = DateTime.UtcNow,
                LastLockoutDate = DateTime.UtcNow,
                IsLockedOut = false,
                LastPasswordFailureDate = DateTime.UtcNow,
                ConfirmationToken = token
            };
            UserRepository.AddUser(newUser);
            return token;
        }

        private static string GenerateToken()
        {
            using (var cryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                return GenerateToken(cryptoServiceProvider);
            }
        }

        private static string GenerateToken(RandomNumberGenerator generator)
        {
            var tokenBytes = new byte[TokenSizeInBytes];
            generator.GetBytes(tokenBytes);
            return HttpServerUtility.UrlTokenEncode(tokenBytes);
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException("Consider using methods from WebSecurity module.");
        }

        public override void UpdateUser(MembershipUser user)
        {
            throw new NotSupportedException();
        }

        private User FindUserByName(string username)
        {
            var users = UserRepository.FindUsersByNames(username);
            return users == null ? null : users.SingleOrDefault();
        }
    }
}