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
        private readonly IStorageSession _session;
        
        public RoleRepositoryImpl(IStorageSession session)
        {
            _session = session;
        }

        public bool IsRoleExists(string roleName)
        {
            return _session.Query<RoleEntity>().Any(r => r.RoleName == roleName);
        }

        public IEnumerable<Role> GetAll()
        {
            var roles = _session.Query<RoleEntity>().OrderBy(r => r.RoleName).ToList();
            return roles.Select(r => r.ToRole()).ToList();
        }

        public IEnumerable<Role> FindRolesByNames(params string[] roleNames)
        {
            var roles = _session.Query<RoleEntity>()
                .ContainsIn(r => r.RoleName, roleNames)
                .OrderBy(r => r.RoleName)
                .ToList();
            return roles.Select(r => r.ToRole()).ToList();
        }

        public void CreateRole(Role role)
        {
            var roleEntity = role.ToEntityWithoutUsers();
            var users = _session.Query<UserEntity>()
                .ContainsIn(r => r.Username, role.Users)
                .ToList();
            foreach (var user in users)
            {
                roleEntity.AddUserOrUpdate(user);
            }
            _session.Save(roleEntity);

            foreach (var user in users)
            {
                user.AddRoleOrUpdate(roleEntity);
                _session.Save(user);
            }
        }

        public void DeleteRole(Role role)
        {
            var rolesToDelete = _session.Query<RoleEntity>().Where(r => r.RoleName == role.RoleName).ToList();
            foreach (var roleToDelete in rolesToDelete)
            {
                foreach (var userReference in roleToDelete.Users)
                {
                    var user = _session.GetById<UserEntity>(userReference.Id);
                    if (user != null)
                    {
                        user.RemoveRole(roleToDelete);
                        _session.Save(user);
                    }
                }
                _session.Delete(roleToDelete);
            }
        }
    }
}