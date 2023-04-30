using CommunityToolkit.Diagnostics;
using Patterns.Account.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Patterns.Account
{
    /// <summary>
    /// The PatternzUserAccount proxy, which makes sure the user has correct permissions to perform operations
    /// </summary>
    public class AccountProxy : IUserAccount
    {
        private readonly IUserAccount _userManager;
        private readonly IPatternzUser _user;

        /// <summary>
        /// Constructor which takes in a user to check permissions on
        /// </summary>
        /// <param name="currentUser">The user that is performing the operations</param>
        public AccountProxy(IPatternzUser currentUser)
        {
            _user = currentUser;
            _userManager = new PatternzUserAccount();
        }

        /// <summary>
        /// Creates a new PatternzUser and adds it to the in memory list. Username
        /// must be unique. The password will be hashed before it is stored on the user record
        /// </summary>
        /// <param name="username">The desired username</param>
        /// <param name="password">The password of the new user</param>
        /// <returns>The newly created PatternzUser</returns>
        public IPatternzUser CreateUser(string username, string password)
        {
            if ((_user.Permissions | Permission.AddUser) == Permission.AddUser)
            {
                return _userManager.CreateUser(username, password);
            }
            return ThrowHelper.ThrowUnauthorizedAccessException<PatternzUser>();
        }

        /// <summary>
        /// Removes a user from the user records
        /// </summary>
        /// <param name="user">The user to remove</param>
        /// <returns>True if a user was removed</returns>
        public bool DeleteUser(IPatternzUser user)
        {
            if ((_user.Permissions | Permission.RemoveUser) == Permission.RemoveUser)
            {
                return _userManager.DeleteUser(user);
            }
            return ThrowHelper.ThrowUnauthorizedAccessException<bool>();
        }

        /// <summary>
        /// Gets a copy of the list of users
        /// </summary>
        /// <returns></returns>
        public ICollection<IPatternzUser> GetUserListCopy()
        {
            return _userManager.GetUserListCopy();
        }

        /// <summary>
        /// Attempts to get a user from the user list
        /// </summary>
        /// <param name="username">The username to search on</param>
        /// <param name="foundUser">The found PatternzUser. Can be null if not found</param>
        /// <returns>True if the user was found</returns>
        public bool TryGetUser(string username, out IPatternzUser? foundUser)
        {
            return _userManager.TryGetUser(username, out foundUser);
        }

        /// <summary>
        /// Attempts to load all users in a store into memory
        /// </summary>
        /// <param name="pathToStore">Optional. Path to the store</param>
        /// <returns>True if users were found</returns>
        public Task<bool> TryReadUsersFromStoreAsync(string? pathToStore = null)
        {
            return _userManager.TryReadUsersFromStoreAsync(pathToStore);
        }

        /// <summary>
        /// Attempts to write all users into a store from memory
        /// </summary>
        /// <param name="pathToStore">Optional. Path to the store</param>
        /// <returns>True if the write operation succeeds</returns>
        public Task<bool> TryWriteUsersToStoreAsync(string? pathToStore = null)
        {
            return _userManager.TryWriteUsersToStoreAsync(pathToStore);
        }

        /// <summary>
        /// Updates an existing PatternzUser. Password information must match
        /// for the operation to succeed. If a new password is provided, the password will be hashed
        /// before it is attached to the updated record.
        /// </summary>
        /// <param name="user">The mutated user record to update. PasswordHash on this object must match
        /// PasswordHash in the records</param>
        /// <param name="newPassword">Optional. Use this parameter if you desire to change the user's password</param>
        /// <returns>The updated PatternzUser</returns>
        public IPatternzUser UpdateUser(IPatternzUser user, string? newPassword)
        {
            if ((_user.Permissions & Permission.UpdateUser) == Permission.UpdateUser)
            {
                return _userManager.UpdateUser(user, newPassword);
            }
            return ThrowHelper.ThrowUnauthorizedAccessException<IPatternzUser>();
        }
    }
}
