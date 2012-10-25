using System.Collections.Generic;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership.Contract
{
    public interface IRoleRepository
    {
        bool IsRoleExists(string applicationName, string roleName);

        IEnumerable<Role> GetAll(string applicationName);
        IEnumerable<Role> FindRolesByNames(string applicationName, params string[] roleNames);

        void CreateRole(Role role);
        void DeleteRole(Role role);
    }
}