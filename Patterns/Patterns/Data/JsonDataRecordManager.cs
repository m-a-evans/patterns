using Patterns.Data.Model;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        /// <param name="file">The resultant DataFile, if parsed successfully</param>
        /// <returns>True if the records were able to be parsed</returns>
        public bool TryParseRecords(string jsonFileName, out DataFile? file)
        {
            List<DataRecord> dataRecords = new();
            file = null;

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
                file = new DataFile() { 
                    FileName = jsonFileName, 
                    Format = DataRecordFormat.Json, 
                    Path = Path.GetDirectoryName(jsonFileName) ?? string.Empty 
                };
                foreach (DataRecord record in dataRecordsAsArr)
                {
                    file.DataRecords.Add(record.CreatedDate, record);
                }
                return true;
            }
        }

        /// <summary>
        /// Writes a collection of data records to a JSON file
        /// </summary>
        /// <param name="dataFile">The DataFile to write. It will be written to the location in its FileName</param>
        /// <returns>The number of bytes written</returns>
        public long WriteDataRecords(DataFile dataFile)
        {
            string collectionName = dataFile.FileName;
            DataRecord[] userRecordsAsArr = dataFile.DataRecords.Values.ToArray();

            string serialized = JsonSerializer.Serialize(userRecordsAsArr);
            File.WriteAllText(collectionName, serialized);

            return Encoding.UTF8.GetByteCount(serialized);
        }
    }
}
