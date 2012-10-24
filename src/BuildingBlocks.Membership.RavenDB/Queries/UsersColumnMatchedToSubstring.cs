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
    public class UsersColumnMatchedToSubstring : IQuery<FindByEmailSubstring, Page<User>>, IQuery<FindByUsernameSubstring, Page<User>>
    {
        private readonly IStorageSession _session;

        public UsersColumnMatchedToSubstring(IStorageSession session)
        {
            _session = session;
        }

        Page<User> IQuery<FindByEmailSubstring, Page<User>>.Execute(FindByEmailSubstring criteria)
        {
            return GetPageOfUsersMatchedToTerm("Email", criteria.EmailSubstring, criteria);
        }

        Page<User> IQuery<FindByUsernameSubstring, Page<User>>.Execute(FindByUsernameSubstring criteria)
        {
            return GetPageOfUsersMatchedToTerm("Username", criteria.UsernameSubstring, criteria);
        }

        private Page<User> GetPageOfUsersMatchedToTerm(string column, string term, PageCriteria pageCriteria)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return Pagination
                    .From(_session.Query<UserEntity>())
                    .Page(pageCriteria.PageNumber, pageCriteria.PageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
            }

            var session = ((RavenDbSession)_session).Session;
            var allMatchedUsers = session.Advanced.LuceneQuery<UserEntity>()
                .Where(string.Format("{0}: *{1}*", column, term))
                .WaitForNonStaleResultsAsOfLastWrite()
                .ToList();

            return Pagination
                .From(allMatchedUsers.AsQueryable())
                .Page(pageCriteria.PageNumber, pageCriteria.PageSize)
                .GetPageWithItemsMappedBy(u => u.ToUser());
        }
    }
}