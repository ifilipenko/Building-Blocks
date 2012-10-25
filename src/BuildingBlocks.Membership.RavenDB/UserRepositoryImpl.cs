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

namespace BuildingBlocks.Membership.RavenDB
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly IStorageSession _session;

        public UserRepositoryImpl(IStorageSession session)
        {
            _session = session;
        }

        public bool HasUserWithName(string applicationName, string username)
        {
            return _session.Query<UserEntity>().Any(u => u.Username == username);
        }

        public bool HasUserWithEmail(string applicationName, string email)
        {
            return _session.Query<UserEntity>().Any(u => u.Email == email);
        }

        public IEnumerable<User> FindUsersByNames(string applicationName, params string[] usernames)
        {
            var users = _session.Query<UserEntity>().ContainsIn(u => u.Username, usernames).OrderBy(u => u.Username).ToList();
            return users.Select(u => u.ToUser()).ToList();
        }

        public User FindUserByEmail(string applicationName, string email)
        {
            var user = _session.Query<UserEntity>().FirstOrDefault(u => u.ApplicationName == applicationName && u.Email == email);
            return user == null ? null : user.ToUser();
        }

        public User FindUserById(Guid userId)
        {
            var user = _session.Query<UserEntity>().FirstOrDefault(u => u.UserId == userId);
            return user == null ? null : user.ToUser();
        }

        public Page<User> GetUsersPageByEmail(string applicationName, string emailToMatch, int pageIndex, int pageSize)
        {
            IQuery<FindByEmailSubstring, Page<User>> query = new UsersColumnMatchedToSubstring(_session);
            var page = query.Execute(new FindByEmailSubstring(emailToMatch, pageIndex + 1, pageSize));
            return page;
        }

        public Page<User> GetUsersPageByUsername(string applicationName, string usernameToMatch, int pageIndex, int pageSize)
        {
            IQuery<FindByUsernameSubstring, Page<User>> query = new UsersColumnMatchedToSubstring(_session);
            var page = query.Execute(new FindByUsernameSubstring(usernameToMatch, pageIndex + 1, pageSize));
            return page;
        }

        public Page<User> GetUsersPage(string applicationName, int pageIndex, int pageSize)
        {
            return Pagination.From(_session.Query<UserEntity>().OrderBy(u => u.Username))
                .Page(pageIndex + 1, pageSize)
                .GetPageWithItemsMappedBy(u => u.ToUser());
        }

        public int GetUsersCountWithLastActivityDateGreaterThen(string applicationName, DateTime dateActive)
        {
            return _session.Query<UserEntity>().Count(u => u.LastActivityDate > dateActive);
        }

        public void AddUser(User newUser)
        {
            var userEntity = newUser.ToEntityWithoutRoles();
            var roles = _session.Query<RoleEntity>().ContainsIn(r => r.RoleName, newUser.Roles).ToList();
            foreach (var role in roles)
            {
                userEntity.Roles.Add(new RoleReference(role));
            }
            _session.Save(userEntity);

            foreach (var role in roles)
            {
                role.Users.Add(new UserReference(userEntity));
                _session.Save(role);
            }
        }

        public void SaveUser(User user)
        {
            var userEntity = _session.Query<UserEntity>().Single(u => u.UserId == user.UserId);
            userEntity.UpdateUser(user);

            UpdateUsersRolesList(userEntity, user.Roles);

            _session.Save(userEntity);
        }

        public void DeleteUser(User user)
        {
            var userEntity = _session.Query<UserEntity>().Single(u => u.UserId == user.UserId);
            var roles = _session.Query<RoleEntity>().ContainsIn(r => r.RoleName, user.Roles).ToList();
            foreach (var role in roles)
            {
                role.RemoveUser(userEntity);
                _session.Save(role);
            }
            _session.Delete(userEntity);
        }

        private void UpdateUsersRolesList(UserEntity userEntity, IEnumerable<string> newRoles)
        {
            var newRolesList = _session.Query<RoleEntity>().ContainsIn(r => r.RoleName, newRoles).ToList();
            var roleIdsToRemove = userEntity.GetRoleIdsToRemove(newRolesList);

            foreach (var roleId in roleIdsToRemove)
            {
                var roleEntity = _session.GetById<RoleEntity>(roleId);
                if (roleEntity == null)
                {
                    userEntity.RemoveRoleWithId(roleId);
                }
                else
                {
                    userEntity.RemoveRole(roleEntity);
                    roleEntity.RemoveUser(userEntity);
                    _session.Save(roleEntity);
                }
            }

            foreach (var roleEntity in newRolesList)
            {
                userEntity.AddRoleOrUpdate(roleEntity);
                roleEntity.AddUserOrUpdate(userEntity);
                _session.Save(roleEntity);
            }
        }
    }
}