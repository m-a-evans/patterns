using Patterns.Data.Model;
using System.Collections.Generic;

namespace Patterns.IO
{
    /// <summary>
    /// Interface for managing user data records
    /// </summary>
    public interface IDataRecordManager
    {
        /// <summary>
        /// Writes a collection of data records to some storage medium
        /// </summary>
        /// <param name="collectionName">The name of the collection to write</param>
        /// <param name="dataRecords">The collection to write</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(string collectionName, List<DataRecord> dataRecords);

        /// <summary>
        /// Attempts to parse a collection of data records by name
        /// </summary>
        /// <param name="recordName">The name of the data records to parse</param>
        /// <param name="dataRecords">The resultant list of records, if parsed successfully</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string recordName, out List<DataRecord>? dataRecords);
    }
}
