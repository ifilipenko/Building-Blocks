using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Membership.RavenDB.Queries;
using BuildingBlocks.Membership.RavenDB.Queries.Criteria;
using BuildingBlocks.Query;
using BuildingBlocks.Store;
using Common.Logging;

namespace BuildingBlocks.Membership.RavenDB
{
    public class UserRepositoryImpl : RepositoryBase, IUserRepository
    {
        private static readonly ILog _log = LogManager.GetLogger<UserRepositoryImpl>();

        public UserRepositoryImpl(IStorage storage)
            : base(storage)
        {
        }

        public UserRepositoryImpl(IStorageSession outsideSession)
            : base(outsideSession)
        {
        }

        public bool HasUserWithName(string applicationName, string username)
        {
            using (var session = OpenSesion())
            {
                return session.Query<UserEntity>().Any(u => u.Username == username);
            }
        }

        public bool HasUserWithEmail(string applicationName, string email)
        {
            using (var session = OpenSesion())
            {
                return session.Query<UserEntity>().Any(u => u.Email == email);
            }
        }

        public IEnumerable<User> FindUsersByNames(string applicationName, params string[] usernames)
        {
            using (var session = OpenSesion())
            {
                var users = session.Query<UserEntity>(staleResults: StaleResultsMode.WaitForNonStaleResults)
                    .ContainsIn(u => u.Username, usernames)
                    .OrderBy(u => u.Username)
                    .ToList();
                return users.Select(u => u.ToUser()).ToList();
            }
        }

        public User FindUserByEmail(string applicationName, string email)
        {
            using (var session = OpenSesion())
            {
                var user = session.Query<UserEntity>()
                    .FirstOrDefault(u => u.ApplicationName == applicationName && u.Email == email);
                return user == null ? null : user.ToUser();
            }
        }

        public User FindUserById(Guid userId)
        {
            using (var session = OpenSesion())
            {
                var user = session.Query<UserEntity>().FirstOrDefault(u => u.UserId == userId);
                return user == null ? null : user.ToUser();
            }
        }

        public IEnumerable<User> FindUsersInRole(string applicationName, string roleName)
        {
            using (var session = OpenSesion())
            {
                var usernames = from u in session.Query<UserEntity>()
                                where u.ApplicationName == applicationName && u.Roles.Any(r => r == roleName)
                                select u;
                return usernames.AsEnumerable().Select(u => u.ToUser());
            }
        }

        public IEnumerable<User> FindUsersInRole(string applicationName, string roleName, string usernameToMatch)
        {
            using (var session = OpenSesion())
            {
                var usernames = from u in session.Query<UserEntity>()
                                where u.ApplicationName == applicationName && u.Roles.Any(r => r == roleName) &&
                                      u.Username.Contains(usernameToMatch)
                                select u;
                return usernames.AsEnumerable().Select(u => u.ToUser());
            }
        }

        public Page<User> GetUsersPageByEmail(string emailToMatch, string applicationName, int pageIndex, int pageSize)
        {
            using (var session = OpenSesion())
            {
                IQuery<FindByEmailSubstring, Page<User>> query = new UsersColumnMatchedToSubstring(session);
                var page =
                    query.Execute(new FindByEmailSubstring(emailToMatch, applicationName, pageIndex + 1, pageSize));
                return page;
            }
        }

        public Page<User> GetUsersPageByUsername(string usernameToMatch, string applicationName, int pageIndex,
                                                 int pageSize)
        {
            using (var session = OpenSesion())
            {
                IQuery<FindByUsernameSubstring, Page<User>> query = new UsersColumnMatchedToSubstring(session);
                var page =
                    query.Execute(new FindByUsernameSubstring(usernameToMatch, applicationName, pageIndex + 1, pageSize));
                return page;
            }
        }

        public Page<User> GetUsersPage(string applicationName, int pageIndex, int pageSize)
        {
            using (var session = OpenSesion())
            {
                return Pagination.From(session.Query<UserEntity>().OrderBy(u => u.Username))
                    .Page(pageIndex + 1, pageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
            }
        }

        public int GetUsersCountWithLastActivityDateGreaterThen(string applicationName, DateTime dateActive)
        {
            using (var session = OpenSesion())
            {
                return session.Query<UserEntity>().Count(u => u.LastActivityDate > dateActive);
            }
        }

        public void AddUser(User newUser)
        {
            _log.Trace(m => m("Adding user started"));
            using (var session = OpenSesion())
            {
                session.UseOptimisticConcurrency();

                var userEntity = newUser.ToEntityWithoutRoles();
                var roles = newUser.HasRoles
                                ? session.Query<RoleEntity>().ContainsIn(r => r.RoleName, newUser.Roles).ToList()
                                : Enumerable.Empty<RoleEntity>();
                _log.FoundedRolesByParameters(roles, newUser.Roles);

                foreach (var role in roles)
                {
                    userEntity.AddRole(role.RoleName);
                }
                session.Save(userEntity);
                session.SumbitChanges();
            }
            _log.Trace(m => m("User successfully added"));
        }

        public void SaveUser(User user)
        {
            using (var session = OpenSesion())
            {
                session.UseOptimisticConcurrency();

                var userEntity = session.Query<UserEntity>().Single(u => u.UserId == user.UserId);
                userEntity.UpdateUser(user);
                _log.UpdatedUserData(user, userEntity);

                UpdateUsersRolesList(session, userEntity, user.Roles);

                session.Save(userEntity);
                session.SumbitChanges();
            }
        }

        public void DeleteUser(User user)
        {
            using (var session = OpenSesion())
            {
                var userEntity = session.Query<UserEntity>().Single(u => u.UserId == user.UserId);
                session.Delete(userEntity);
                session.SumbitChanges();
            }
        }

        private void UpdateUsersRolesList(IStorageSession session, UserEntity userEntity, IEnumerable<string> newRoles)
        {
            var newRolesList = newRoles.Any()
                                   ? session.Query<RoleEntity>(staleResults: StaleResultsMode.WaitForNonStaleResults)
                                         .ContainsIn(r => r.RoleName, newRoles)
                                         .ToList()
                                   : Enumerable.Empty<RoleEntity>();

            var roleNamesToRemove = userEntity.GetRolesToRemove(newRoles);
            foreach (var roleName in roleNamesToRemove)
            {
                if (userEntity.RemoveRole(roleName))
                {
                    _log.Debug(m => m("remove role by name {0}", roleName));
                }
            }

            foreach (var roleEntity in newRolesList)
            {
                if (userEntity.AddRole(roleEntity.RoleName))
                {
                    _log.Debug(m => m("Added user {0} to role {1}", userEntity, roleEntity));
                }
            }
        }
    }
}