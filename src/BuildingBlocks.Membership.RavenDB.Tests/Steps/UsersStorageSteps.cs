using System;
using System.Linq;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store.RavenDB;
using BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow;
using FluentAssertions;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace BuildingBlocks.Membership.RavenDB.Tests.Steps
{
    [Binding]
    public class UsersStorageSteps
    {
        [Given(@"существуют пользователи")]
        public void ƒопустим—уществуют–оли(Table table)
        {
            foreach (var row in table.Rows)
            {
                var username = row["им€"];
                var email = row.ContainsKey("email") ? row["email"] : username + "@mail.com";
                var user = new UserEntity
                    {
                        ApplicationName = MembershipSettings.ApplicationName,
                        UserId = Guid.NewGuid(),
                        Username = username,
                        Email = email, 
                        Password = "123"
                    };
                RavenDb.CurrentSession.Store(user);
            }
        }

        [Given(@"задана активность пользователей")]
        public void ƒопустим«аданајктивностьѕользователей(Table table)
        {
            var session = RavenDb.CurrentStorageSession;
            foreach (var row in table.CreateDynamicSet())
            {
                string username = row.им€;
                var user = session.Query<UserEntity>().Single(u => u.Username == username);
                user.LastActivityDate = (DateTime)row.активность;
                session.Save(user);
            }
        }

        [Given(@"пользователь ""(.*)"" имеет Id ""(.*)""")]
        public void ƒопустимѕользователь»меетId(string userName, string id)
        {
            var user = RavenDb.CurrentStorageSession.Query<UserEntity>().Single(u => u.Username == userName);
            user.UserId = new Guid(id);
            RavenDb.CurrentStorageSession.Save(user);
        }

        [Then(@"пользователь с Id ""(.*)"" имеет следующие пол€")]
        public void “оѕользователь—Id»меет—ледующиеѕол€(Guid id, Table table)
        {
            dynamic data = table.CreateDynamicInstance();

            RavenDb.CurrentStorageSession.Query<UserEntity>().Should().Contain(u => u.UserId == id);
            var user = RavenDb.CurrentStorageSession.Query<UserEntity>().Single(u => u.UserId == id);

            user.Password                                .Should().Be((string)  data.ѕароль);
            user.Username                                .Should().Be((string)  data.»м€);
            user.Email                                   .Should().Be((string)  data.Email);
            user.ApplicationName                         .Should().Be((string)  data.ApplicationName);
            user.Comment                                 .Should().Be((string)  data. омментарий);
            user.ConfirmationToken                       .Should().Be((string)  data.ConfirmationToken);
            user.CreateDate                              .Should().Be((DateTime)data.CreateDate);
            user.IsApproved                              .Should().Be((bool)    data.IsApproved);
            user.IsLockedOut                             .Should().Be((bool)    data.IsLockedOut);
            user.LastActivityDate                        .Should().Be((DateTime)data.LastActivityDate);
            user.LastLockoutDate                         .Should().Be((DateTime)data.LastLockoutDate);
            user.LastLoginDate                           .Should().Be((DateTime)data.LastLoginDate);
            user.LastPasswordChangedDate                 .Should().Be((DateTime)data.LastPasswordChangedDate);
            user.LastPasswordFailureDate                 .Should().Be((DateTime)data.LastPasswordFailureDate);
            user.PasswordFailuresSinceLastSuccess        .Should().Be((int)     data.PasswordFailuresSinceLastSuccess);
            user.PasswordVerificationToken               .Should().Be((string)  data.PasswordVerificationToken);
            user.PasswordVerificationTokenExpirationDate .Should().Be((DateTime)data.PasswordVerificationTokenExpirationDate);
        }

        [Then(@"существует (.*) пользовател€")]
        public void “о—уществуетѕользовател€(int count)
        {
            var actualCount = RavenDb.CurrentStorageSession.Query<UserEntity>().WaitForNonStaleResultsAsOfLastWrite().Count();
            actualCount.Should().Be(count);
        }

        [Then(@"существует пользователь ""(.*)"" со списком ролей")]
        public void “о—уществуетѕользователь—о—писком–олей(string username, Table table)
        {
            var expectedRoles = table.Rows.Select(r => r["роль"]).ToList();

            var allUsers = RavenDb.CurrentSession.Query<UserEntity>().WaitForNonStaleResultsAsOfLastWrite().ToList();
            var allRoles = RavenDb.CurrentSession.Query<RoleEntity>().WaitForNonStaleResultsAsOfLastWrite().ToList();

            allUsers.Where(u => u.Username == username).Should().HaveCount(1);

            var user = allUsers.Single(u => u.Username == username);
            user.Roles.Should().HaveCount(table.RowCount);

            foreach (var roleReference in user.Roles)
            {
                var role = allRoles.Single(r => r.Id == roleReference.Id);
                role.RoleName.Should().Be(roleReference.Name);
                expectedRoles.Should().Contain(role.RoleName);
            }
        }
    }
}