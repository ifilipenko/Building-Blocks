using System;
using System.Linq;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow;
using TechTalk.SpecFlow;

namespace BuildingBlocks.Membership.RavenDB.Tests.Steps
{
    [Binding]
    public class RoleStorageSteps
    {
        [Given(@"существуют роли")]
        public void ДопустимСуществуютРоли(Table table)
        {
            foreach (var roleName in table.Rows.Select(r => r["имя"]))
            {
                var role = new RoleEntity
                    {
                        ApplicationName = MembershipSettings.DefaultApplicationName,
                        RoleId = Guid.NewGuid(),
                        RoleName = roleName,
                        Description = roleName
                    };
                RavenDb.CurrentSession.Store(role);
            }
        }
    }
}