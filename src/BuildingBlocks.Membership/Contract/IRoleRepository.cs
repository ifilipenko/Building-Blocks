using System.Collections.Generic;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership.Contract
{
    public interface IRoleRepository
    {
        IEnumerable<Role> GetAll();
        Role FindByName(string roleName);
        IEnumerable<Role> FindRolesByNames(string[] roleNames);
        bool IsRoleExists(string roleName);
        void CreateRole(Role role);
        void DeleteRole(Role role);
    }
}