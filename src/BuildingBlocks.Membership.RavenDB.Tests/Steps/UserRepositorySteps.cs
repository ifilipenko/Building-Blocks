using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.Entities;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Membership.RavenDB.Queries;
using BuildingBlocks.Membership.RavenDB.Queries.Criteria;
using BuildingBlocks.Query;
using BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace BuildingBlocks.Membership.RavenDB.Tests.Steps
{
    [Binding]
    public class UserRepositorySteps
    {
        private bool UserExistsResult
        {
            get { return ScenarioContext.Current.Get<bool>("UserExistsResult"); }
            set { ScenarioContext.Current.Set(value, "UserExistsResult"); }
        }

        private IEnumerable<User> UsersResult
        {
            get { return ScenarioContext.Current.Get<IEnumerable<User>>("UsersResult"); }
            set { ScenarioContext.Current.Set(value, "UsersResult"); }
        }

        private Page<User> UserPageResult
        {
            get { return ScenarioContext.Current.Get<Page<User>>("UserPageResult"); }
            set { ScenarioContext.Current.Set(value, "UserPageResult"); }
        }

        private int UsersCountResult
        {
            get { return ScenarioContext.Current.Get<int>("UsersCountResult"); }
            set { ScenarioContext.Current.Set(value, "UsersCountResult"); }
        }

        private IQueryBuilder QueryBuilder
        {
            get { return ScenarioContext.Current.Obtain(() => new QueryBuilder(QueryFactory), "QueryBuilder"); }
        }

        private IQueryFactory QueryFactory
        {
            get { return ScenarioContext.Current.Obtain(() => new Mock<IQueryFactory>().Object, "QueryFactory"); }
        }

        [When(@"провер€ют что пользователь с именем ""(.*)"" существует")]
        public void ≈слиѕровер€ют„тоѕользователь—уществует(string userName)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UserExistsResult = userRepository.HasUserWithName(MembershipSettings.ApplicationName, userName);
        }

        [When(@"провер€ют что пользователь с email ""(.*)"" существует")]
        public void ≈слиѕровер€ют„тоѕользовательByEmail—уществует(string email)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UserExistsResult = userRepository.HasUserWithEmail(MembershipSettings.ApplicationName, email);
        }

        [When(@"получают список пользователей содержащих имена")]
        public void ≈слиѕолучают—писокѕользователей—одержащих»мена(Table table)
        {
            var namesList = table.Rows.Select(r => r["им€"]).ToArray();
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UsersResult = userRepository.FindUsersByNames(MembershipSettings.ApplicationName, namesList);
        }

        [When(@"ищут пользовател€ с email ""(.*)""")]
        public void ≈сли»щутѕользовател€—Email(string email)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UsersResult = userRepository.FindUserByEmail(MembershipSettings.ApplicationName, email).ToEnumerableOrEmpty();
        }

        [When(@"ищут пользовател€ с Id ""(.*)""")]
        public void ≈сли»щутѕользовател€—Id(string id)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UsersResult = userRepository.FindUserById(new Guid(id)).ToEnumerableOrEmpty();
        }

        [When(@"загружают (.*) страницу пользоватлей по (.*) пользовател€ с фильтром по email ""(.*)""")]
        public void ≈сли«агружают—траницуѕользоватлейѕоѕользовател€—‘ильтромѕоEmail(int pageNumber, int pageSize, string emailToMatch)
        {
            Mock.Get(QueryFactory)
                .Setup(f => f.Create<FindByEmailSubstring, Page<User>>())
                .Returns(new UsersColumnMatchedToSubstring(RavenDb.CurrentStorageSession));

            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UserPageResult = userRepository.GetUsersPageByEmail(MembershipSettings.ApplicationName, emailToMatch, pageNumber - 1, pageSize);
        }

        [When(@"загружают (.*) страницу пользоватлей по (.*) пользовател€ с фильтром по имени ""(.*)""")]
        public void ≈сли«агружают—траницуѕользоватлейѕоѕользовател€—‘ильтромѕо»мени(int pageNumber, int pageSize, string usernameToMatch)
        {
            Mock.Get(QueryFactory)
                .Setup(f => f.Create<FindByUsernameSubstring, Page<User>>())
                .Returns(new UsersColumnMatchedToSubstring(RavenDb.CurrentStorageSession));

            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UserPageResult = userRepository.GetUsersPageByUsername(MembershipSettings.ApplicationName, usernameToMatch, pageNumber - 1, pageSize);
        }

        [When(@"загружают (.*) страницу пользоватлей по (.*) пользовател€")]
        public void ≈сли«агружают—траницуѕользоватлейѕоѕользовател€(int pageNumber, int pageSize)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UserPageResult = userRepository.GetUsersPage(MembershipSettings.ApplicationName, pageNumber - 1, pageSize);
        }

        [When(@"создают нового пользовател€ ""(.*)""")]
        public void ≈сли—оздаютЌовогоѕользовател€(string username)
        {
            var user = new User(Guid.NewGuid(), username, username + "@mail.ru", MembershipSettings.ApplicationName)
            {
                Password = "123"
            };
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            userRepository.AddUser(user);
        }

        [When(@"создают нового пользовател€ ""(.*)"" с назначенными рол€ми")]
        public void ≈сли—оздаютЌовогоѕользовател€—Ќазначенными–ол€ми(string username, Table table)
        {
            var user = new User(Guid.NewGuid(), username, username + "@mail.ru", MembershipSettings.ApplicationName)
            {
                Password = "123"
            };

            foreach (var row in table.CreateDynamicSet())
            {
                string roleName = row.роль;
                user.AddRole(roleName);
            }

            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            userRepository.AddUser(user);
        }

        [When(@"получают количество пользователей с последней активностью от (.*)")]
        public void ≈слиѕолучают оличествоѕользователей—ѕоследнейјктивностьюќт(DateTime dateTime)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            UsersCountResult = userRepository.GetUsersCountWithLastActivityDateGreaterThen(MembershipSettings.ApplicationName, dateTime);
        }

        [When(@"дл€ пользовател€ с Id ""(.*)"" обновл€ют пол€")]
        public void ≈слиƒл€ѕользовател€—Idќбновл€ютѕол€(Guid id, Table table)
        {
            var user = RavenDb.CurrentStorageSession.Query<UserEntity>().Single(u => u.UserId == id).ToUser();

            dynamic data = table.CreateDynamicInstance();
            user.Password                                = data.ѕароль;
            user.Username                                = data.»м€;
            user.ApplicationName                         = data.ApplicationName;
            user.Email                                   = data.Email;
            user.Comment                                 = data. омментарий;
            user.ConfirmationToken                       = data.ConfirmationToken;
            user.CreateDate                              = data.CreateDate;
            user.IsApproved                              = data.IsApproved;
            user.IsLockedOut                             = data.IsLockedOut;
            user.LastActivityDate                        = data.LastActivityDate;
            user.LastLockoutDate                         = data.LastLockoutDate;
            user.LastLoginDate                           = data.LastLoginDate;
            user.LastPasswordChangedDate                 = data.LastPasswordChangedDate;
            user.LastPasswordFailureDate                 = data.LastPasswordFailureDate;
            user.PasswordFailuresSinceLastSuccess        = data.PasswordFailuresSinceLastSuccess;
            user.PasswordVerificationToken               = data.PasswordVerificationToken;
            user.PasswordVerificationTokenExpirationDate = data.PasswordVerificationTokenExpirationDate;

            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            userRepository.SaveUser(user);
        }

        [When(@"пользователю ""(.*)"" мен€ют назначение ролей")]
        public void ≈слиѕользователюћен€ютЌазначение–олей(string userName, Table table)
        {
            var user = RavenDb.CurrentStorageSession.Query<UserEntity>().Single(u => u.Username == userName).ToUser();

            foreach (var role in user.Roles.ToList())
            {
                user.RemoveRole(role);
            }

            foreach (var row in table.CreateDynamicSet())
            {
                string roleName = row.роль;
                user.AddRole(roleName);
            }
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            userRepository.SaveUser(user);
        }

        [When(@"удал€ют пользовател€ ""(.*)""")]
        public void ≈сли”дал€ютѕользовател€(string userName)
        {
            var user = RavenDb.CurrentStorageSession.Query<UserEntity>().Single(u => u.Username == userName).ToUser();

            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession);
            userRepository.DeleteUser(user);
        }

        [Then(@"количество пользователей равно (.*)")]
        public void “о оличествоѕользователей–авно(int count)
        {
            UsersCountResult.Should().Be(count);
        }

        [Then(@"возвращаетс€ страница пользователей")]
        public void “о¬озвращаетс€—траницаѕользователей(dynamic page)
        {
            UserPageResult.TotalItemCount.Should().Be((int) page.всего);
            UserPageResult.Items.Should().HaveCount((int) page.на—транице);
        }

        [Then(@"возвращаетс€ страница с пользовател€ми")]
        public void “о¬озвращаетс€—траница—ѕользовател€ми(Table table)
        {
            UserPageResult.Items.Should().HaveCount(table.RowCount);
            for (int i = 0; i < table.RowCount; i++)
            {
                var username = table.Rows[i]["им€"];
                var actualUser = UserPageResult.Items.ElementAt(i);
                actualUser.Username.Should().Be(username);
            }
        }

        [Then(@"не возвращаетс€ ни одного пользовател€")]
        public void “оЌе¬озвращаетс€Ќиќдногоѕользовател€()
        {
            UsersResult.Should().BeEmpty();
        }

        [Then(@"результат проверки признает что пользователь ""(.*)""")]
        public void “о–езультатѕроверкиѕризнает„тоѕользователь(bool isExists)
        {
            UserExistsResult.Should().Be(isExists);
        }

        [Then(@"возвращаетс€ следующий список пользователей")]
        public void “о¬озвращаетс€—ледующий—писокѕользователей(Table table)
        {
            UsersResult.Should().HaveSameCount(table.Rows);
            for (var i = 0; i < table.RowCount; i++)
            {
                var expectedUserName = table.Rows[i]["им€"];
                var user = UsersResult.ElementAt(i);
                user.Username.Should().Be(expectedUserName);
            }
        }
    }
}