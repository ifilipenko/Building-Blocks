using System;
using System.Collections.Specialized;
using System.Linq;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership
{
    public class RoleProvider : System.Web.Security.RoleProvider
    {
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
        }

        public override bool RoleExists(string roleName)
        {
            return !string.IsNullOrEmpty(roleName) && RoleRepository.IsRoleExists(ApplicationName, roleName);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleName))
                return false;
            
            {
                var user = UserRepository.FindUsersByNames(ApplicationName, username).SingleOrDefault();
                if (user == null)
                    return false;

                var role = RoleRepository.FindRolesByNames(ApplicationName, roleName).SingleOrDefault();
                return role != null && user.Roles.Contains(role.RoleName);
            }
        }

        public override string[] GetAllRoles()
        {
            return RoleRepository.GetAll(ApplicationName).Select(r => r.RoleName).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return null;
            var role = RoleRepository.FindRolesByNames(ApplicationName, roleName).SingleOrDefault();
            return role == null ? null : role.Users.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var user = UserRepository.FindUsersByNames(ApplicationName, username).SingleOrDefault();
            return user == null ? null : user.Roles.ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(usernameToMatch))
                return null;

            return (from r in RoleRepository.GetAll(ApplicationName)
                    from userName in r.Users
                    where r.RoleName == roleName && userName.Contains(usernameToMatch)
                    select userName).ToArray();
        }

        public override void CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return;
            var role = RoleRepository.FindRolesByNames(ApplicationName, roleName).SingleOrDefault();
            if (role != null) 
                return;

            var newRole = new Role(Guid.NewGuid(), roleName, ApplicationName);
            RoleRepository.CreateRole(newRole);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
                return false;

            var role = RoleRepository.FindRolesByNames(ApplicationName, roleName).SingleOrDefault();
            if (role == null)
                return false;

            if (throwOnPopulatedRole)
            {
                if (role.Users.Any())
                    return false;
            }
            else
            {
                foreach (var userName in role.Users)
                {
                    var user = UserRepository.FindUsersByNames(ApplicationName, userName).SingleOrDefault();
                    if (user != null)
                    {
                        user.RemoveRole(role.RoleName);
                        UserRepository.SaveUser(user);
                    }
                }
            }
            RoleRepository.DeleteRole(role);
            return true;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            {
                var users = UserRepository.FindUsersByNames(ApplicationName, usernames);
                var roles = RoleRepository.FindRolesByNames(ApplicationName, roleNames);
                foreach (var user in users)
                {
                    foreach (var role in roles)
                    {
                        user.AddRole(role.RoleName);
                        UserRepository.SaveUser(user);  
                    }
                }
            }
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            var users = UserRepository.FindUsersByNames(ApplicationName, usernames).Where(u => u != null);
            var roles = RoleRepository.FindRolesByNames(ApplicationName, roleNames).Where(r => r != null).ToList();
            foreach (var user in users)
            {
                foreach (var role in roles)
                {
                    user.RemoveRole(role.RoleName);
                    UserRepository.SaveUser(user);
                }
            }
        }
    }
}