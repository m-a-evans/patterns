using System;

namespace Patterns.Model.Data
{
    /// <summary>
    /// A record of data
    /// </summary>
    public class DataRecord
    {
        public Guid Id { get; set; }
        public string DataRecordName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; }

        public DateTime DateModified { get; set; }
    }
}
