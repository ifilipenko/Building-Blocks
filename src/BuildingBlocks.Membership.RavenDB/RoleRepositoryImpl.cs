using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB
{
    public class RoleRepositoryImpl : IRoleRepository
    {
        private readonly IStorageSession _storageSession;
        
        public RoleRepositoryImpl(IStorageSession storageSession)
        {
            _storageSession = storageSession;
        }

        public bool IsRoleExists(string roleName)
        {
            return _storageSession.Query<RoleEntity>().Any(r => r.RoleName == roleName);
        }

        public IEnumerable<Role> GetAll()
        {
            var roles = _storageSession.Query<RoleEntity>().OrderBy(r => r.RoleName).ToList();
            return roles.Select(r => r.ToRole()).ToList();
        }

        public IEnumerable<Role> FindRolesByNames(params string[] roleNames)
        {
            var roles = _storageSession.Query<RoleEntity>()
                .ContainsIn(r => r.RoleName, roleNames)
                .OrderBy(r => r.RoleName)
                .ToList();
            return roles.Select(r => r.ToRole()).ToList();
        }

        public void CreateRole(Role role)
        {
            var entity = role.ToEntityWithoutUsers();
            var users = _storageSession.Query<UserEntity>()
                .ContainsIn(r => r.Username, role.Users)
                .ToList();
            foreach (var user in users)
            {
                entity.Users.Add(new UserReference(user));
            }
            _storageSession.Save(entity);
        }

        public void DeleteRole(Role role)
        {
            var rolesToDelete = _storageSession.Query<RoleEntity>().Where(r => r.RoleName == role.RoleName).ToList();
            foreach (var roleToDelete in rolesToDelete)
            {
                _storageSession.Delete(roleToDelete);
            }
        }
    }
}