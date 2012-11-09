using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store;

namespace BuildingBlocks.Membership.RavenDB
{
    public class RoleRepositoryImpl : IRoleRepository
    {
        private readonly IStorageSession _outsideSession;
        private readonly IStorage _storage;

        public RoleRepositoryImpl(IStorage storage)
        {
            _storage = storage;
        }

        public RoleRepositoryImpl(IStorageSession outsideSession)
        {
            _outsideSession = outsideSession;
        }

        public bool IsRoleExists(string applicationName, string roleName)
        {
            using (var session = OpenSesion())
            {
                return session.Query<RoleEntity>().Any(r => r.ApplicationName == applicationName && r.RoleName == roleName);
            }
        }

        public IEnumerable<string> GetAll(string applicationName)
        {
            using (var session = OpenSesion())
            {
                var roles = session.Query<RoleEntity>()
                    .OrderBy(r => r.RoleName)
                    .Select(r => r.RoleName)
                    .ToList();
                return roles;
            }
        }

        public IEnumerable<string> FindRolesByNames(string applicationName, params string[] roleNames)
        {
            using (var session = OpenSesion())
            {
                var roles = session.Query<RoleEntity>(staleResults: StaleResultsMode.WaitForNonStaleResults)
                    .Where(r => r.ApplicationName == applicationName)
                    .ContainsIn(r => r.RoleName, roleNames)
                    .OrderBy(r => r.RoleName)
                    .Select(r => r.RoleName)
                    .ToList();
                return roles;
            }
        }

        public void CreateRole(Guid roleId, string applicationName, string roleName)
        {
            using (var session = OpenSesion())
            {
                var roleEntity = new RoleEntity
                {
                    ApplicationName = applicationName,
                    RoleId = roleId,
                    RoleName = roleName
                };
                session.Save(roleEntity);
                session.SumbitChanges();
            }
        }

        public void DeleteRole(string applicationName, string roleName)
        {
            using (var session = OpenSesion())
            {
                var rolesToDelete = session.Query<RoleEntity>()
                    .Where(r => r.ApplicationName == applicationName && r.RoleName == roleName)
                    .ToList();
                foreach (var roleToDelete in rolesToDelete)
                {
                    session.Delete(roleToDelete);
                }
                session.SumbitChanges();
            }
        }

        private IStorageSession OpenSesion()
        {
            return _storage == null
                       ? new OutsideSessionDecorator(_outsideSession)
                       : _storage.OpenSesion();
        }
    }
}