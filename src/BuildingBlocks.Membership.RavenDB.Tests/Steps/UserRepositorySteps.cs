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
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession, QueryBuilder);
            UserExistsResult = userRepository.HasUserWithName(userName);
        }

        [When(@"провер€ют что пользователь с email ""(.*)"" существует")]
        public void ≈слиѕровер€ют„тоѕользовательByEmail—уществует(string email)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession, QueryBuilder);
            UserExistsResult = userRepository.HasUserWithEmail(email);
        }

        [When(@"получают список пользователей содержащих имена")]
        public void ≈слиѕолучают—писокѕользователей—одержащих»мена(Table table)
        {
            var namesList = table.Rows.Select(r => r["им€"]).ToArray();
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession, QueryBuilder);
            UsersResult = userRepository.FindUsersByNames(namesList);
        }

        [When(@"ищут пользовател€ с email ""(.*)""")]
        public void ≈сли»щутѕользовател€—Email(string email)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession, QueryBuilder);
            UsersResult = userRepository.FindUserByEmail(email).ToEnumerableOrEmpty();
        }

        [Given(@"пользователь ""(.*)"" имеет Id ""(.*)""")]
        public void ƒопустимѕользователь»меетId(string userName, string id)
        {
            var user = RavenDb.CurrentStorageSession.Query<UserEntity>().Single(u => u.Username == userName);
            user.UserId = new Guid(id);
            RavenDb.CurrentStorageSession.Save(user);
        }

        [When(@"ищут пользовател€ с Id ""(.*)""")]
        public void ≈сли»щутѕользовател€—Id(string id)
        {
            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession, QueryBuilder);
            UsersResult = userRepository.FindUserById(new Guid(id)).ToEnumerableOrEmpty();
        }

        [When(@"загружают (.*) страницу пользоватлей по (.*) пользовател€ с фильтром по email ""(.*)""")]
        public void ≈сли«агружают—траницуѕользоватлейѕоѕользовател€—‘ильтромѕоEmailKreml_Uc(int pageNumber, int pageSize, string emailToMatch)
        {
            Mock.Get(QueryFactory)
                .Setup(f => f.Create<FindByEmailSubstring, Page<User>>())
                .Returns(new UserWithMatchedEmails(RavenDb.CurrentStorageSession));

            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession, QueryBuilder);
            UserPageResult = userRepository.GetUsersPageByEmail(emailToMatch, pageNumber - 1, pageSize);
        }

        [When(@"загружают (.*) страницу пользоватлей по (.*) пользовател€ с фильтром по имени ""(.*)""")]
        public void ≈сли«агружают—траницуѕользоватлейѕоѕользовател€—‘ильтромѕо»мени(int p0, int p1, string p2)
        {
            Mock.Get(QueryFactory)
                .Setup(f => f.Create<FindByUsernameSubstring, Page<User>>())
                .Returns(new UserWithMatchedUsernames(RavenDb.CurrentStorageSession));

            var userRepository = new UserRepositoryImpl(RavenDb.CurrentStorageSession, QueryBuilder);
            UserPageResult = userRepository.GetUsersPageByEmail(emailToMatch, pageNumber - 1, pageSize);
        }

        [Then(@"возвращаетс€ страница пользователей")]
        public void “о¬озвращаетс€—траницаѕользователей(dynamic page)
        {
            UserPageResult.TotalItemCount.Should().Be((int) page.всего);
            UserPageResult.Items.Should().HaveCount((int) page.на—транице);
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