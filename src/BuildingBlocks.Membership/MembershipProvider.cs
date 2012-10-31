using System;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Security;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;
using Common.Logging;

namespace BuildingBlocks.Membership
{
    public class MembershipProvider : System.Web.Security.MembershipProvider, IForcedPasswordChangeProvider
    {
        private static readonly ILog _log = LogManager.GetLogger<MembershipProvider>();
        private string _applicationName;
        private const int TokenSizeInBytes = 16;
        private readonly Lazy<IUserRepository> _userRepository;
        private int? _minRequiredPasswordLength;
        private int? _minRequiredNonAlphanumericCharacters;
        private int? _maxInvalidPasswordAttempts;
        private int? _passwordAttemptWindow;

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
                return _applicationName ?? (_applicationName = GetType().Namespace);
            }
            set 
            {
                _applicationName = value;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get { return _maxInvalidPasswordAttempts ?? 5; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return _minRequiredNonAlphanumericCharacters ?? 0; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return _minRequiredPasswordLength ?? 6; }
        }

        public override int PasswordAttemptWindow
        {
            get { return _passwordAttemptWindow ?? 0; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return MembershipPasswordFormat.Hashed; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return string.Empty; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _log.Trace(m => m("MembershipProvider initalization started"));

            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
            {
                name = "BuildingBlocks.MembershipProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Simple membership provider");
            }

            _minRequiredPasswordLength            = config.GetOf<int?>("minRequiredPasswordLength", validator: ShouldBePositive);
            _minRequiredNonAlphanumericCharacters = config.GetOf<int?>("minRequiredNonalphanumericCharacters", validator: ShouldBePositive);
            _maxInvalidPasswordAttempts           = config.GetOf<int?>("maxInvalidPasswordAttempts", validator: ShouldBePositive);
            _passwordAttemptWindow                = config.GetOf<int?>("passwordAttemptWindow", validator: ShouldBePositive);

            base.Initialize(name, config);

            ApplicationName = !string.IsNullOrEmpty(config["applicationName"]) 
                ? config["applicationName"]
                : ProviderHelpers.GetDefaultAppName();

            _log.Trace(m => m("MembershipProvider with name {0} sucessfully initalized", name));
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {
            _log.Trace(m => m("User creation started"));

            status = MembershipCreateStatus.Success;
            if (string.IsNullOrEmpty(username))
            {
                status = MembershipCreateStatus.InvalidUserName;
            }
            if (string.IsNullOrEmpty(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
            }
            if (string.IsNullOrEmpty(email))
            {
                status = MembershipCreateStatus.InvalidEmail;
            }

            var hashedPassword = Crypto.HashPassword(password);
            if (hashedPassword.Length > 128)
            {
                status = MembershipCreateStatus.InvalidPassword;
            }

            var userRepository = RepositoryFactory.Current.CreateUserRepository();
            if (userRepository.HasUserWithName(ApplicationName, username))
            {
                status = MembershipCreateStatus.DuplicateUserName;
            }

            if (userRepository.HasUserWithEmail(ApplicationName, email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
            }

            if (status != MembershipCreateStatus.Success)
            {
                _log.LogUserCreateStatus(username, status);
                return null;
            }

            var newUser = new User(Guid.NewGuid(), username, email, ApplicationName)
            {
                Password = hashedPassword,
                IsApproved = isApproved,
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

            _log.Trace(m => m("User successfully created"));
            return new MembershipUser(
                System.Web.Security.Membership.Provider.Name,
                newUser.Username,
                newUser.UserId,
                newUser.Email,
                null,
                newUser.Comment,
                newUser.IsApproved,
                newUser.IsLockedOut,
                newUser.CreateDate.Value,
                newUser.LastLoginDate.Value,
                newUser.LastActivityDate.Value,
                newUser.LastPasswordChangedDate.Value,
                newUser.LastLockoutDate.Value
            );
        }

        public override bool ValidateUser(string username, string password)
        {
            _log.Trace(m => m("User with name \"{0}\" validation started", username));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            var user = FindUserByName(username);
            _log.LogUserStateForValidationOperation(username, user);
            if (user == null || !user.IsApproved || user.IsLockedOut)
                return false;

            var hashedPassword = user.Password;
            var verificationSucceeded = hashedPassword != null && Crypto.VerifyHashedPassword(hashedPassword, password);
            if (verificationSucceeded)
            {
                _log.Debug(m => m("Password verification succeeded for user with name \"{0}\"", username));
                user.PasswordFailuresSinceLastSuccess = 0;
                user.LastLoginDate = DateTime.UtcNow;
                user.LastActivityDate = DateTime.UtcNow;
            }
            else
            {
                _log.Debug(m => m("Password verification FAILED! for user with name \"{0}\"", username));
                var failures = user.PasswordFailuresSinceLastSuccess;
                if (failures < MaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    _log.PasswordFailuresIncreased(user);
                }
                else if (failures >= MaxInvalidPasswordAttempts)
                {
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    user.LastLockoutDate = DateTime.UtcNow;
                    user.IsLockedOut = true;
                    _log.UserLockedBecousePasswordVerificationAttemptsIsOver(user);
                }
            }

            UserRepository.SaveUser(user);
            _log.Trace(m => m("User with name \"{0}\" validation successfully completed with result {1}", username, verificationSucceeded));
            return verificationSucceeded;
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            _log.Trace(m => m("Find user by name \"{0}\"", username));
            if (string.IsNullOrEmpty(username))
            {
                _log.Debug(m => m("Given username is empty", username));
                return null;
            }
            
            var user = FindUserByName(username);
            if (user == null)
            {
                _log.UserNotFoundByName(username);
                return null;
            }

            if (userIsOnline)
            {
                SaveLastUserActivity(user);
            }

            _log.Trace(m => m("User with name \"{0}\" successfully founded", username));
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
            _log.Trace(m => m("Find user by provider key \"{0}\"", providerUserKey));
            if (!(providerUserKey is Guid))
            {
                _log.Debug(m => m("Given provider key \"{0}\" is not Guid", providerUserKey));
                return null;
            }

            var user = UserRepository.FindUserById((Guid) providerUserKey);
            if (user == null)
            {
                _log.UserNotFoundByKey(providerUserKey);
                return null;
            }
            _log.Debug(m => m("User by provider key \"{0}\" founded, username is \"{1}\"", providerUserKey, user.Username));

            if (userIsOnline)
            {
                SaveLastUserActivity(user);
            }

            _log.Trace(m => m("User with key \"{0}\" successfully founded", providerUserKey));
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

        bool IForcedPasswordChangeProvider.ChangePassword(string username, string newPassword)
        {
            _log.Trace(m => m("Started password changing for user \"{0}\"", username));
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(newPassword))
            {
                _log.Debug(m => m("Invalid parameters"));
                return false;
            }
            var user = FindUserByName(username);
            if (user == null)
            {
                _log.Debug(m => m("User is not found"));
                return false;
            }

            var newHashedPassword = Crypto.HashPassword(newPassword);
            if (newHashedPassword.Length > 128)
            {
                _log.Debug(m => m("New password hash has invalid lenght"));
                return false;
            }

            user.Password = newHashedPassword;
            user.LastPasswordChangedDate = DateTime.UtcNow;
            UserRepository.SaveUser(user);

            _log.Trace(m => m("Password sucessfully changed for user \"{0}\"", username));
            return true;
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            _log.Trace(m => m("Started password changing for user \"{0}\"", username));
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
                _log.Debug(m => m("Old password verification succeeded for user with name \"{0}\"", username));
                user.PasswordFailuresSinceLastSuccess = 0;
            }
            else
            {
                _log.Debug(m => m("Old verification FAILED! for user with name \"{0}\"", username));
                var failures = user.PasswordFailuresSinceLastSuccess;
                if (failures < MaxInvalidPasswordAttempts)
                {
                    user.PasswordFailuresSinceLastSuccess += 1;
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    _log.PasswordFailuresIncreased(user);
                }
                else if (failures >= MaxInvalidPasswordAttempts)
                {
                    user.LastPasswordFailureDate = DateTime.UtcNow;
                    user.LastLockoutDate = DateTime.UtcNow;
                    user.IsLockedOut = true;
                    _log.UserLockedBecousePasswordVerificationAttemptsIsOver(user);
                }
                UserRepository.SaveUser(user);
                return false;
            }

            var newHashedPassword = Crypto.HashPassword(newPassword);
            if (newHashedPassword.Length > 128)
            {
                _log.Debug(m => m("New password hash has invalid lenght"));
                return false;
            }

            user.Password = newHashedPassword;
            user.LastPasswordChangedDate = DateTime.UtcNow;
            UserRepository.SaveUser(user);

            _log.Trace(m => m("Password sucessfully changed for user \"{0}\"", username));
            return true;
        }

        public override bool UnlockUser(string userName)
        {
            _log.Trace(m => m("Start unlock user with name \"{0}\"", userName));
            var user = FindUserByName(userName);
            if (user == null)
            {
                _log.UserNotFoundByName(userName);
                return false;
            }

            user.IsLockedOut = false;
            user.PasswordFailuresSinceLastSuccess = 0;
            UserRepository.SaveUser(user);

            _log.Trace(m => m("Password sucessfully changed for user \"{0}\"", userName));
            return true;
        }

        public override int GetNumberOfUsersOnline()
        {
            _log.Trace(m => m("Start searching for online users"));

            try
            {
                var userIsOnlineTimeWindow = (double) System.Web.Security.Membership.UserIsOnlineTimeWindow;
                var dateActive = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(userIsOnlineTimeWindow));
                _log.Debug(m => m("All users with last activity more then\"{0}\" is online", dateActive));

                return UserRepository.GetUsersCountWithLastActivityDateGreaterThen(ApplicationName, dateActive);
            }
            finally
            {
                _log.Trace(m => m("Online users successfully founded!"));
            }
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            _log.Trace(m => m("Start deleting user with name \"{0}\"", username));
            if (string.IsNullOrEmpty(username))
            {
                _log.Debug(m => m("User name is empty"));
                return false;
            }

            var user = FindUserByName(username);
            if (user == null)
            {
                _log.UserNotFoundByName(username);
                return false;
            }

            UserRepository.DeleteUser(user);

            _log.Trace(m => m("User with name \"{0}\" successfully deleted", username));
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="MembershipUpdateUserException">The user to update could not be found.</exception>
        public override void UpdateUser(MembershipUser user)
        {
            _log.Trace(m => m("Start updating user with name \"{0}\"", user.UserName));
            var existUser = UserRepository.FindUserById((Guid) user.ProviderUserKey);
            if (existUser == null)
            {
                _log.UserNotFoundByKey((Guid) user.ProviderUserKey);
                throw new MembershipUpdateUserException("The user to update could not be found.");
            }

            existUser.Username = user.UserName;
            existUser.Email = user.Email;
            existUser.Comment = user.Comment;
            existUser.CreateDate = user.CreationDate;
            existUser.IsApproved = user.IsApproved;
            existUser.IsLockedOut = user.IsLockedOut;
            existUser.LastActivityDate = user.LastActivityDate;
            existUser.LastLockoutDate = user.LastLockoutDate;
            existUser.LastLoginDate = user.LastLoginDate;
            existUser.LastPasswordChangedDate = user.LastPasswordChangedDate;
            UserRepository.SaveUser(existUser);

            _log.Trace(m => m("User with name \"{0}\" sucessfully updated", user.UserName));
        }

        public override string GetUserNameByEmail(string email)
        {
            _log.Trace(m => m("Start getting username by email \"{0}\"", email));

            var user = UserRepository.FindUserByEmail(ApplicationName, email);
            if (user == null)
            {
                _log.Debug(m => m("User not found by email \"{0}\"", email));
                return string.Empty;
            }

            _log.Trace(m => m("User with email \"{0}\" successfully found, username is \"{1}\"", email, user.Username));
            return user.Username;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            _log.Trace(m => m("Finding users by email part \"{0}\"", emailToMatch));

            var page = UserRepository.GetUsersPageByEmail(emailToMatch, ApplicationName, pageIndex, pageSize);
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

            _log.FoundedUsersCont(totalRecords);
            _log.Trace(m => m("Users with email part \"{0}\" successfully loaded", emailToMatch));
            return membershipUsers;
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            _log.Trace(m => m("Finding users by username part \"{0}\"", usernameToMatch));

            var page = UserRepository.GetUsersPageByUsername(usernameToMatch, ApplicationName, pageIndex, pageSize);
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

            _log.FoundedUsersCont(totalRecords);
            _log.Trace(m => m("Users with username part \"{0}\" successfully loaded", usernameToMatch));
            return membershipUsers;
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            _log.Trace(m => m("Loading users"));

            var page = UserRepository.GetUsersPage(ApplicationName, pageIndex, pageSize);
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

            _log.FoundedUsersCont(totalRecords);
            _log.Trace(m => m("Users successfully loaded"));
            return membershipUsers;
        }

        public override bool EnablePasswordRetrieval
        {
            get { return false; }
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotSupportedException("Can not retrieve password");
        }

        public override bool EnablePasswordReset
        {
            get { return false; }
        }

        public override string ResetPassword(string username, string answer)
        {
            throw new NotSupportedException("Can not reset password.");
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return false; }
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotSupportedException("Can not change password querstion and password.");
        }

        private User FindUserByName(string username)
        {
            var users = UserRepository.FindUsersByNames(ApplicationName, username);
            return users == null ? null : users.SingleOrDefault();
        }

        private string ShouldBePositive(int? value)
        {
            if (value.HasValue && value.Value < 0)
            {
                return "value should be greater then -1";
            }
            return null;
        }

        private void SaveLastUserActivity(User user)
        {
            _log.Debug(m => m("Mark that user with name \"{0}\" is online", user.Username));
            user.LastActivityDate = DateTime.UtcNow;
            UserRepository.SaveUser(user);
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
    }
}