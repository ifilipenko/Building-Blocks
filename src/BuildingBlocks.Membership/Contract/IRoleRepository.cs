using System;
using System.Collections.Generic;

namespace BuildingBlocks.Membership.Contract
{
    public interface IRoleRepository
    {
        bool IsRoleExists(string applicationName, string roleName);

        IEnumerable<string> GetAll(string applicationName);
        IEnumerable<string> FindRolesByNames(string applicationName, params string[] roleNames);

        void CreateRole(Guid roleId, string applicationName, string roleName);
        void DeleteRole(string applicationName, string roleName);
    }
}