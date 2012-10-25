using System.Linq;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership.RavenDB.DomainModel
{
    public static class RoleEntityMappingHelpers
    {
        public static Role ToRole(this RoleEntity entity)
        {
            var role = new Role(entity.RoleId,entity.RoleName,entity.ApplicationName)
                {
                    Description = entity.Description,
                    Users = entity.Users.Select(u => u.Name).ToList()
                };
            return role;
        }

        public static RoleEntity ToEntityWithoutUsers(this Role role)
        {
            return new RoleEntity
                {
                    Description = role.Description,
                    RoleId = role.RoleId,
                    RoleName = role.RoleName,
                    ApplicationName = role.ApplicationName
                };
        }
    }
}