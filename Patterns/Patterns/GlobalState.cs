using Patterns.Account;
using Patterns.Proxy;
using System;

namespace Patterns
{
    /// <summary>
    /// Contains fields that are globally accessible to the application
    /// </summary>
    public class GlobalState : IGlobalState
    {
        private IUserAccount _userManager = new AccountProxy(PatternzUser.AnyUser);

        private IPatternzUser _currentUser;

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
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool PerformLogout()
        {
            throw new NotImplementedException();
        }
    }
}
