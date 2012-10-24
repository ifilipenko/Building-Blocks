using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Membership.RavenDB.Queries.Criteria;
using BuildingBlocks.Query;
using BuildingBlocks.Store;
using BuildingBlocks.Store.RavenDB;

namespace BuildingBlocks.Membership.RavenDB.Queries
{
    public class UserWithMatchedEmails : IQuery<FindByEmailSubstring, Page<User>>
    {
        private readonly IStorageSession _session;

        public UserWithMatchedEmails(IStorageSession session)
        {
            _session = session;
        }

        public Page<User> Execute(FindByEmailSubstring criteria)
        {
            if (string.IsNullOrWhiteSpace(criteria.Email))
            {
                return Pagination
                    .From(_session.Query<UserEntity>())
                    .Page(criteria.PageNumber, criteria.PageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
            }

            var session = ((RavenDbSession) _session).Session;
            var allMatchedUsers = session.Advanced.LuceneQuery<UserEntity>()
                .Where("Email: *" + criteria.Email + "*")
                .WaitForNonStaleResultsAsOfLastWrite()
                .ToList();

            return Pagination
                    .From(allMatchedUsers.AsQueryable())
                    .Page(criteria.PageNumber, criteria.PageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
        }
    }

    public class UserWithMatchedUsernames : IQuery<FindByEmailSubstring, Page<User>>
    {
        private readonly IStorageSession _session;

        public UserWithMatchedUsernames(IStorageSession session)
        {
            _session = session;
        }

        public Page<User> Execute(FindByEmailSubstring criteria)
        {
            if (string.IsNullOrWhiteSpace(criteria.Email))
            {
                return Pagination
                    .From(_session.Query<UserEntity>())
                    .Page(criteria.PageNumber, criteria.PageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
            }

            var session = ((RavenDbSession)_session).Session;
            var allMatchedUsers = session.Advanced.LuceneQuery<UserEntity>()
                .Where("Email: *" + criteria.Email + "*")
                .WaitForNonStaleResultsAsOfLastWrite()
                .ToList();

            return Pagination
                    .From(allMatchedUsers.AsQueryable())
                    .Page(criteria.PageNumber, criteria.PageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
        }
    }
}
