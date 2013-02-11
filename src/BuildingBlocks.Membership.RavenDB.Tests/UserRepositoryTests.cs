using System;
using System.Collections.Generic;
using System.Linq;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Membership.RavenDB.Tests.Steps;
using BuildingBlocks.Store;
using BuildingBlocks.Store.RavenDB;
using FluentAssertions;
using NUnit.Framework;
using Raven.Client;
using Raven.Client.Embedded;

namespace BuildingBlocks.Membership.RavenDB.Tests
{


    [TestFixture]
    public class UserRepositoryTests : RavenDBTestBase
    {
        private UserEntity[] _users;

        [SetUp]
        public void SetUp()
        {
            _users = new[]
                {
                    new UserEntity {Username = "Sidorov", Email = "Sidorov@outlook.com"},
                    new UserEntity {Username = "Pertov",  Email = "Pertov@mail.ru"},
                    new UserEntity {Username = "Ivanov",  Email = "Ivanov@gmail.com"}
                };
            TestDataGenerator.CreateUsers(DocumentStore, _users);
        }

        [TestCase("Pertov", true)]
        [TestCase("Medvedev", false)]
        public void HasUserWithName_should_indicate_that_user_with_name_is_exists(string username, bool expectedResult)
        {
            var userRepository = new UserRepositoryImpl(Storage);

            var actual = userRepository.HasUserWithName(MembershipSettings.DefaultApplicationName, username);

            actual.Should().Be(expectedResult);
        }

        [TestCase("Pertov@mail.ru", true)]
        [TestCase("Medvedev@mail.ru", false)]
        public void HasUserWithEmail_should_indicate_that_user_with_email_is_exists(string username, bool expectedResult)
        {
            var userRepository = new UserRepositoryImpl(Storage);

            var actual = userRepository.HasUserWithEmail(MembershipSettings.DefaultApplicationName, username);

            actual.Should().Be(expectedResult);
        }

        [Test]
        public void FindUsersByNames_should_return_all_exists_users_witn_names_from_list()
        {
            var userRepository = new UserRepositoryImpl(Storage);

            var users = userRepository.FindUsersByNames(MembershipSettings.DefaultApplicationName, "Putin", "Ivanov", "Pertov");

            users.Should().HaveCount(2);
            users.Select(u => u.Username).Should().Contain("Ivanov", "Pertov");
            users.Select(u => u.Username).Should().NotContain("Putin");
        }

        [Test]
        public void FindUserByEmail_should_return_user_by_email()
        {
            var userRepository = new UserRepositoryImpl(Storage);
            var user = userRepository.FindUserByEmail(MembershipSettings.DefaultApplicationName, "Sidorov@outlook.com");
            user.Should().NotBeNull();
        }

        [Test]
        public void FindUserByEmail_should_return_null_when_user_with_email_is_not_exists()
        {
            var userRepository = new UserRepositoryImpl(Storage);
            var user = userRepository.FindUserByEmail(MembershipSettings.DefaultApplicationName, "putin@outlook.com");
            user.Should().BeNull();
        }

        [Test]
        public void FindUserById_should_return_user_by_id()
        {
            var userRepository = new UserRepositoryImpl(Storage);
            var user = userRepository.FindUserById(_users.First().UserId);
            user.Should().NotBeNull();
        }

        [Test]
        public void FindUserById_should_return_null_when_user_with_id_is_not_exists()
        {
            var userRepository = new UserRepositoryImpl(Storage);
            var user = userRepository.FindUserById(Guid.NewGuid());
            user.Should().BeNull();
        }

        [Test]
        public void FindUsersInRole_should_return_users_by_role_name()
        {
            TestDataGenerator.CreateRoles(DocumentStore, "Doctor", "Assistant", "Admin");
            var usersWithRoles = new[]
                {
                    new UserEntity {Username = "Medvedev",  Roles = new []{"Admin"              }},
                    new UserEntity {Username = "Medvedeva", Roles = new []{"Assistant"          }},
                    new UserEntity {Username = "Putin",     Roles = new []{"Doctor","Admin"     }},
                    new UserEntity {Username = "Kabaeva",   Roles = new []{"Doctor","Assistant" }},
                    new UserEntity {Username = "Chu",       Roles = new []{"Assistant"          }}
                };
            TestDataGenerator.CreateUsers(DocumentStore, usersWithRoles);
            var expectedUsers = new[] {"Medvedev", "Putin"};

            var userRepository = new UserRepositoryImpl(Storage);

            var actualUsers = userRepository.FindUsersInRole(MembershipSettings.DefaultApplicationName, "Admin");

            actualUsers.Should().HaveCount(2);
            actualUsers.Select(u => u.Username).Should().Contain(expectedUsers);
        }

