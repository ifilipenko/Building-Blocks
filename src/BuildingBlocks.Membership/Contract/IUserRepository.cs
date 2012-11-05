using System;
using System.Collections.Generic;
using BuildingBlocks.Common;
using BuildingBlocks.Membership.Entities;

namespace BuildingBlocks.Membership.Contract
{
    public interface IUserRepository
    {
        bool HasUserWithName(string applicationName, string username);
        bool HasUserWithEmail(string applicationName, string email);

        IEnumerable<User> FindUsersByNames(string applicationName, params string[] usernames);
        User FindUserByEmail(string applicationName, string email);
        User FindUserById(Guid userId);
        IEnumerable<User> FindUsersInRole(string applicationName, string roleName);
        IEnumerable<User> FindUsersInRole(string applicationName, string roleName, string usernameToMatch);

        Page<User> GetUsersPageByEmail(string applicationName, string emailToMatch, int pageIndex, int pageSize);
        Page<User> GetUsersPageByUsername(string applicationName, string usernameToMatch, int pageIndex, int pageSize);
        Page<User> GetUsersPage(string applicationName, int pageIndex, int pageSize);

        int GetUsersCountWithLastActivityDateGreaterThen(string applicationName, DateTime dateActive);

        void AddUser(User newUser);
        void SaveUser(User user);
        void DeleteUser(User user);
    }
}