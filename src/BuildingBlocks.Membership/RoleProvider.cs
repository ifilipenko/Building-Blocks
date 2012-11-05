using System;
using System.Collections.Specialized;
using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;
using Common.Logging;

namespace BuildingBlocks.Membership
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
        private static readonly ILog _log = LogManager.GetLogger<RoleProvider>();
        private string _applicationName;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly Lazy<IUserRepository> _userRepository;

        public RoleProvider()
        {
            _roleRepository = new Lazy<IRoleRepository>(() => RepositoryFactory.Current.CreateRoleRepository(), true);
            _userRepository = new Lazy<IUserRepository>(() => RepositoryFactory.Current.CreateUserRepository(), true);
        }

        public IRoleRepository RoleRepository
        {
            get { return _roleRepository.Value; }
        }

        public IUserRepository UserRepository
        {
            get { return _userRepository.Value; }
        }

        public override string ApplicationName
        {
            get
            {
                return _applicationName ?? GetType().Namespace;
            }
            set
            {
                _applicationName = value;
            }
        }

        public override void Initialize(string name, NameValueCollection config)
        {
            _log.Trace(m => m("Initialization started"));

            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrEmpty(name))
            {
                name = "BuildingBlocks.RoleProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Simple role provider");
            }

            base.Initialize(name, config);

            ApplicationName = !string.IsNullOrEmpty(config["applicationName"])
                ? config["applicationName"]
                : ProviderHelpers.GetDefaultAppName();

            _log.Trace(m => m("Initialization succedded with provider name \"{0}\"", name));
        }

        public override bool RoleExists(string roleName)
        {
            _log.Trace(m => m("Checking role existsing by name \"{0}\"", roleName));

            if (string.IsNullOrWhiteSpace(roleName))
            {
                _log.Debug(m => m("Log name is null or empty"));
                return false;
            }
            var isRoleExists = RoleRepository.IsRoleExists(ApplicationName, roleName);

            _log.Trace(m => m("Role \"{0}\" exists checking succedded", roleName));
            return isRoleExists;
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            _log.Trace(m => m("Checking that user \"{0}\" contains in role \"{1}\"", username, roleName));

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleName))
            {
                _log.Debug(m =>
                    {
                        if (string.IsNullOrEmpty(username))
                        {
                            m("Username is empty");
                        }
                        if (string.IsNullOrEmpty(roleName))
                        {
                            m("Rolename is empty");
                        }
                    });
                return false;
            }

            var user = UserRepository.FindUsersByNames(ApplicationName, username).SingleOrDefault();
            if (user == null)
            {
                _log.UserNotFoundByName(username);
                return false;
            }

            var role = RoleRepository.FindRolesByNames(ApplicationName, roleName).SingleOrDefault();
            if (role == null)
            {
                _log.RoleNotFoundByName(roleName);
                return false;
            }

            var isUserInRole = user.Roles.Contains(role);
            _log.Trace(m => m("Successfully checked that user \"{0}\" contains in role \"{1}\"", username, roleName));
            return isUserInRole;
        }

        public override string[] GetAllRoles()
        {
            _log.Trace(m => m("Load all roles"));
            var allRoles = RoleRepository.GetAll(ApplicationName).ToArray();
            _log.Trace(m => m("All roles successfully loaded"));
            return allRoles;
        }

        public override string[] GetUsersInRole(string roleName)
        {
            _log.Trace(m => m("Finding users contained in role \"{0}\"", roleName));
            if (string.IsNullOrEmpty(roleName))
            {
                _log.Debug(m => m("Role name is empty"));
                return null;
            }
            var role = RoleRepository.FindRolesByNames(ApplicationName, roleName).SingleOrDefault();

            if (role == null)
            {
                _log.RoleNotFoundByName(roleName);
                return null;
            }

            var usersInRole = UserRepository.FindUsersInRole(ApplicationName, roleName).Select(u => u.Username).ToArray();
            _log.Trace(m => m("Successfully loaded all users contained in role \"{0}\"", roleName));
            return usersInRole;
        }

        public override string[] GetRolesForUser(string username)
        {
            _log.Trace(m => m("Finding roles for user with name \"{0}\"", username));
            if (string.IsNullOrEmpty(username))
                return null;

            var user = UserRepository.FindUsersByNames(ApplicationName, username).SingleOrDefault();
            if (user == null)
            {
                _log.UserNotFoundByName(username);
                return null;
            }

            var rolesForUser = user.Roles.ToArray();
            _log.Trace(m => m("Successfully loaded all roles for user with name \"{0}\"", username));
            return rolesForUser;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            _log.Trace(m => m("Start find users by name part \"{0}\" and conrained in role \"{1}\"", usernameToMatch, roleName));
            if (string.IsNullOrEmpty(usernameToMatch) || string.IsNullOrEmpty(roleName))
            {
                _log.Debug(m =>
                {
                    if (string.IsNullOrEmpty(usernameToMatch))
                    {
                        m("Username part is empty");
                    }
                    if (string.IsNullOrEmpty(roleName))
                    {
                        m("Rolename is empty");
                    }
                });
                return null;
            }

            var usersInRole = UserRepository.FindUsersInRole(ApplicationName, roleName, usernameToMatch).Select(u => u.Username).ToArray();
            _log.Trace(m => m("Successfully finded users contained in role\"{0}\" and with names contained \"{1}\"", roleName, usernameToMatch));
            return usersInRole;
        }

        public override void CreateRole(string roleName)
        {
            _log.Trace(m => m("Start role creation with name \"{0}\"", roleName));

            if (string.IsNullOrEmpty(roleName))
            {
                _log.Debug(m => m("Rolename is empty"));
                return;
            }

            var role = RoleRepository.FindRolesByNames(ApplicationName, roleName).SingleOrDefault();
            if (role != null)
            {
                _log.RoleNotFoundByName(roleName);
                return;
            }

            RoleRepository.CreateRole(Guid.NewGuid(), ApplicationName, roleName);
            _log.Trace(m => m("Role with name \"{0}\" successfully created", roleName));
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            _log.Trace(m => m("Start role with name \"{0}\" deleting", roleName));

            if (string.IsNullOrEmpty(roleName))
            {
                _log.Debug(m => m("Rolename is empty"));
                return false;
            }

            if (!RoleRepository.IsRoleExists(ApplicationName, roleName))
            {
                _log.RoleNotFoundByName(roleName);
                return false;
            }

            var users = UserRepository.FindUsersInRole(ApplicationName, roleName);
            if (throwOnPopulatedRole)
            {
                if (users.Any())
                {
                    _log.Debug(m => m("Role \"{0}\" has {1} users and deprecated to delete", roleName, users.Count()));
                    return false;
                }
            }
            else
            {
                foreach (var user in users)
                {
                    user.RemoveRole(roleName);
                    UserRepository.SaveUser(user);
                    _log.Debug(m => m("User with name \"{0}\" removed from role \"{1}\"", user.Username, roleName));
                }
            }
            RoleRepository.DeleteRole(ApplicationName, roleName);

            _log.Trace(m => m("Role with name \"{0}\" sucessfully deleted", roleName));
            return true;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            _log.Trace(m => m("Start add users {0} to roles \"{1}\"", usernames.JoinToString(), roleNames.JoinToString()));

            var users = UserRepository.FindUsersByNames(ApplicationName, usernames).Where(u => u != null).ToList();
            var roles = RoleRepository.FindRolesByNames(ApplicationName, roleNames).Where(r => r != null).ToList();
            _log.Debug(m => m("Founded [{0}] users by [{1}] names", users.Select(u => u.Username).JoinToString(), usernames.JoinToString()));
            _log.Debug(m => m("Founded [{0}] roles by [{1}] names", roles.JoinToString(), roleNames.JoinToString()));

            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    user.AddRole(role);
                    UserRepository.SaveUser(user);
                }
            }

            _log.Trace(m => m("Adding users [{0}] to roles [{1}] succedded", usernames.JoinToString(), roleNames.JoinToString()));
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            _log.Trace(m => m("Start remove users {0} from roles \"{1}\"", usernames.JoinToString(), roleNames.JoinToString()));

            var users = UserRepository.FindUsersByNames(ApplicationName, usernames).Where(u => u != null).ToList();
            var roles = RoleRepository.FindRolesByNames(ApplicationName, roleNames).Where(r => r != null).ToList();
            _log.Debug(m => m("Founded {0} users by {1} names", usernames.Length, users.Count));
            _log.Debug(m => m("Founded {0} roles by {1} names", roleNames.Length, roles.Count));

            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    user.RemoveRole(role);
                    UserRepository.SaveUser(user);
                }
            }

            _log.Trace(m => m("Removing users {0} from roles \"{1}\" succedded", usernames.JoinToString(), roleNames.JoinToString()));
        }
    }
}