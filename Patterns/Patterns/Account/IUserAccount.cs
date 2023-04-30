using System.Collections.Generic;
using System.Threading.Tasks;
using Patterns.Account.Model;

namespace Patterns.Account
{
    /// <summary>
    /// Interface for managing Patternz Users
    /// </summary>
    public interface IUserAccount
    {
        /// <summary>
        /// Attempts to load all users in a store into memory
        /// </summary>
        /// <param name="pathToStore">Optional. Path to the store</param>
        /// <returns>True if users were found</returns>
        Task<bool> TryReadUsersFromStoreAsync(string? pathToStore = null);

        /// <summary>
        /// Attempts to write all users into a store from memory
        /// </summary>
        /// <param name="pathToStore">Optional. Path to the store</param>
        /// <returns>True if the write operation succeeds</returns>
        Task<bool> TryWriteUsersToStoreAsync(string? pathToStore = null);

        /// <summary>
        /// Creates a new PatternzUser and adds it to the in memory list. Username
        /// must be unique.
        /// </summary>
        /// <param name="username">The desired username</param>
        /// <param name="password">The password of the new user</param>
        /// <returns>The newly created PatternzUser</returns>
        IPatternzUser CreateUser(string username, string password);

        /// <summary>
        /// Updates an existing PatternzUser. Password information must match
        /// for the operation to succeed.
        /// </summary>
        /// <param name="user">The mutated user record to update. PasswordHash on this object must match
        /// PasswordHash in the records</param>
        /// <param name="newPassword">Optional. Use this parameter if you desire to change the user's password</param>
        /// <returns>The updated PatternzUser</returns>
        IPatternzUser UpdateUser(IPatternzUser user, string? newPassword);

        /// <summary>
        /// Removes a user from the user records
        /// </summary>
        /// <param name="user">The user to remove</param>
        /// <returns>True if a user was removed</returns>
        bool DeleteUser(IPatternzUser user);

        /// <summary>
        /// Attempts to find a user from the user records
        /// </summary>
        /// <param name="username">The username to search on</param>
        /// <param name="foundUser">The found user record. May be null if the user can't be found</param>
        /// <returns>True if the user was found</returns>
        bool TryGetUser(string username, out IPatternzUser? foundUser);

        /// <summary>
        /// Returns a copy of the list of users.
        /// </summary>
        /// <returns></returns>
        ICollection<IPatternzUser> GetUserListCopy();
    }
}
