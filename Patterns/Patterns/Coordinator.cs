using Patterns.Account;
using Patterns.Data.Model;
using Patterns.IO;

namespace Patterns
{
    /// <summary>
    /// Acts on behalf of the current user requests to get access to components of Patternz 
    /// </summary>
    public class Coordinator : ICoordinator
    {
        private static Coordinator _instance;
        /// <summary>
        /// Gets the Singleton instance
        /// </summary>
        public static ICoordinator Instance 
        { 
            get
            {
                return _instance ??= new Coordinator();
            }
        }
        private Coordinator()
        {
            UserManager = new UserManager();
        }

        /// <summary>
        /// Access to information about the current user, and login/logout functionality
        /// </summary>
        public IUserManager UserManager { get; private set; }

        /// <summary>
        /// Gets a data record manager of the specified format, gated by the current user's permissions
        /// </summary>
        /// <param name="dataRecordManagerFormat"></param>
        /// <returns></returns>
        public IDataRecordManager GetDataRecordManager(DataRecordFormat dataRecordManagerFormat)
        {
            return DataRecordManagerFactory.GetUserRecordManager(UserManager.CurrentUser, dataRecordManagerFormat);
        }

        /// <summary>
        /// Gets an account manager to manage users, gated by the current user's permissions
        /// </summary>
        /// <returns></returns>
        public IUserAccount GetUserAccountManager()
        {
            return new AccountProxy(UserManager.CurrentUser);
        }
    }
}
