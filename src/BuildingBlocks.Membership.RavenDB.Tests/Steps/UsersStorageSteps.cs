using System;
using BuildingBlocks.Membership.RavenDB.DomainModel;
using BuildingBlocks.Store.RavenDB.TestHelpers.SpecFlow;
using TechTalk.SpecFlow;

namespace BuildingBlocks.Membership.RavenDB.Tests.Steps
{
    [Binding]
    public class UsersStorageSteps
    {
        [Given(@"существуют пользователи")]
        public void ДопустимСуществуютРоли(Table table)
        {
            foreach (var row in table.Rows)
            {
                var username = row["имя"];
                var email = row.ContainsKey("email") ? row["email"] : username + "@mail.com";
                var user = new UserEntity
                    {
                        UserId = Guid.NewGuid(),
                        Username = username,
                        Email = email, 
                        Password = "123"
                    };
                RavenDb.CurrentSession.Store(user);
            }
        }
    }
}