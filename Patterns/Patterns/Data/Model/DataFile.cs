using System;
using System.Collections.Generic;

namespace Patterns.Data.Model
{
    public class DataFile 
    {
        public string FileName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public SortedDictionary<DateTime, DataRecord> DataRecords { get; set; } = new();
        public DataRecordFormat Format { get; set; }

        public DataFile DeepCopy()
        {
            var sortedCopy = new SortedDictionary<DateTime, DataRecord>();
            foreach (KeyValuePair<DateTime, DataRecord> record in DataRecords)
            {
                sortedCopy.Add(record.Key, record.Value.DeepCopy());
            }
            return new DataFile()
            {
                FileName = FileName,
                Path = Path,
                Format = Format,
                DataRecords = sortedCopy
            };
        }
    }
}
