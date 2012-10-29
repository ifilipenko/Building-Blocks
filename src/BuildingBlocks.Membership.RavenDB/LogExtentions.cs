using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using Common.Logging;

namespace BuildingBlocks.Membership.RavenDB
{
    public static class LogExtentions
    {
        public static void FoundedRolesByParameters(this ILog log, IEnumerable<RoleEntity> founded, IEnumerable<string> given)
        {
            founded = founded ?? Enumerable.Empty<RoleEntity>();
            log.Debug(m => m("For user founded roles array [{0}] by given roles array [{1}]", 
                founded.Select(r => r.RoleName).JoinToString(), 
                (given ?? Enumerable.Empty<string>()).JoinToString()));
        }
    }
}