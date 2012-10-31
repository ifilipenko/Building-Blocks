using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store;
using BuildingBlocks.Store.RavenDB;

namespace BuildingBlocks.Membership.RavenDB
{
    public class RoleRepositoryImpl : IRoleRepository
    {
        private readonly IStorage _storage;

        public RoleRepositoryImpl(IStorage storage)
        {
            _storage = storage;
        }

        public bool IsRoleExists(string applicationName, string roleName)
        {
            using (var session = _storage.OpenSesion())
            {
                return session.Query<RoleEntity>().Any(r => r.ApplicationName == applicationName && r.RoleName == roleName);
            }
        }

        public IEnumerable<Role> GetAll(string applicationName)
        {
            using (var session = _storage.OpenSesion())
            {
                var roles = session.Query<RoleEntity>().OrderBy(r => r.RoleName).ToList();
                return roles.Select(r => r.ToRole()).ToList();
            }
        }

        public IEnumerable<Role> FindRolesByNames(string applicationName, params string[] roleNames)
        {
            using (var session = _storage.OpenSesion())
            {
                var roles = session.Query<RoleEntity>()
                    .WaitForNonStaleResultsAsOfLastWrite()
                    .Where(r => r.ApplicationName == applicationName)
                    .ContainsIn(r => r.RoleName, roleNames)
                    .OrderBy(r => r.RoleName)
                    .ToList();
                return roles.Select(r => r.ToRole()).ToList();
            }
        }

        public void CreateRole(Role role)
        {
            using (var session = _storage.OpenSesion())
            {
                var roleEntity = role.ToEntityWithoutUsers();
                var users = role.Users.Any()
                                ? session.Query<UserEntity>()
                                      .WaitForNonStaleResultsAsOfLastWrite()
                                      .Where(r => r.ApplicationName == role.ApplicationName)
                                      .ContainsIn(r => r.Username, role.Users)
                                      .ToList()
                                : Enumerable.Empty<UserEntity>();
                foreach (var user in users)
                {
                    roleEntity.AddUserOrUpdate(user);
                }
                session.Save(roleEntity);

                foreach (var user in users)
                {
                    user.AddRoleOrUpdate(roleEntity);
                    session.Save(user);
                }
                session.SumbitChanges();
            }
        }

        public void DeleteRole(Role role)
        {
            using (var session = _storage.OpenSesion())
            {
                var rolesToDelete = session.Query<RoleEntity>().Where(r => r.RoleName == role.RoleName).ToList();
                foreach (var roleToDelete in rolesToDelete)
                {
                    foreach (var userReference in roleToDelete.Users)
                    {
                        var user = session.GetById<UserEntity>(userReference.Id);
                        if (user != null)
                        {
                            user.RemoveRole(roleToDelete);
                            session.Save(user);
                        }
                    }
                    session.Delete(roleToDelete);
                }
                session.SumbitChanges();
            }
        }
    }
}