using CommunityToolkit.Diagnostics;
using Patterns.Account;
using Patterns.IO;
using Patterns.Model.Data;
using System.Collections.Generic;

namespace Patterns.Proxy
{
    /// <summary>
    /// Proxy for DataRecordManager - checks to make sure the current user is allowed to perform operations before performing them
    /// </summary>
    internal class IOProxy : IDataRecordManager
    {
        private readonly IPatternzUser _user;
        private readonly IDataRecordManager _userManager;

        /// <summary>
        /// Creates a new instance of this class, with a user to check permissions against
        /// </summary>
        /// <param name="user">The user performing the operations</param>
        /// <param name="type">The type of data records to interface with</param>
        public IOProxy(IPatternzUser user, DataRecordManagerType type) 
        {
            _user = user;
            _userManager = DataRecordManagerFactory.GetUserRecordManager(type);
        }

        /// <summary>
        /// Attempts to parse a collection of data records by name
        /// </summary>
        /// <param name="recordName">The name of the data records to parse</param>
        /// <param name="dataRecords">The resultant list of records, if parsed successfully</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string recordName, out List<DataRecord> userRecord)
        {
            if ((_user.Permissions & Permission.ReadAccess) == Permission.ReadAccess)
            {
                return _userManager.TryParseRecords(recordName, out userRecord);
            }
            userRecord = new List<DataRecord>(); ;
            return ThrowHelper.ThrowUnauthorizedAccessException<bool>();
        }

        /// <summary>
        /// Writes a collection of data records to some storage medium
        /// </summary>
        /// <param name="collectionName">The name of the collection to write</param>
        /// <param name="dataRecords">The collection to write</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(string collectionName, List<DataRecord> userRecord)
        {
            if ((_user.Permissions & Permission.WriteAccess) == Permission.WriteAccess)
            {
                return _userManager.WriteDataRecords(collectionName, userRecord);
            }
            return ThrowHelper.ThrowUnauthorizedAccessException<long>();
        }
    }
}
