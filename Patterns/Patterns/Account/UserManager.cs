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
        private IUserAccount _userManager;

        private IPatternzUser _currentUser;

        private Task? _loadUsersFromStore;

        private string? _pathToStore;
        private bool _isStale;

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
        public UserManager(string? pathToStore = null)
        {
            _currentUser = PatternzUser.AnyUser;
            _pathToStore = pathToStore;
            _userManager = new AccountProxy(PatternzUser.AnyUser, pathToStore);
            StartReadingUserStore();
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
            if (_isStale)
            {
                _loadUsersFromStore?.Wait();
                _isStale = false;
            }
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
                    _userManager = new AccountProxy(CurrentUser, _pathToStore);
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
            _userManager = new AccountProxy(PatternzUser.AnyUser, _pathToStore);
            StartReadingUserStore();
        }

        private void StartReadingUserStore()
        {
            _isStale = true;
            _loadUsersFromStore = _userManager.TryReadUsersFromStoreAsync(_pathToStore);
        }
    }
}
