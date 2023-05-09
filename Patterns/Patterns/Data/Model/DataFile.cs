using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Patterns.Data.Model
{
    public class DataFile
    {
        public string FileName { get; set; } = string.Empty;
        public string Path { get; set; } = string.Empty;
        public SortedDictionary<DateTime, DataRecord> DataRecords { get; set; } = new();
        public DataRecordFormat Format { get; set; }
    }
}
