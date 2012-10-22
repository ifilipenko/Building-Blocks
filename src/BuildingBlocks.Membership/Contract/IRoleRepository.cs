using System.Collections.Generic;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership.Contract
{
    public interface IRoleRepository
    {
        bool IsRoleExists(string roleName);

        IEnumerable<Role> GetAll();
        IEnumerable<Role> FindRolesByNames(params string[] roleNames);

        void CreateRole(Role role);
        void DeleteRole(Role role);
    }
}