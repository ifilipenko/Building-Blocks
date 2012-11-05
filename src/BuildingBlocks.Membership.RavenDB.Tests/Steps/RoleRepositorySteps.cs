using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store.RavenDB;
using BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow;
using FluentAssertions;
using TechTalk.SpecFlow;

namespace BuildingBlocks.Membership.RavenDB.Tests.Steps
{
    [Binding]
    public class RoleRepositorySteps
    {
        private bool RoleExistsResult
        {
            get { return ScenarioContext.Current.Get<bool>("RoleExistsResult"); }
            set { ScenarioContext.Current.Set(value, "RoleExistsResult"); }
        }

        private IEnumerable<string> RolesResult
        {
            get { return ScenarioContext.Current.Get<IEnumerable<string>>("RolesResult"); }
            set { ScenarioContext.Current.Set(value, "RolesResult"); }
        }

        [When(@"провер€ют что роль ""(.*)"" существует")]
        public void ≈слиѕровер€ют„то–оль—уществует(string roleName)
        {
            var roleRepository = new RoleRepositoryImpl(RavenDb.Storage);
            RoleExistsResult = roleRepository.IsRoleExists(MembershipSettings.DefaultApplicationName, roleName);
        }

        [When(@"получают список ролей")]
        public void ≈слиѕолучают—писок–олей()
        {
            var roleRepository = new RoleRepositoryImpl(RavenDb.Storage);
            RolesResult = roleRepository.GetAll(MembershipSettings.DefaultApplicationName);
        }

        [When(@"получают список ролей содержащих имена")]
        public void ≈слиѕолучают—писок–олей—одержащих»мена(Table table)
        {
            var namesList = table.Rows.Select(r => r["им€"]).ToArray();
            var roleRepository = new RoleRepositoryImpl(RavenDb.Storage);
            RolesResult = roleRepository.FindRolesByNames(MembershipSettings.DefaultApplicationName, namesList);
        }

        [When(@"создают роль ""(.*)""")]
        public void ≈сли—оздают–оль(string roleName)
        {
            var roleRepository = new RoleRepositoryImpl(RavenDb.Storage);
            roleRepository.CreateRole(Guid.NewGuid(), MembershipSettings.DefaultApplicationName, roleName);
        }

        [When(@"удал€ют роль ""(.*)""")]
        public void ≈сли”дал€ют–оль(string roleName)
        {
            var roleRepository = new RoleRepositoryImpl(RavenDb.Storage);
            roleRepository.DeleteRole(MembershipSettings.DefaultApplicationName, roleName);
        }

        [Then(@"результат проверки признает что роль ""(.*)""")]
        public void “о–езультатѕроверкиѕризнает„то–оль—уществует(bool expectedValue)
        {
            RoleExistsResult.Should().Be(expectedValue);
        }

        [Then(@"возвращаетс€ следующий список ролей")]
        public void “о¬озвращаетс€—ледующий—писок–олей(Table table)
        {
            RolesResult.Should().HaveSameCount(table.Rows);
            for (var i = 0; i < table.RowCount; i++)
            {
                var expectedRoleName = table.Rows[i]["им€"];
                var actualRole = RolesResult.ElementAt(i);
                actualRole.Should().Be(expectedRoleName);
            }
        }

        [Then(@"существует ""(.*)"" роли")]
        [Then(@"существует ""(.*)"" ролей")]
        public void “о—уществует–оли(int count)
        {
            RavenDb.CurrentStorageSession.Query<RoleEntity>().WaitForNonStaleResultsAsOfLastWrite().Count().Should().Be(count);
        }

        [Then(@"существует роль ""(.*)"" со списком пользователей")]
        public void “о—уществует–оль—о—пискомѕользователей(string roleName, Table table)
        {
            var allUsers = RavenDb.CurrentSession.Query<UserEntity>().WaitForNonStaleResultsAsOfLastWrite().ToList();
            var allRoles = RavenDb.CurrentSession.Query<RoleEntity>().WaitForNonStaleResultsAsOfLastWrite().ToList();
            allRoles.Should().Contain(r => r.RoleName == roleName);

            foreach (var username in table.Rows.Select(r => r["им€"]))
            {
                var user = allUsers.Single(u => u.Username == username);
                user.Roles.Should().Contain(roleName);
            }
        }
    }
}