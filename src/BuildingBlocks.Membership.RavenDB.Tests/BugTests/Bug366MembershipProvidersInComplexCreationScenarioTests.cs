using System.Linq;
using System.Web.Security;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store;
using BuildingBlocks.Store.RavenDB;
using BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BuildingBlocks.Membership.RavenDB.Tests.BugTests
{
    /// <summary>
    /// This fixture detect problem with ContainsIn implementation and stale results
    /// </summary>
    [TestFixture]
    public class Bug366MembershipProvidersInComplexCreationScenarioTests
    {
        private RoleProvider _roleProvider;
        private MembershipProvider _membershipProvider;
        private IStorageSession _outsideSession;
        private bool _enableOutsideSession;

        [SetUp]
        public void Setup()
        {
            RavenDb.InitializeStorage();

            ((RavenDbStorage) RavenDb.Storage).SessionSettings.SetStaleResultsWhait(StaleResultWhaitMode.AllNonStale);

            var repositoryFactory = Substitute.For<IRepositoryFactory>();
            repositoryFactory.CreateUserRepository().Returns(_ => _enableOutsideSession
                                                                      ? new UserRepositoryImpl(_outsideSession)
                                                                      : new UserRepositoryImpl(RavenDb.Storage));
            repositoryFactory.CreateRoleRepository().Returns(_ => _enableOutsideSession
                                                                      ? new RoleRepositoryImpl(_outsideSession)
                                                                      : new RoleRepositoryImpl(RavenDb.Storage));
            
            RepositoryFactory.Initialize(repositoryFactory);

            _roleProvider = new RoleProvider();
            _membershipProvider = new MembershipProvider();
        }

        [Test]
        public void Should_Executed_Correctly()
        {
            ObtainRequiredRoles("role1", "role2", "role3");
            var status = ObtainUser("user", "role3");
            status.Should().Be(MembershipCreateStatus.Success);
        }

        [Test]
        public void Should_Ignore_When_Invoke_inSecond_time()
        {
            ObtainRequiredRoles("role1", "role2", "role3");
            ObtainUser("user", "role3");

            ObtainRequiredRoles("role1", "role2", "role3");
            var status = ObtainUser("user", "role3");
            status.Should().BeNull();
        }

        [Test]
        public void Should_Executed_Correctly_With_Outside_session()
        {
            _enableOutsideSession = true;
            using (_outsideSession = RavenDb.Storage.OpenSesion())
            {
                ObtainRequiredRoles("role1", "role2", "role3");
                _outsideSession.SumbitChanges();
            }

            using (_outsideSession = RavenDb.Storage.OpenSesion())
            {
                var status = ObtainUser("user", "role3");
                _outsideSession.SumbitChanges();
                status.Should().Be(MembershipCreateStatus.Success);
            }
        }

        [Test]
        public void Should_Ignore_When_Invoke_inSecond_time_With_Outside_session()
        {
            _enableOutsideSession = true;

            using (_outsideSession = RavenDb.Storage.OpenSesion())
            {
                ObtainRequiredRoles("role1", "role2", "role3");
                _outsideSession.SumbitChanges();

                var status = ObtainUser("user");
                status.Should().Be(MembershipCreateStatus.Success);
                _outsideSession.SumbitChanges();

                ObtainUserInRole("user", "role3");
                _outsideSession.SumbitChanges();
            }

            using (var session = RavenDb.Storage.OpenSesion())
            {
                session.Query<RoleEntity>().Should().HaveCount(3);
                session.Query<RoleEntity>().Select(r => r.RoleName).Should().Contain("role1", "role2", "role3");

                session.Query<UserEntity>().Should().HaveCount(1);
                session.Query<UserEntity>().Select(u => u.Username).Should().OnlyContain(u => u == "user");
                session.Query<UserEntity>().Single().Roles.Should().OnlyContain(r => r == "role3");
            }
            
            using (_outsideSession = RavenDb.Storage.OpenSesion())
            {
                ObtainRequiredRoles("role1", "role2", "role3");
                _outsideSession.SumbitChanges();
                
                var status = ObtainUser("user", "role3");
                status.Should().BeNull();
            }
        }

        private MembershipCreateStatus? ObtainUser(string username)
        {
            MembershipCreateStatus status;
            _membershipProvider.CreateUser(username,
                                            "234234234",
                                            username + "@" + username + ".com",
                                            null,
                                            null,
                                            true,
                                            null,
                                            out status);
            return status;
        }

        private MembershipCreateStatus? ObtainUser(string username, string roleName)
        {
            if (_membershipProvider.GetUser(username, false) != null)
            {
                if (!_roleProvider.IsUserInRole(username, roleName))
                {
                    _roleProvider.AddUsersToRoles(new[] {username}, new[] {roleName});
                }
                return null;
            }

            var status = ObtainUser(username);
            _roleProvider.AddUsersToRoles(new[] {username}, new[] {roleName});
            return status;
        }

        private void ObtainUserInRole(string username, string roleName)
        {
            if (_membershipProvider.GetUser(username, false) != null)
            {
                if (!_roleProvider.IsUserInRole(username, roleName))
                {
                    _roleProvider.AddUsersToRoles(new[] { username }, new[] { roleName });
                }
            }
        }

        private void ObtainRequiredRoles(params string[] roles)
        {
            foreach (var rolename in roles)
            {
                if (_roleProvider.RoleExists(rolename))
                    continue;
                _roleProvider.CreateRole(rolename);
            }
        }
    }
}