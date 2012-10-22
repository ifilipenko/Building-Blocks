using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Web.Security;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership
{
    public class CodeFirstRoleProvider : RoleProvider
    {
        private string _applicationName;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly Lazy<IUserRepository> _userRepository;

        public CodeFirstRoleProvider()
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
                return _applicationName ?? GetType().Assembly.GetName().Name;
            }
            set
            {
                _applicationName = value;
            }
        }

        public override bool RoleExists(string roleName)
        {
            return !string.IsNullOrEmpty(roleName) && RoleRepository.IsRoleExists(roleName);
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(roleName))
                return false;
            
            {
                var user = UserRepository.FindUserByName(username);
                if (user == null)
                    return false;

                var role = RoleRepository.FindByName(roleName);
                if (role == null)
                    return false;

                return user.Roles.Contains(role.RoleName);
            }
        }

        public override string[] GetAllRoles()
        {
            return RoleRepository.GetAll().Select(r => r.RoleName).ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return null;
            var role = RoleRepository.FindByName(roleName);
            return role == null ? null : role.Users.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
                return null;

            var user = UserRepository.FindUserByName(username);
            return user == null ? null : user.Roles.ToArray();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            if (string.IsNullOrEmpty(roleName) || string.IsNullOrEmpty(usernameToMatch))
                return null;

            return (from r in RoleRepository.GetAll()
                    from userName in r.Users
                    where r.RoleName == roleName && userName.Contains(usernameToMatch)
                    select userName).ToArray();
        }

        public override void CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                return;
            var role = RoleRepository.FindByName(roleName);
            if (role != null) 
                return;

            var newRole = new Role
            {
                RoleId = Guid.NewGuid(),
                RoleName = roleName
            };
            RoleRepository.CreateRole(newRole);
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
                return false;
            
            {
                var role = RoleRepository.FindByName(roleName);
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
                        var user = UserRepository.FindUserByName(userName);
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
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            {
                var users = UserRepository.FindUsersByNames(usernames);
                var roles = RoleRepository.FindRolesByNames(roleNames);
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
            var users = UserRepository.FindUsersByNames(usernames).Where(u => u != null);
            var roles = RoleRepository.FindRolesByNames(roleNames).Where(r => r != null).ToList();
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