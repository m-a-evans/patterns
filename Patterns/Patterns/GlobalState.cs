using Patterns.Account;
using Patterns.Proxy;
using Patterns.Security;
using System;
using System.Threading.Tasks;

namespace Patterns
{
    /// <summary>
    /// Contains fields that are globally accessible to the application
    /// </summary>
    public class GlobalState : IGlobalState
    {
        private IUserAccount _userManager = new AccountProxy(PatternzUser.AnyUser);

        private IPatternzUser _currentUser;

        private Task _loadUsersFromStore;

        /// <summary>
        /// Gets the current user. Will be AnyUser if no one is logged in
        /// </summary>
        public IPatternzUser CurrentUser
        {
            get
            {
                return _currentUser;
            }
            private set
            {
                _currentUser = value;
            }
        }

        /// <summary>
        /// Constructor, sets current user to AnyUser
        /// </summary>
        public GlobalState()
        {
            _currentUser = PatternzUser.AnyUser;
            _loadUsersFromStore = _userManager.TryReadUsersFromStoreAsync();
        }

        /// <summary>
        /// True if a user other than AnyUser is logged in
        /// </summary>
        public bool IsLoggedIn { get { return !CurrentUser.IsAnyUser; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool PerformLogin(string username, string password)
        { 
            if (!_loadUsersFromStore.IsCompleted)
                _loadUsersFromStore.Wait();
            IPatternzUser? user;
            if (_userManager.TryGetUser(username, out user))
            {
                Pbkdf2DataHasher hasher = new();

                return hasher.ValidateHash(password, user!.PasswordHash);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool PerformLogout()
        {
            CurrentUser = PatternzUser.AnyUser;
            return true;
        }
    }
}
