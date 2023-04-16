using Patterns.Account;

namespace Patterns
{
    /// <summary>
    /// Interface representing globally available fields
    /// </summary>
    public interface IGlobalState
    {
        /// <summary>
        /// The current user performing operations
        /// </summary>
        IPatternzUser CurrentUser { get; }

        /// <summary>
        /// A flag indicating a user is logged in
        /// </summary>
        bool IsLoggedIn { get; }

        /// <summary>
        /// Performs a login
        /// </summary>
        /// <param name="username">The user name</param>
        /// <param name="password">The password</param>
        /// <returns>True if the operation succeeded</returns>
        bool PerformLogin(string username, string password);

        /// <summary>
        /// Logs out the current user
        /// </summary>
        /// <returns></returns>
        bool PerformLogout();
    }
}
