using Patterns.Data.Model;

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
        /// <param name="file">The file containing the records to write</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(DataFile file);

        /// <summary>
        /// Attempts to parse a collection of data records by name
        /// </summary>
        /// <param name="recordName">The name of the data records to parse</param>
        /// <param name="file">The resultant file containing the records, if parsed successfully</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string recordName, out DataFile? file);
    }
}
