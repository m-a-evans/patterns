using Patterns.Account;
using Patterns.Data.Model;
using Patterns.IO;
using System.Configuration;

namespace Patterns
{
    /// <summary>
    /// Acts on behalf of the current user requests to get access to components of Patternz 
    /// </summary>
    public class Coordinator : ICoordinator
    {
        private static ICoordinator? _instance;
        private const string UserRecordLocationName = "UserRecordLocation";
        private string? _userRecordLocation;
        private UserManager _userManager;

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

        }

        /// <summary>
        /// Access to information about the current user, and login/logout functionality
        /// </summary>
        public IUserManager UserManager
        {
            get
            {
                return _userManager ??= new UserManager(_userRecordLocation);
            }
        }

        /// <summary>
        /// Gets a data record manager of the specified format, gated by the current user's permissions
        /// </summary>
        /// <param name="dataRecordManagerFormat"></param>
        /// <returns></returns>
        public IDataRecordManager GetDataRecordManager(DataRecordFormat dataRecordManagerFormat)
        {
            return DataRecordManagerFactory.GetDataRecordManager(UserManager.CurrentUser, dataRecordManagerFormat);
        }

        /// <summary>
        /// Gets an account manager to manage users, gated by the current user's permissions
        /// </summary>
        /// <returns></returns>
        public IUserAccount GetUserAccountManager()
        {
            return new AccountProxy(UserManager.CurrentUser, _userRecordLocation);
        }

        /// <summary>
        /// Configures the components of the Coordinator with application settings
        /// </summary>
        /// <param name="settings"></param>
        public void Configure(ApplicationSettingsBase settings)
        {
            string? location = settings.Properties[UserRecordLocationName].DefaultValue as string;
            if (_userRecordLocation != location || UserManager == null)
            {
                _userRecordLocation = location;
                _userManager = new UserManager(_userRecordLocation);
            }
        }

        /// <summary>
        /// Gets a data record manager of the format inferred from the filename, gated by the current user's permissions
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public IDataRecordManager GetDataRecordManager(string fileName)
        {
            return DataRecordManagerFactory.GetDataRecordManager(UserManager.CurrentUser, fileName);
        }
    }
}
