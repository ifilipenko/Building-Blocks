using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Membership.RavenDB.Queries.Criteria;
using BuildingBlocks.Query;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB
{
    public class UserRepositoryImpl : IUserRepository
    {
        private readonly IStorageSession _session;
        private readonly IQueryBuilder _queryBuilder;

        public UserRepositoryImpl(IStorageSession session, IQueryBuilder queryBuilder)
        {
            _session = session;
            _queryBuilder = queryBuilder;
        }

        public bool HasUserWithName(string username)
        {
            return _session.Query<UserEntity>().Any(u => u.Username == username);
        }

        public bool HasUserWithEmail(string email)
        {
            return _session.Query<UserEntity>().Any(u => u.Email == email);
        }

        public IEnumerable<User> FindUsersByNames(params string[] usernames)
        {
            var users = _session.Query<UserEntity>().ContainsIn(u => u.Username, usernames).OrderBy(u => u.Username).ToList();
            return users.Select(u => u.ToUser()).ToList();
        }

        public User FindUserByEmail(string email)
        {
            var user = _session.Query<UserEntity>().FirstOrDefault(u => u.Email == email);
            return user == null ? null : user.ToUser();
        }

        public User FindUserById(Guid userId)
        {
            var user = _session.Query<UserEntity>().FirstOrDefault(u => u.UserId == userId);
            return user == null ? null : user.ToUser();
        }

        public Page<User> GetUsersPageByEmail(string emailToMatch, int pageIndex, int pageSize)
        {
            var page = _queryBuilder.For<User>()
                .With(new FindByEmailSubstring(emailToMatch, pageIndex + 1, pageSize))
                .Page();
            return page;
        }

        public Page<User> GetUsersPageByUsername(string usernameToMatch, int pageIndex, int pageSize)
        {
            var page = _queryBuilder.For<User>()
                .With(new FindByUsernameSubstring(usernameToMatch, pageIndex + 1, pageSize))
                .Page();
            return page;
        }

        public Page<User> GetUsersPage(int pageIndex, int pageSize)
        {
            return Pagination.From(_session.Query<UserEntity>())
                .Page(pageIndex + 1, pageSize)
                .GetPageWithItemsMappedBy(u => u.ToUser());
        }

        public int GetUsersCountWithLastActivityDateGreaterThen(DateTime dateActive)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User newUser)
        {
            throw new NotImplementedException();
        }

        public void SaveUser(User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteUser(User user)
        {
            throw new NotImplementedException();
        }
    }
}