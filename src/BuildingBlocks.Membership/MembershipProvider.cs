using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Hosting;
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

            _minRequiredPasswordLength            = GetSetting<int?>(config, "minRequiredPasswordLength", validator: ShouldBePositive);
            _minRequiredNonAlphanumericCharacters = GetSetting<int?>(config, "minRequiredNonalphanumericCharacters", validator: ShouldBePositive);
            _maxInvalidPasswordAttempts           = GetSetting<int?>(config, "maxInvalidPasswordAttempts", validator: ShouldBePositive);
            _passwordAttemptWindow                = GetSetting<int?>(config, "passwordAttemptWindow", validator: ShouldBePositive);

            base.Initialize(name, config);

            ApplicationName = !string.IsNullOrEmpty(config["applicationName"]) 
                ? config["applicationName"] 
                : GetDefaultAppName();
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
            if (userRepository.HasUserWithName(ApplicationName, username))
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            if (userRepository.HasUserWithEmail(ApplicationName, email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
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
            return UserRepository.GetUsersCountWithLastActivityDateGreaterThen(ApplicationName, dateActive);
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
            var user = UserRepository.FindUserByEmail(ApplicationName, email);
            return user == null ? string.Empty : user.Username;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            var page = UserRepository.GetUsersPageByEmail(ApplicationName, emailToMatch, pageIndex, pageSize);
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
            var page = UserRepository.GetUsersPageByUsername(ApplicationName, usernameToMatch, pageIndex, pageSize);
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
            return membershipUsers;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <exception cref="MembershipUpdateUserException">The user to update could not be found.</exception>
        public override void UpdateUser(MembershipUser user)
        {
            var existUser = UserRepository.FindUserById((Guid) user.ProviderUserKey);
            if (existUser == null)
                throw new MembershipUpdateUserException("The user to update could not be found.");

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
        }

        private User FindUserByName(string username)
        {
            var users = UserRepository.FindUsersByNames(ApplicationName, username);
            return users == null ? null : users.SingleOrDefault();
        }

        private string GetDefaultAppName()
        {
            try
            {
                var virtualPath = HostingEnvironment.ApplicationVirtualPath;

                if (string.IsNullOrEmpty(virtualPath))
                {
                    virtualPath = Process.GetCurrentProcess().MainModule.ModuleName;
                    var startIndex = virtualPath.IndexOf('.');
                    if (startIndex > -1)
                    {
                        virtualPath = virtualPath.Remove(startIndex);
                    }
                }

                return string.IsNullOrEmpty(virtualPath) ? "/" : virtualPath;
            }
            catch (Exception)
            {
                return "/";
            }
        }

        private static T GetSetting<T>(NameValueCollection config, string sectionName, T defaultValue = default(T), Func<T, string> validator = null)
        {
            string settingValue;
            try
            {
                settingValue = config[sectionName];
            }
            catch (Exception)
            {
                return defaultValue;
            }

            T value;
            try
            {
                value = (T) Convert.ChangeType(settingValue, typeof (T));
            }
            catch (Exception)
            {
                throw new InvalidCastException(string.Format("Value \"{0}\" of membership attribute \"{1}\" can be converted to \"{2}\"", value, sectionName, typeof(T)));
            }

            if (validator != null)
            {
                var error = validator(value);
                if (!string.IsNullOrEmpty(error))
                {
                    throw new ConfigurationErrorsException(string.Format("Value \"{0}\" of membership attribute \"{1}\" is invalid: \"{2}\"", value, sectionName, error));
                }
            }

            return value;
        }

        private string ShouldBePositive(int? value)
        {
            if (value.HasValue && value.Value > -1)
            {
                return "value should be greater then -1";
            }
            return null;
        }
    }
}