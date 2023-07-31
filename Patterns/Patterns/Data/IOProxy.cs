using CommunityToolkit.Diagnostics;
using Patterns.Account.Model;
using Patterns.Data.Model;
using Patterns.IO;

namespace Patterns.Data
{
    /// <summary>
    /// Proxy for DataRecordManager - checks to make sure the current user is allowed to perform operations before performing them
    /// </summary>
    internal class IOProxy : IDataRecordManager
    {
        private readonly IPatternzUser _user;
        private readonly IDataRecordManager _dataRecordManager;

        /// <summary>
        /// Creates a new instance of this class, with a user to check permissions against
        /// </summary>
        /// <param name="user">The user performing the operations</param>
        /// <param name="format">The format of data records to interface with</param>
        public IOProxy(IPatternzUser user, IDataRecordManager manager)
        {
            _user = user;
            _dataRecordManager = manager;
        }

        /// <summary>
        /// Attempts to parse a collection of data records by name
        /// </summary>
        /// <param name="recordName">The name of the data records to parse</param>
        /// <param name="dataFile">The resultant DataFile, if parsed successfully</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string recordName, out DataFile? dataFile)
        {
            dataFile = null;
            if ((_user.Permissions & Permission.ReadAccess) == Permission.ReadAccess)
            {
                return _dataRecordManager.TryParseRecords(recordName, out dataFile);
            }
            return ThrowHelper.ThrowUnauthorizedAccessException<bool>();
        }

        /// <summary>
        /// Writes a collection of data records to some storage medium
        /// </summary>
        /// <param name="dataFile">The DataFile to write to the store</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(DataFile dataFile)
        {
            if ((_user.Permissions & Permission.WriteAccess) == Permission.WriteAccess)
            {
                return _dataRecordManager.WriteDataRecords(dataFile);
            }
            return ThrowHelper.ThrowUnauthorizedAccessException<long>();
        }
    }
}
