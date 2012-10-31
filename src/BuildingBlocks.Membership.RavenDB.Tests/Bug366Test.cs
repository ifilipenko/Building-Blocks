using System.Web.Security;
using BuildingBlocks.Membership.Contract;
using BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BuildingBlocks.Membership.RavenDB.Tests
{
    [TestFixture]
    public class Bug366MembershipProvidersInComplexCreationScenarioTest
    {
        private RoleProvider _roleProvider;
        private MembershipProvider _membershipProvider;

        [SetUp]
        public void Setup()
        {
            RavenDb.InitializeStorage();

            var userRepository = new UserRepositoryImpl(RavenDb.Storage);
            var rolesRepository = new RoleRepositoryImpl(RavenDb.Storage);

            var repositoryFactory = Substitute.For<IRepositoryFactory>();
            repositoryFactory.CreateUserRepository().Returns(userRepository);
            repositoryFactory.CreateRoleRepository().Returns(rolesRepository);
            
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

        private MembershipCreateStatus? ObtainUser(string username, string roleName)
        {
            if (_membershipProvider.GetUser(username, false) != null)
            {
                if (_roleProvider.IsUserInRole(username, roleName))
                    return null;
                _roleProvider.AddUsersToRoles(new[] {username}, new[] {roleName});
            }

            MembershipCreateStatus status;
            _membershipProvider.CreateUser(username,
                                            "234234234",
                                            username + "@" + username + ".com",
                                            null, 
                                            null, 
                                            true, 
                                            null, 
                                            out status);
            _roleProvider.AddUsersToRoles(new[] {username}, new[] {roleName});
            return status;
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