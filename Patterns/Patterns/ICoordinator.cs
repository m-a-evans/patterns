using Patterns.Account;
using Patterns.Data.Model;
using Patterns.IO;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace Patterns
{
    public interface ICoordinator
    {
        /// <summary>
        /// Access to information about the current user, and login/logout functionality
        /// </summary>
        IUserManager UserManager { get; }

        /// <summary>
        /// Gets an account manager to manage users
        /// </summary>
        /// <returns></returns>
        IUserAccount GetUserAccountManager();

        /// <summary>
        /// Gets a data record manager of the specified format
        /// </summary>
        /// <param name="dataRecordManagerFormat"></param>
        /// <returns></returns>
        IDataRecordManager GetDataRecordManager(DataRecordFormat dataRecordManagerType);

        /// <summary>
        /// Configures the components of the Coordinator with application settings
        /// </summary>
        /// <param name="settings"></param>
        void Configure(ApplicationSettingsBase settings);
    }
}
