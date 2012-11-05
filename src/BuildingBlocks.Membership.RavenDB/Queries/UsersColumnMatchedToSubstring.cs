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
            return GetPageOfUsersMatchedToTerm("Email", criteria.EmailSubstring, criteria.ApplicationName, criteria);
        }

        Page<User> IQuery<FindByUsernameSubstring, Page<User>>.Execute(FindByUsernameSubstring criteria)
        {
            return GetPageOfUsersMatchedToTerm("Username", criteria.UsernameSubstring, criteria.ApplicationName, criteria);
        }

        private Page<User> GetPageOfUsersMatchedToTerm(string column, string term, string applicationName, PageCriteria pageCriteria)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                var usersQuery = _session.Query<UserEntity>();
                if (!string.IsNullOrWhiteSpace(applicationName))
                {
                    usersQuery = usersQuery.Where(u => u.ApplicationName == applicationName);
                }
                return Pagination
                    .From(usersQuery.OrderBy(u => u.Username))
                    .Page(pageCriteria.PageNumber, pageCriteria.PageSize)
                    .GetPageWithItemsMappedBy(u => u.ToUser());
            }

            var session = ((RavenDbSession) _session).Session;
            var query = session.Advanced.LuceneQuery<UserEntity>();
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                query = session.Advanced.LuceneQuery<UserEntity>()
                    .Where(string.Format("{0}:*{1}*", column, term));
            }
            else
            {
                query = session.Advanced.LuceneQuery<UserEntity>()
                    .Where(string.Format("{0}:*{1}* AND ApplicationName:{2}", column, term, applicationName));
            }
            var allMatchedUsers = query
                .OrderBy(u => u.Username)
                .WaitForNonStaleResultsAsOfLastWrite()
                .ToList();

            return Pagination
                .From(allMatchedUsers.AsQueryable())
                .Page(pageCriteria.PageNumber, pageCriteria.PageSize)
                .GetPageWithItemsMappedBy(u => u.ToUser());
        }
    }
}