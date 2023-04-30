using Patterns.Account.Model;
using Patterns.Security;
using System;
using System.Threading.Tasks;

namespace Patterns.Account
{
    /// <summary>
    /// Contains fields that are globally accessible to the application
    /// </summary>
    internal class UserManager : IUserManager
    {
        private IUserAccount _userManager = new AccountProxy(PatternzUser.AnyUser);

        private IPatternzUser _currentUser;

        private Task _loadUsersFromStore;

        public event EventHandler<CurrentUserChangedEventArgs>? CurrentUserChanged;

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
                IPatternzUser old = _currentUser;
                _currentUser = value;

                CurrentUserChanged?.Invoke(this, new CurrentUserChangedEventArgs(old, _currentUser));
            }
        }

        /// <summary>
        /// Constructor, sets current user to AnyUser
        /// </summary>
        public UserManager()
        {
            _currentUser = PatternzUser.AnyUser;
            _loadUsersFromStore = _userManager.TryReadUsersFromStoreAsync();
        }

        /// <summary>
        /// True if a user other than AnyUser is logged in
        /// </summary>
        public bool IsLoggedIn { get { return !CurrentUser.IsAnyUser; } }

        /// <summary>
        /// Attempts to perform a login for the given username and password
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns>True if the login succeeded</returns>
        public bool PerformLogin(string username, string password)
        {
            if (!_loadUsersFromStore.IsCompleted)
                _loadUsersFromStore.Wait();
            IPatternzUser? user;
            if (_userManager.TryGetUser(username, out user))
            {
#if DEBUG
                if (string.IsNullOrWhiteSpace(password)) { CurrentUser = user; return true; }
#endif
                Pbkdf2DataHasher hasher = new();

                if (hasher.ValidateHash(password, user!.PasswordHash))
                {
                    CurrentUser = user;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Logs out the current user
        /// </summary>
        /// <returns></returns>
        public void PerformLogout()
        {
            CurrentUser = PatternzUser.AnyUser;
        }
    }
}