        [Test]
        public void FindUsersInRole_should_return_users_by_part_of_username_and_role_name()
        {
            TestDataGenerator.CreateRoles(DocumentStore, "Doctor", "Assistant", "Admin");
            var usersWithRoles = new[]
                {
                    new UserEntity {Username = "Medvedev",  Roles = new []{"Admin"              }},
                    new UserEntity {Username = "Medvedeva", Roles = new []{"Assistant","Admin"  }},
                    new UserEntity {Username = "Putin",     Roles = new []{"Doctor","Admin"     }},
                    new UserEntity {Username = "Kabaeva",   Roles = new []{"Doctor","Assistant" }},
                    new UserEntity {Username = "Chu",       Roles = new []{"Assistant"          }}
                };
            TestDataGenerator.CreateUsers(DocumentStore, usersWithRoles);
            var expectedUsers = new[] { "Medvedev", "Medvedeva" };

            var userRepository = new UserRepositoryImpl(Storage);

            var actualUsers = userRepository.FindUsersInRole(MembershipSettings.DefaultApplicationName, "Admin", "Medvede");

            actualUsers.Should().HaveCount(2);
            actualUsers.Select(u => u.Username).Should().Contain(expectedUsers);
        }

        [TestCase("kreml.uc", "Ru",     4, 3)]
        [TestCase("",         "Ru",     5, 3)]
        [TestCase("",         "Yandex", 1, 1)]
        [TestCase("kreml",    "Yandex", 1, 1)]
        [TestCase("kreml.uc", "Yandex", 0, 0)]
        public void FindUsersInRole_should_return_users_by_part_of_username_and_role_name(
            string emailToMatch, 
            string applicationName, 
            int expectedTotoal, 
            int expectedItemsPerPage)
        {
            TestDataGenerator.CreateRoles(DocumentStore, "Doctor", "Assistant", "Admin");
            var usersWithRoles = new[]
                {
                    new UserEntity {Username = "Medvedev" , Email = "Sidorov@kreml.uc",   ApplicationName = "Ru"    },
                    new UserEntity {Username = "Medvedeva", Email = "Petrov@kreml.uc",    ApplicationName = "Ru"    },
                    new UserEntity {Username = "Putin"    , Email = "Putin@kreml.uc",     ApplicationName = "Ru"    },
                    new UserEntity {Username = "Kabaeva"  , Email = "Kabaeva@kreml.uc",   ApplicationName = "Ru"    },
                    new UserEntity {Username = "Kabaev"   , Email = "Kabaeva@yanndex.uc", ApplicationName = "Ru"    },
                    new UserEntity {Username = "Chu"      , Email = "Chu@kreml.ru",       ApplicationName = "Yandex"}
                };
            TestDataGenerator.CreateUsers(DocumentStore, usersWithRoles);

            var userRepository = new UserRepositoryImpl(Storage);

            var actualUsers = userRepository.GetUsersPageByEmail(emailToMatch, applicationName, 0, 3);

            actualUsers.TotalItemCount.Should().Be(expectedTotoal);
            actualUsers.Items.Should().HaveCount(expectedItemsPerPage);
        }
    }

    public static class TestDataGenerator
    {
        public static void CreateUsers(IDocumentStore storage, params UserEntity[] users)
        {
            using (var session = storage.OpenSession())
            {
                foreach (var user in users)
                {
                    if (user.Email == null)
                    {
                        user.Email = user.Username + "@mail.com";
                    }

                    if (user.ApplicationName == null)
                    {
                        user.ApplicationName = MembershipSettings.DefaultApplicationName;
                    }
                    if (user.UserId == Guid.Empty)
                    {
                        user.UserId = Guid.NewGuid();
                    }
                    if (user.Password == null)
                    {
                        user.Password = "123";
                    }
                    
                    session.Store(user);
                }

                session.SaveChanges();
            }
        }

        public static void CreateRoles(IDocumentStore documentStore, params string[] roles)
        {
            using (var session = documentStore.OpenSession())
            {
                foreach (var roleName in roles)
                {
                    var role = new RoleEntity
                        {
                            ApplicationName = MembershipSettings.DefaultApplicationName,
                            RoleId = Guid.NewGuid(),
                            RoleName = roleName,
                            Description = roleName
                        };
                    session.Store(role);
                }
                session.SaveChanges();
            }
        }
    }

    public class RavenDBTestBase
    {
        private EmbeddableDocumentStore _store;

        [SetUp]
        public void SetUp()
        {
            Initialize();
        }

        public void Initialize()
        {
            var store = new EmbeddableDocumentStore
            {
                RunInMemory = true
            };
            store.Initialize();

            _store = store;
        }

        public IDocumentStore DocumentStore
        {
            get
            {
                CheckInitialized();
                return _store;
            }
        }

        public IStorage Storage
        {
            get
            {
                CheckInitialized();
                return new RavenDbStorage(DocumentStore);
            }
        }

        private void CheckInitialized()
        {
            if (_store == null)
            {
                throw new InvalidOperationException("Bedore use storage need run Initialize() method");
            }
        }
    }
}
