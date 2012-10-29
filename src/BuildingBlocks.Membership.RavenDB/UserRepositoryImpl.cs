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
    public class UserRepositoryImpl : IUserRepository
    {
        private static readonly ILog _log = LogManager.GetLogger<UserRepositoryImpl>();
        private readonly IStorage _storage;

        public UserRepositoryImpl(IStorage storage)
        {
            _storage = storage;
        }

        public bool HasUserWithName(string applicationName, string username)
        {
            using (var session = _storage.OpenSesion())
            {
                return session.Query<UserEntity>().Any(u => u.Username == username);
            }
        }

        public bool HasUserWithEmail(string applicationName, string email)
        {
            using (var session = _storage.OpenSesion())
            {
                return session.Query<UserEntity>().Any(u => u.Email == email);
            }
        }

        public IEnumerable<User> FindUsersByNames(string applicationName, params string[] usernames)
        {
            using (var session = _storage.OpenSesion())
            {
                var users = session.Query<UserEntity>().ContainsIn(u => u.Username, usernames).OrderBy(u => u.Username).ToList();
                return users.Select(u => u.ToUser()).ToList();
            }
        }

        public User FindUserByEmail(string applicationName, string email)
        {
            using (var session = _storage.OpenSesion())
            {
                var user = session.Query<UserEntity>()
                    .FirstOrDefault(u => u.ApplicationName == applicationName && u.Email == email);
                return user == null ? null : user.ToUser();
            }
        }

        public User FindUserById(Guid userId)
        {
            using (var session = _storage.OpenSesion())
            {
                var user = session.Query<UserEntity>().FirstOrDefault(u => u.UserId == userId);
                return user == null ? null : user.ToUser();
            }
        }

        public Page<User> GetUsersPageByEmail(string emailToMatch, string applicationName, int pageIndex, int pageSize)
        {
            using (var session = _storage.OpenSesion())
            {
                IQuery<FindByEmailSubstring, Page<User>> query = new UsersColumnMatchedToSubstring(session);
                var page = query.Execute(new FindByEmailSubstring(emailToMatch, applicationName, pageIndex + 1, pageSize));
                return page;
            }
        }

        public Page<User> GetUsersPageByUsername(string usernameToMatch, string applicationName, int pageIndex, int pageSize)
        {
            using (var session = _storage.OpenSesion())
            {
                IQuery<FindByUsernameSubstring, Page<User>> query = new UsersColumnMatchedToSubstring(session);
                var page = query.Execute(new FindByUsernameSubstring(usernameToMatch, applicationName, pageIndex + 1, pageSize));
                return page;
            }
        }

        public Page<User> GetUsersPage(string applicationName, int pageIndex, int pageSize)
        {
            using (var session = _storage.OpenSesion())
            {
                return Pagination.From(session.Query<UserEntity>().OrderBy(u => u.Username))
                    .Page(pageIndex + 1, pageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
            }
        }

        public int GetUsersCountWithLastActivityDateGreaterThen(string applicationName, DateTime dateActive)
        {
            using (var session = _storage.OpenSesion())
            {
                return session.Query<UserEntity>().Count(u => u.LastActivityDate > dateActive);
            }
        }

        public void AddUser(User newUser)
        {
            _log.Trace(m => m("Adding user started"));
            using (var session = _storage.OpenSesion())
            {
                var userEntity = newUser.ToEntityWithoutRoles();
                var roles = session.Query<RoleEntity>().ContainsIn(r => r.RoleName, newUser.Roles).ToList();
                _log.FoundedRolesByParameters(roles, newUser.Roles);

                foreach (var role in roles)
                {
                    userEntity.Roles.Add(new RoleReference(role));
                }
                session.Save(userEntity);

                foreach (var role in roles)
                {
                    role.Users.Add(new UserReference(userEntity));
                    session.Save(role);
                }
                session.SumbitChanges();
            }
            _log.Trace(m => m("User successfully added"));
        }

        public void SaveUser(User user)
        {
            using (var session = _storage.OpenSesion())
            {
                var userEntity = session.Query<UserEntity>().Single(u => u.UserId == user.UserId);
                userEntity.UpdateUser(user);

                UpdateUsersRolesList(userEntity, user.Roles);

                session.Save(userEntity);
                session.SumbitChanges();
            }
        }

        public void DeleteUser(User user)
        {
            using (var session = _storage.OpenSesion())
            {
                var userEntity = session.Query<UserEntity>().Single(u => u.UserId == user.UserId);
                var roles = session.Query<RoleEntity>().ContainsIn(r => r.RoleName, user.Roles).ToList();
                foreach (var role in roles)
                {
                    role.RemoveUser(userEntity);
                    session.Save(role);
                }
                session.Delete(userEntity);
                session.SumbitChanges();
            }
        }

        private void UpdateUsersRolesList(UserEntity userEntity, IEnumerable<string> newRoles)
        {
            using (var session = _storage.OpenSesion())
            {
                var newRolesList = session.Query<RoleEntity>().ContainsIn(r => r.RoleName, newRoles).ToList();
                var roleIdsToRemove = userEntity.GetRoleIdsToRemove(newRolesList);

                foreach (var roleId in roleIdsToRemove)
                {
                    var roleEntity = session.GetById<RoleEntity>(roleId);
                    if (roleEntity == null)
                    {
                        userEntity.RemoveRoleWithId(roleId);
                    }
                    else
                    {
                        userEntity.RemoveRole(roleEntity);
                        roleEntity.RemoveUser(userEntity);
                        session.Save(roleEntity);
                    }
                }

                foreach (var roleEntity in newRolesList)
                {
                    userEntity.AddRoleOrUpdate(roleEntity);
                    roleEntity.AddUserOrUpdate(userEntity);
                    session.Save(roleEntity);
                }
                session.SumbitChanges();
            }
        }
    }
}