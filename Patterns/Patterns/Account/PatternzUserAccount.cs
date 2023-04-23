using CommunityToolkit.Diagnostics;
using Patterns.Security;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Patterns.Account
{
    /// <summary>
    /// User manager for PatternzUser
    /// </summary>
    public class PatternzUserAccount : IUserAccount
    {
        private const string PathToStore = @".\data\";
        private const string StoreName = @"users.dat";
        private const char RecordDelimiter = ';';
        private const char FieldDelimiter = ',';

        private string _storeFullName;
        private Dictionary<string, IPatternzUser> _users = new();

        /// <summary>
        /// Creates a new instance of PatternzUserAccount
        /// </summary>
        /// <param name="pathToStore">Optional. Path to user store for read/write operations</param>
        public PatternzUserAccount(string? pathToStore = null) 
        {
            _storeFullName = pathToStore ?? PathToStore + StoreName;
        }

        /// <summary>
        /// Attempts to get a user from the user list
        /// </summary>
        /// <param name="username">The username to search on</param>
        /// <param name="foundUser">The found PatternzUser. Can be null if not found</param>
        /// <returns>True if the user was found</returns>
        public bool TryGetUser(string username, out IPatternzUser? foundUser)
        {
            Guard.IsNotNull(username, $"{nameof(username)} cannot be null");
            Guard.IsNotNull(_users, $"Users must be loaded from store before {nameof(TryGetUser)} can be called.");

            return _users.TryGetValue(username, out foundUser);
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
        public PatternzUser UpdateUser(PatternzUser user, string? newPassword = null)
        {
            Guard.IsNotNull(user, $"{nameof(user)} cannot be null");

            IPatternzUser? foundUser = null;
            if (!_users.TryGetValue(user.Username, out foundUser))
            {
                ThrowHelper.ThrowInvalidOperationException($"User {user.Username} not found");
            }
            if (user.PasswordHash != foundUser.PasswordHash)
            {
                ThrowHelper.ThrowInvalidOperationException($"Password not valid for user");
            }

            PatternzUser result = user;
            if (newPassword != null)
            {
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    ThrowHelper.ThrowArgumentException("New password cannot be blank");
                }

                Pbkdf2DataHasher hasher = new();
                string hashed = hasher.HashString(newPassword);

                result.PasswordHash = hashed;
            }
            
            return result;
        }

        /// <summary>
        /// Removes a user from the user records
        /// </summary>
        /// <param name="user">The user to remove</param>
        /// <returns>True if a user was removed</returns>
        public bool DeleteUser(PatternzUser user)
        {
            Guard.IsNotNull(user, $"{nameof(user)} cannot be null");

            if (_users.ContainsKey(user.Username))
            {
                _users.Remove(user.Username);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Creates a new PatternzUser and adds it to the in memory list. Username
        /// must be unique. The password will be hashed before it is stored on the user record
        /// </summary>
        /// <param name="username">The desired username</param>
        /// <param name="password">The password of the new user</param>
        /// <returns>The newly created PatternzUser</returns>
        public PatternzUser CreateUser(string username, string password)
        {
            Guard.IsNotNullOrWhiteSpace(username, nameof(username));
            Guard.IsNotNullOrWhiteSpace(password, nameof(password));

            if (_users.ContainsKey(username))
            {
                ThrowHelper.ThrowInvalidOperationException("Username already exists");
            }
            Pbkdf2DataHasher hasher = new();

            string hashed = hasher.HashString(password);
            PatternzUser result = new(username, hashed);
            _users.Add(username, result);
            return result;
        }

        /// <summary>
        /// Attempts to load all users in a store into memory
        /// </summary>
        /// <param name="pathToStore">Optional. Path to the store</param>
        /// <returns>True if users were found</returns>
        public async Task<bool> TryReadUsersFromStoreAsync(string? pathToStore = null)
        {
            bool result = false;
            pathToStore ??= PathToStore + StoreName;

            try
            {
                if (File.Exists(pathToStore))
                {
                    string contents = await File.ReadAllTextAsync(pathToStore);
                    PatternzUser[]? users = JsonSerializer.Deserialize<PatternzUser[]>(contents);
                    if (users != null)
                    {
                        _users = new();
                        foreach (IPatternzUser user in users) 
                        {
                            _users.Add(user.Username, user);
                        }
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"An exception occurred while trying to read the user store: {ex}");
            }

            return result;
        }

        /// <summary>
        /// Attempts to write all users into a store from memory
        /// </summary>
        /// <param name="pathToStore">Optional. Path to the store</param>
        /// <returns>True if the write operation succeeds</returns>
        public async Task<bool> TryWriteUsersToStoreAsync(string? pathToStore = null)
        {
            pathToStore ??= PathToStore + StoreName;

            try
            {
                CreateDirectoryIfNotExists(pathToStore);
                List<IPatternzUser> userList = new (_users.Count);
                foreach(string username in _users.Keys)
                {
                    userList.Add(_users[username]);
                }
                string jsonContents = JsonSerializer.Serialize(userList.ToArray());
                await File.WriteAllTextAsync(pathToStore, jsonContents);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing user store {ex}");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Creates a directory for the path to the store if it doesn't already exist
        /// </summary>
        /// <param name="pathToStore"></param>
        private void CreateDirectoryIfNotExists(string pathToStore) 
        {
            string? path = Path.GetDirectoryName(pathToStore);
            if (path != null && !Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }
}
