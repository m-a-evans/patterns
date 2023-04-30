using Patterns.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace Patterns.IO
{
    /// <summary>
    /// Implementation of the data record manager interface that handles JSON files
    /// </summary>
    internal class JsonDataRecordManager : IDataRecordManager
    {
        /// <summary>
        /// Attempts to parse a collection of data records from a JSON file
        /// </summary>
        /// <param name="jsonFileName">The name of the data records to parse</param>
        /// <param name="dataRecords">The resultant list of records, if parsed successfully</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string jsonFileName, out List<DataRecord> dataRecords)
        {
            dataRecords = new List<DataRecord>();
            if (!File.Exists(jsonFileName))
            {
                return false;
            }

            DataRecord[]? dataRecordsAsArr = JsonSerializer.Deserialize<DataRecord[]>(File.ReadAllText(jsonFileName));

            if (dataRecordsAsArr == null)
            {
                return false;
            }
            else
            {
                dataRecords = new List<DataRecord>(dataRecordsAsArr);
                return true;
            }
        }

        /// <summary>
        /// Writes a collection of data records to a JSON file
        /// </summary>
        /// <param name="collectionName">The name of the collection to write</param>
        /// <param name="dataRecords">The collection to write</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(string collectionName, List<DataRecord> dataRecords)
        {
            DataRecord[] userRecordsAsArr = dataRecords.ToArray();

            string serialized = JsonSerializer.Serialize(userRecordsAsArr);
            File.WriteAllText(collectionName, serialized);

            return Encoding.UTF8.GetByteCount(serialized);
        }
    }
}
